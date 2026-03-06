namespace SaaS.Application.User;

public sealed record InviteUserCommand(Guid TenantId, string Email, string DisplayName, string InitialRoleKey, Guid ActorUserId);

public sealed record InviteUserResponse(Guid UserProfileId, Guid TenantId, string Email, string Status);
