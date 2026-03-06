using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SaaS.Application.Common;
using SaaS.Application.Contracts;
using SaaS.Application.Notification;
using SaaS.Application.User;
using SaaS.Domain.Modules.Identity;
using SaaS.Domain.Modules.User;
using SaaS.Infrastructure.Identity;
using SaaS.Infrastructure.Persistence;

namespace SaaS.Infrastructure.Services;

public sealed class UserService : IUserService, IInviteUserCommandHandler
{
    private readonly AppDbContext _dbContext;
    private readonly UserManager<AppUser> _userManager;
    private readonly IInvitationEmailSender _invitationEmailSender;
    private readonly TimeProvider _timeProvider;

    public UserService(
        AppDbContext dbContext,
        UserManager<AppUser> userManager,
        IInvitationEmailSender invitationEmailSender,
        TimeProvider timeProvider)
    {
        _dbContext = dbContext;
        _userManager = userManager;
        _invitationEmailSender = invitationEmailSender;
        _timeProvider = timeProvider;
    }

    public async Task<ApplicationResult<InviteUserResponse>> InviteUserAsync(InviteUserCommand command, CancellationToken cancellationToken)
        => await HandleAsync(command, cancellationToken);

    public async Task<ApplicationResult<InviteUserResponse>> HandleAsync(InviteUserCommand command, CancellationToken cancellationToken)
    {
        if (command.ActorUserId == Guid.Empty)
        {
            return ApplicationResult<InviteUserResponse>.Failure("identity.missing_user", "Authenticated user ID is required.", 401);
        }

        if (command.TenantId == Guid.Empty || string.IsNullOrWhiteSpace(command.Email))
        {
            return ApplicationResult<InviteUserResponse>.Failure("user.invalid_input", "Tenant and email are required.", 400);
        }

        var tenantExists = await _dbContext.Tenants.AnyAsync(x => x.Id == command.TenantId, cancellationToken);
        if (!tenantExists)
        {
            return ApplicationResult<InviteUserResponse>.Failure("tenant.not_found", "Tenant not found.", 404);
        }

        var normalizedEmail = command.Email.Trim().ToLowerInvariant();
        var duplicate = await _dbContext.UserProfiles.AnyAsync(
            x => x.TenantId == command.TenantId && x.Email == normalizedEmail,
            cancellationToken);

        if (duplicate)
        {
            return ApplicationResult<InviteUserResponse>.Failure("user.duplicate_email", "Email already exists in tenant.", 409);
        }

        var identityUser = new AppUser
        {
            Id = Guid.NewGuid(),
            UserName = normalizedEmail,
            Email = normalizedEmail,
            DisplayName = string.IsNullOrWhiteSpace(command.DisplayName) ? normalizedEmail : command.DisplayName.Trim(),
            EmailConfirmed = true
        };

        // Starter default for invitation bootstrap; reset flow can replace this in later slices.
        var createResult = await _userManager.CreateAsync(identityUser, "TempPass!12345");
        if (!createResult.Succeeded)
        {
            return ApplicationResult<InviteUserResponse>.Failure("identity.create_failed", "Could not create identity user.", 400);
        }

        var now = _timeProvider.GetUtcNow().UtcDateTime;
        var profile = UserProfile.Invite(identityUser.Id, command.TenantId, normalizedEmail, identityUser.DisplayName, now);
        var membership = UserMembership.Create(profile.Id, command.TenantId, RoleKeys.TenantUser, now);

        _dbContext.UserProfiles.Add(profile);
        _dbContext.UserMemberships.Add(membership);
        await _dbContext.SaveChangesAsync(cancellationToken);

        await _invitationEmailSender.SendInvitationAsync(
            new InvitationEmailMessage(profile.Id, profile.TenantId, profile.Email, profile.DisplayName),
            cancellationToken);

        return ApplicationResult<InviteUserResponse>.Success(
            new InviteUserResponse(profile.Id, profile.TenantId, profile.Email, profile.Status.ToString()));
    }
}
