using Microsoft.EntityFrameworkCore;
using SaaS.Application.Contracts;
using SaaS.Domain.Modules.Identity;
using SaaS.Infrastructure.Persistence;

namespace SaaS.Infrastructure.Services;

public sealed class AccessControlService : IAccessControlService
{
    private readonly AppDbContext _dbContext;

    public AccessControlService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> CanManageTenantAsync(Guid actorUserId, Guid tenantId, CancellationToken cancellationToken)
    {
        if (actorUserId == Guid.Empty || tenantId == Guid.Empty)
        {
            return false;
        }

        var actorProfile = await _dbContext.UserProfiles
            .Where(x => x.IdentityUserId == actorUserId && x.TenantId == tenantId)
            .Select(x => x.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (actorProfile == Guid.Empty)
        {
            return false;
        }

        return await _dbContext.UserMemberships.AnyAsync(
            x => x.UserProfileId == actorProfile
                && x.TenantId == tenantId
                && (x.RoleKey == RoleKeys.TenantAdmin || x.RoleKey == RoleKeys.PlatformAdmin),
            cancellationToken);
    }
}
