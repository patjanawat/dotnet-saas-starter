using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace SaaS.IntegrationTests;

public sealed class ApiSlicesTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public ApiSlicesTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.UseSetting("Database:UseInMemory", "true");
        });
    }

    [Fact]
    public async Task Health_Endpoints_ReturnSuccess()
    {
        using var client = _factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

        var live = await client.GetAsync("/health/live");
        var ready = await client.GetAsync("/health/ready");

        Assert.Equal(HttpStatusCode.OK, live.StatusCode);
        Assert.Equal(HttpStatusCode.OK, ready.StatusCode);
    }

    [Fact]
    public async Task Wave_Slices_Login_CreateTenant_Invite_AssignRole_Work()
    {
        using var client = _factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false, HandleCookies = true });

        var loginResponse = await client.PostAsJsonAsync("/api/identity/auth/sign-in", new
        {
            userNameOrEmail = "admin@saas.local",
            password = "Admin!12345"
        });
        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

        var createTenantResponse = await client.PostAsJsonAsync("/api/tenant/tenants", new
        {
            code = "acme",
            name = "Acme"
        });
        Assert.Equal(HttpStatusCode.OK, createTenantResponse.StatusCode);

        var tenant = await createTenantResponse.Content.ReadFromJsonAsync<TenantContract>();
        Assert.NotNull(tenant);

        var inviteResponse = await client.PostAsJsonAsync("/api/user/users", new
        {
            tenantId = tenant!.id,
            email = "first.user@acme.test",
            displayName = "First User",
            initialRoleKey = "tenant_user"
        });
        Assert.Equal(HttpStatusCode.OK, inviteResponse.StatusCode);

        var invited = await inviteResponse.Content.ReadFromJsonAsync<InviteContract>();
        Assert.NotNull(invited);

        var assignRoleResponse = await client.PostAsJsonAsync($"/api/user/users/{invited!.userProfileId}/roles", new
        {
            tenantId = tenant.id,
            roleKey = "tenant_admin"
        });

        Assert.Equal(HttpStatusCode.OK, assignRoleResponse.StatusCode);
    }

    private sealed record TenantContract(Guid id, string code, string name, string status);
    private sealed record InviteContract(Guid userProfileId, Guid tenantId, string email, string status);
}
