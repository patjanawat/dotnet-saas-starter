namespace SaaS.Domain.Common;

public abstract class AuditableEntityBase : EntityBase, IAuditableEntity
{
    public DateTime CreatedAtUtc { get; protected set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; protected set; } = DateTime.UtcNow;

    protected void Touch(DateTime nowUtc)
    {
        UpdatedAtUtc = nowUtc;
    }
}
