namespace SaaS.Domain.Modules.Tenant;

public sealed class TenantConfiguration
{
    public Guid Id { get; private set; }
    public Guid TenantId { get; private set; }
    public string Key { get; private set; } = string.Empty;
    public string Value { get; private set; } = string.Empty;
    public DateTime UpdatedAtUtc { get; private set; }

    private TenantConfiguration() { }

    public static TenantConfiguration Create(Guid tenantId, string key, string value, DateTime updatedAtUtc)
    {
        if (tenantId == Guid.Empty)
        {
            throw new ArgumentException("Tenant ID is required.", nameof(tenantId));
        }

        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentException("Configuration key is required.", nameof(key));
        }

        return new TenantConfiguration
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Key = key.Trim(),
            Value = value.Trim(),
            UpdatedAtUtc = updatedAtUtc
        };
    }
}
