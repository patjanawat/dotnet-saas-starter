namespace SaaS.Api.Tenant;

public sealed record CreateTenantRequest(string Code, string Name);

public sealed record TenantResponseModel(Guid Id, string Code, string Name, string Status);
