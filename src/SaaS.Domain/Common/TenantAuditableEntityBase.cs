namespace SaaS.Domain.Common;

public abstract class TenantAuditableEntityBase : AuditableEntityBase, ITenantOwnedEntity
{
    public Guid TenantId { get; protected set; }
}
