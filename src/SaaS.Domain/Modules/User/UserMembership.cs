namespace SaaS.Domain.Modules.User;

public sealed class UserMembership
{
    public Guid Id { get; private set; }
    public Guid UserProfileId { get; private set; }
    public Guid TenantId { get; private set; }
    public string RoleKey { get; private set; } = string.Empty;
    public UserStatus Status { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    private UserMembership() { }

    public static UserMembership Create(Guid userProfileId, Guid tenantId, string roleKey, DateTime createdAtUtc)
    {
        if (userProfileId == Guid.Empty)
        {
            throw new ArgumentException("User profile ID is required.", nameof(userProfileId));
        }

        if (tenantId == Guid.Empty)
        {
            throw new ArgumentException("Tenant ID is required.", nameof(tenantId));
        }

        if (string.IsNullOrWhiteSpace(roleKey))
        {
            throw new ArgumentException("Role key is required.", nameof(roleKey));
        }

        return new UserMembership
        {
            Id = Guid.NewGuid(),
            UserProfileId = userProfileId,
            TenantId = tenantId,
            RoleKey = roleKey.Trim().ToLowerInvariant(),
            Status = UserStatus.Active,
            CreatedAtUtc = createdAtUtc
        };
    }
}
