namespace SaaS.Application.Notification;

public sealed record InvitationEmailMessage(Guid UserProfileId, Guid TenantId, string RecipientEmail, string DisplayName);
