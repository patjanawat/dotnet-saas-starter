namespace SaaS.Domain.Modules.User;

public sealed class UserProfile
{
    public Guid Id { get; private set; }
    public Guid IdentityUserId { get; private set; }
    public Guid TenantId { get; private set; }
    public string Email { get; private set; } = string.Empty;
    public string DisplayName { get; private set; } = string.Empty;
    public UserStatus Status { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime UpdatedAtUtc { get; private set; }

    private UserProfile() { }

    public static UserProfile Invite(Guid identityUserId, Guid tenantId, string email, string displayName, DateTime nowUtc)
    {
        if (identityUserId == Guid.Empty)
        {
            throw new ArgumentException("Identity user ID is required.", nameof(identityUserId));
        }

        if (tenantId == Guid.Empty)
        {
            throw new ArgumentException("Tenant ID is required.", nameof(tenantId));
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("Email is required.", nameof(email));
        }

        return new UserProfile
        {
            Id = Guid.NewGuid(),
            IdentityUserId = identityUserId,
            TenantId = tenantId,
            Email = email.Trim().ToLowerInvariant(),
            DisplayName = string.IsNullOrWhiteSpace(displayName) ? email.Trim() : displayName.Trim(),
            Status = UserStatus.Invited,
            CreatedAtUtc = nowUtc,
            UpdatedAtUtc = nowUtc
        };
    }
}
