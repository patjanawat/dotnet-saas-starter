namespace SaaS.Api.Authorization;

public sealed record AssignRoleRequest(Guid TenantId, string RoleKey);

public sealed record AssignRoleResponseModel(Guid UserProfileId, Guid TenantId, string RoleKey);
