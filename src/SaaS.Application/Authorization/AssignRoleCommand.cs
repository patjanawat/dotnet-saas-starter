namespace SaaS.Application.Authorization;

public sealed record AssignRoleCommand(Guid UserProfileId, Guid TenantId, string RoleKey, Guid ActorUserId);

public sealed record AssignRoleResponse(Guid UserProfileId, Guid TenantId, string RoleKey);
