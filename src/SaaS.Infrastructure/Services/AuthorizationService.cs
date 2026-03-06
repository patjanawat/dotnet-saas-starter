using Microsoft.EntityFrameworkCore;
using SaaS.Application.Authorization;
using SaaS.Application.Common;
using SaaS.Application.Contracts;
using SaaS.Domain.Modules.User;
using SaaS.Infrastructure.Persistence;

namespace SaaS.Infrastructure.Services;

public sealed class AuthorizationService : IAuthorizationService, IAssignRoleCommandHandler
{
    private readonly AppDbContext _dbContext;
    private readonly TimeProvider _timeProvider;

    public AuthorizationService(AppDbContext dbContext, TimeProvider timeProvider)
    {
        _dbContext = dbContext;
        _timeProvider = timeProvider;
    }

    public async Task<ApplicationResult<AssignRoleResponse>> AssignRoleAsync(AssignRoleCommand command, CancellationToken cancellationToken)
        => await HandleAsync(command, cancellationToken);

    public async Task<ApplicationResult<AssignRoleResponse>> HandleAsync(AssignRoleCommand command, CancellationToken cancellationToken)
    {
        if (command.ActorUserId == Guid.Empty)
        {
            return ApplicationResult<AssignRoleResponse>.Failure("identity.missing_user", "Authenticated user ID is required.", 401);
        }

        if (command.UserProfileId == Guid.Empty || command.TenantId == Guid.Empty || string.IsNullOrWhiteSpace(command.RoleKey))
        {
            return ApplicationResult<AssignRoleResponse>.Failure("authorization.invalid_input", "User, tenant and role are required.", 400);
        }

        var roleKey = command.RoleKey.Trim().ToLowerInvariant();
        var roleExists = await _dbContext.RoleDefinitions.AnyAsync(x => x.Key == roleKey, cancellationToken);
        if (!roleExists)
        {
            return ApplicationResult<AssignRoleResponse>.Failure("authorization.role_not_found", "Role not found.", 404);
        }

        var userExists = await _dbContext.UserProfiles.AnyAsync(
            x => x.Id == command.UserProfileId && x.TenantId == command.TenantId,
            cancellationToken);

        if (!userExists)
        {
            return ApplicationResult<AssignRoleResponse>.Failure("user.not_found", "User not found in tenant.", 404);
        }

        var existing = await _dbContext.UserMemberships.AnyAsync(
            x => x.UserProfileId == command.UserProfileId && x.TenantId == command.TenantId && x.RoleKey == roleKey,
            cancellationToken);

        if (!existing)
        {
            var membership = UserMembership.Create(command.UserProfileId, command.TenantId, roleKey, _timeProvider.GetUtcNow().UtcDateTime);
            _dbContext.UserMemberships.Add(membership);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        return ApplicationResult<AssignRoleResponse>.Success(
            new AssignRoleResponse(command.UserProfileId, command.TenantId, roleKey));
    }
}
