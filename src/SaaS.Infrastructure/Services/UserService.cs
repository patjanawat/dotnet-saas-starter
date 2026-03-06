using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SaaS.Application.Common;
using SaaS.Application.Contracts;
using SaaS.Application.User;
using SaaS.Domain.Modules.User;
using SaaS.Infrastructure.Identity;
using SaaS.Infrastructure.Persistence;

namespace SaaS.Infrastructure.Services;

public sealed class UserService : IUserService
{
    private readonly AppDbContext _dbContext;
    private readonly UserManager<AppUser> _userManager;
    private readonly TimeProvider _timeProvider;

    public UserService(AppDbContext dbContext, UserManager<AppUser> userManager, TimeProvider timeProvider)
    {
        _dbContext = dbContext;
        _userManager = userManager;
        _timeProvider = timeProvider;
    }

    public async Task<ApplicationResult<InviteUserResponse>> InviteUserAsync(InviteUserCommand command, CancellationToken cancellationToken)
    {
        if (command.TenantId == Guid.Empty || string.IsNullOrWhiteSpace(command.Email))
        {
            return ApplicationResult<InviteUserResponse>.Failure("user.invalid_input", "Tenant and email are required.");
        }

        var tenantExists = await _dbContext.Tenants.AnyAsync(x => x.Id == command.TenantId, cancellationToken);
        if (!tenantExists)
        {
            return ApplicationResult<InviteUserResponse>.Failure("tenant.not_found", "Tenant not found.");
        }

        var normalizedEmail = command.Email.Trim().ToLowerInvariant();
        var duplicate = await _dbContext.UserProfiles.AnyAsync(
            x => x.TenantId == command.TenantId && x.Email == normalizedEmail,
            cancellationToken);

        if (duplicate)
        {
            return ApplicationResult<InviteUserResponse>.Failure("user.duplicate_email", "Email already exists in tenant.");
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
            return ApplicationResult<InviteUserResponse>.Failure("identity.create_failed", "Could not create identity user.");
        }

        var now = _timeProvider.GetUtcNow().UtcDateTime;
        var profile = UserProfile.Invite(identityUser.Id, command.TenantId, normalizedEmail, identityUser.DisplayName, now);
        var membership = UserMembership.Create(profile.Id, command.TenantId, command.InitialRoleKey, now);

        _dbContext.UserProfiles.Add(profile);
        _dbContext.UserMemberships.Add(membership);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return ApplicationResult<InviteUserResponse>.Success(
            new InviteUserResponse(profile.Id, profile.TenantId, profile.Email, profile.Status.ToString()));
    }
}
