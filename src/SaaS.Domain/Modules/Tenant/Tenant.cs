namespace SaaS.Domain.Modules.Tenant;

public sealed class Tenant
{
    public Guid Id { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public TenantStatus Status { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? ActivatedAtUtc { get; private set; }
    public DateTime? SuspendedAtUtc { get; private set; }
    public DateTime? ArchivedAtUtc { get; private set; }

    private Tenant() { }

    public static Tenant Create(string code, string name, DateTime createdAtUtc)
    {
        if (!TenantSlug.TryNormalize(code, out var normalizedCode))
        {
            throw new ArgumentException("Tenant code must be a valid slug.", nameof(code));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Tenant name is required.", nameof(name));
        }

        return new Tenant
        {
            Id = Guid.NewGuid(),
            Code = normalizedCode,
            Name = name.Trim(),
            Status = TenantStatus.Pending,
            CreatedAtUtc = createdAtUtc
        };
    }
}
