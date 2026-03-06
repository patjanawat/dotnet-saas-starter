namespace SaaS.Application.Contracts;

public interface IAccessControlService
{
    Task<bool> CanManageTenantAsync(Guid actorUserId, Guid tenantId, CancellationToken cancellationToken);
}
