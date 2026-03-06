namespace SaaS.Domain.Modules.Identity;

public sealed class IdentitySessionRecord
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string SessionId { get; private set; } = string.Empty;
    public DateTime IssuedAtUtc { get; private set; }
    public DateTime ExpiresAtUtc { get; private set; }
    public DateTime? RevokedAtUtc { get; private set; }

    private IdentitySessionRecord() { }

    public static IdentitySessionRecord Issue(Guid userId, string sessionId, DateTime issuedAtUtc, DateTime expiresAtUtc)
    {
        if (userId == Guid.Empty)
        {
            throw new ArgumentException("User ID is required.", nameof(userId));
        }

        if (string.IsNullOrWhiteSpace(sessionId))
        {
            throw new ArgumentException("Session ID is required.", nameof(sessionId));
        }

        return new IdentitySessionRecord
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            SessionId = sessionId.Trim(),
            IssuedAtUtc = issuedAtUtc,
            ExpiresAtUtc = expiresAtUtc
        };
    }
}
