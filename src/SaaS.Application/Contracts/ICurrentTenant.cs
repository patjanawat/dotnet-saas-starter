namespace SaaS.Application.Contracts;

public interface ICurrentTenant
{
    Guid? TenantId { get; }
    bool HasTenant { get; }
}
