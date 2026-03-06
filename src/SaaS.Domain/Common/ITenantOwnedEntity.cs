namespace SaaS.Domain.Common;

public interface ITenantOwnedEntity
{
    Guid TenantId { get; }
}
