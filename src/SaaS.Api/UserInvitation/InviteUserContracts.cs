namespace SaaS.Api.UserInvitation;

public sealed record InviteUserRequest(Guid TenantId, string Email, string DisplayName);

public sealed record InviteUserResponseModel(Guid UserProfileId, Guid TenantId, string Email, string Status);
