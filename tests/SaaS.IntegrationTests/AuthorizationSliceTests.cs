using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace SaaS.IntegrationTests;

public sealed class AuthorizationSliceTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public AuthorizationSliceTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder => builder.UseSetting("Database:UseInMemory", "true"));
    }

    [Fact]
    public async Task AssignRole_WithPlatformAdmin_ReturnsOk()
    {
        using var client = _factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false, HandleCookies = true });
        await LoginAsPlatformAdmin(client);
        var tenant = await CreateTenant(client, "auth-tenant-1", "Auth Tenant 1");
        var invited = await InviteUser(client, tenant.id, "assign.role@tenant.test");

        var assign = await client.PostAsJsonAsync($"/api/user/users/{invited.userProfileId}/roles", new
        {
            tenantId = tenant.id,
            roleKey = "tenant_admin"
        });

        Assert.Equal(HttpStatusCode.OK, assign.StatusCode);
        var response = await assign.Content.ReadFromJsonAsync<AssignRoleContract>();
        Assert.NotNull(response);
        Assert.Equal("tenant_admin", response!.roleKey);
    }

    [Fact]
    public async Task AssignRole_WithUnknownRole_ReturnsNotFoundProblemDetails()
    {
        using var client = _factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false, HandleCookies = true });
        await LoginAsPlatformAdmin(client);
        var tenant = await CreateTenant(client, "auth-tenant-2", "Auth Tenant 2");
        var invited = await InviteUser(client, tenant.id, "unknown.role@tenant.test");

        var assign = await client.PostAsJsonAsync($"/api/user/users/{invited.userProfileId}/roles", new
        {
            tenantId = tenant.id,
            roleKey = "not_a_role"
        });

        Assert.Equal(HttpStatusCode.NotFound, assign.StatusCode);
        var body = await assign.Content.ReadFromJsonAsync<ProblemContract>();
        Assert.NotNull(body);
        Assert.Equal("authorization.role_not_found", body!.errorCode);
    }

    [Fact]
    public async Task AssignRole_WithTenantUserRole_IsForbidden()
    {
        using var adminClient = _factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false, HandleCookies = true });
        await LoginAsPlatformAdmin(adminClient);
        var tenant = await CreateTenant(adminClient, "auth-tenant-3", "Auth Tenant 3");
        var target = await InviteUser(adminClient, tenant.id, "target.user@tenant.test");
        await InviteUser(adminClient, tenant.id, "plain.user@tenant.test");

        using var tenantUserClient = _factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false, HandleCookies = true });
        var login = await tenantUserClient.PostAsJsonAsync("/api/identity/auth/sign-in", new
        {
            userNameOrEmail = "plain.user@tenant.test",
            password = "TempPass!12345"
        });
        Assert.Equal(HttpStatusCode.OK, login.StatusCode);

        var assign = await tenantUserClient.PostAsJsonAsync($"/api/user/users/{target.userProfileId}/roles", new
        {
            tenantId = tenant.id,
            roleKey = "tenant_admin"
        });

        Assert.Equal(HttpStatusCode.Forbidden, assign.StatusCode);
        var body = await assign.Content.ReadFromJsonAsync<ProblemContract>();
        Assert.NotNull(body);
        Assert.Equal("authorization.forbidden", body!.errorCode);
    }

    private static async Task LoginAsPlatformAdmin(HttpClient client)
    {
        var login = await client.PostAsJsonAsync("/api/identity/auth/sign-in", new
        {
            userNameOrEmail = "admin@saas.local",
            password = "Admin!12345"
        });
        Assert.Equal(HttpStatusCode.OK, login.StatusCode);
    }

    private static async Task<TenantContract> CreateTenant(HttpClient client, string code, string name)
    {
        var create = await client.PostAsJsonAsync("/api/tenant/tenants", new { code, name });
        Assert.Equal(HttpStatusCode.OK, create.StatusCode);
        var tenant = await create.Content.ReadFromJsonAsync<TenantContract>();
        Assert.NotNull(tenant);
        return tenant!;
    }

    private static async Task<InviteContract> InviteUser(HttpClient client, Guid tenantId, string email)
    {
        var invite = await client.PostAsJsonAsync("/api/user/users", new
        {
            tenantId,
            email,
            displayName = email
        });
        Assert.Equal(HttpStatusCode.OK, invite.StatusCode);
        var body = await invite.Content.ReadFromJsonAsync<InviteContract>();
        Assert.NotNull(body);
        return body!;
    }

    private sealed record TenantContract(Guid id, string code, string name, string status);
    private sealed record InviteContract(Guid userProfileId, Guid tenantId, string email, string status);
    private sealed record AssignRoleContract(Guid userProfileId, Guid tenantId, string roleKey);
    private sealed record ProblemContract(string? type, string? title, int status, string? detail, string? instance, string? traceId, string? errorCode);
}
