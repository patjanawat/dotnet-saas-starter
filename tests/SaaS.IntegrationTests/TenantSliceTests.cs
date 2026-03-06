using System.Net;
using System.Net.Http.Json;

namespace SaaS.IntegrationTests;

public sealed class TenantSliceTests : IClassFixture<TestApiFactory>
{
    private readonly TestApiFactory _factory;

    public TenantSliceTests(TestApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task CreateTenant_ThenGetById_ReturnsCreatedTenant()
    {
        using var client = _factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false, HandleCookies = true });
        await LoginAsPlatformAdmin(client);

        var createResponse = await client.PostAsJsonAsync("/api/tenant/tenants", new
        {
            code = "acme",
            name = "Acme"
        });

        Assert.Equal(HttpStatusCode.OK, createResponse.StatusCode);
        var created = await createResponse.Content.ReadFromJsonAsync<TenantContract>();
        Assert.NotNull(created);
        Assert.Equal("acme", created!.code);

        var getResponse = await client.GetAsync($"/api/tenant/tenants/{created.id}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        var fetched = await getResponse.Content.ReadFromJsonAsync<TenantContract>();
        Assert.NotNull(fetched);
        Assert.Equal(created.id, fetched!.id);
        Assert.Equal("Pending", fetched.status);
    }

    [Fact]
    public async Task CreateTenant_WithDuplicateSlug_ReturnsConflictProblemDetails()
    {
        using var client = _factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false, HandleCookies = true });
        await LoginAsPlatformAdmin(client);

        var payload = new { code = "acme-dup", name = "Acme Duplicate" };
        var first = await client.PostAsJsonAsync("/api/tenant/tenants", payload);
        Assert.Equal(HttpStatusCode.OK, first.StatusCode);

        var second = await client.PostAsJsonAsync("/api/tenant/tenants", payload);
        Assert.Equal(HttpStatusCode.Conflict, second.StatusCode);

        var body = await second.Content.ReadFromJsonAsync<ProblemContract>();
        Assert.NotNull(body);
        Assert.Equal("tenant.duplicate_code", body!.errorCode);
        Assert.False(string.IsNullOrWhiteSpace(body.traceId));
    }

    [Fact]
    public async Task CreateTenant_WithInvalidSlug_ReturnsBadRequestProblemDetails()
    {
        using var client = _factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false, HandleCookies = true });
        await LoginAsPlatformAdmin(client);

        var response = await client.PostAsJsonAsync("/api/tenant/tenants", new
        {
            code = "Invalid_Slug",
            name = "Acme Bad"
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<ProblemContract>();
        Assert.NotNull(body);
        Assert.Equal("tenant.invalid_slug", body!.errorCode);
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

    private sealed record TenantContract(Guid id, string code, string name, string status);
    private sealed record ProblemContract(string? type, string? title, int status, string? detail, string? instance, string? traceId, string? errorCode);
}
