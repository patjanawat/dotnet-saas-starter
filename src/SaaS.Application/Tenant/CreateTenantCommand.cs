namespace SaaS.Application.Tenant;

public sealed record CreateTenantCommand(string Code, string Name, Guid ActorUserId);

public sealed record TenantResponse(Guid Id, string Code, string Name, string Status);
