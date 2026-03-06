using System.Net;
using System.Net.Http.Json;

namespace SaaS.IntegrationTests;

public sealed class UserInvitationSliceTests : IClassFixture<TestApiFactory>
{
    private readonly TestApiFactory _factory;

    public UserInvitationSliceTests(TestApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task InviteUser_WithPlatformAdmin_ReturnsInvitedUser()
    {
        using var client = _factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false, HandleCookies = true });
        await LoginAsPlatformAdmin(client);
        var tenant = await CreateTenant(client, "invite-tenant-1", "Invite Tenant 1");

        var invite = await client.PostAsJsonAsync("/api/user/users", new
        {
            tenantId = tenant.id,
            email = "invite.one@tenant.test",
            displayName = "Invite One"
        });

        Assert.Equal(HttpStatusCode.OK, invite.StatusCode);
        var invited = await invite.Content.ReadFromJsonAsync<InviteContract>();
        Assert.NotNull(invited);
        Assert.Equal(tenant.id, invited!.tenantId);
        Assert.Equal("Invited", invited.status);
    }

    [Fact]
    public async Task InviteUser_DuplicateEmailInTenant_ReturnsConflictProblemDetails()
    {
        using var client = _factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false, HandleCookies = true });
        await LoginAsPlatformAdmin(client);
        var tenant = await CreateTenant(client, "invite-tenant-2", "Invite Tenant 2");

        var payload = new
        {
            tenantId = tenant.id,
            email = "dup@tenant.test",
            displayName = "Dup User"
        };

        var first = await client.PostAsJsonAsync("/api/user/users", payload);
        Assert.Equal(HttpStatusCode.OK, first.StatusCode);

        var second = await client.PostAsJsonAsync("/api/user/users", payload);
        Assert.Equal(HttpStatusCode.Conflict, second.StatusCode);
        var body = await second.Content.ReadFromJsonAsync<ProblemContract>();
        Assert.NotNull(body);
        Assert.Equal("user.duplicate_email", body!.errorCode);
    }

    [Fact]
    public async Task InviteUser_WithTenantUserRole_IsForbidden()
    {
        using var adminClient = _factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false, HandleCookies = true });
        await LoginAsPlatformAdmin(adminClient);
        var tenant = await CreateTenant(adminClient, "invite-tenant-3", "Invite Tenant 3");

        var initialInvite = await adminClient.PostAsJsonAsync("/api/user/users", new
        {
            tenantId = tenant.id,
            email = "tenant.user@tenant.test",
            displayName = "Tenant User"
        });
        Assert.Equal(HttpStatusCode.OK, initialInvite.StatusCode);

        using var tenantUserClient = _factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false, HandleCookies = true });
        var loginAsTenantUser = await tenantUserClient.PostAsJsonAsync("/api/identity/auth/sign-in", new
        {
            userNameOrEmail = "tenant.user@tenant.test",
            password = "TempPass!12345"
        });
        Assert.Equal(HttpStatusCode.OK, loginAsTenantUser.StatusCode);

        var forbiddenInvite = await tenantUserClient.PostAsJsonAsync("/api/user/users", new
        {
            tenantId = tenant.id,
            email = "unauthorized.invite@tenant.test",
            displayName = "Unauthorized Invite"
        });

        Assert.Equal(HttpStatusCode.Forbidden, forbiddenInvite.StatusCode);
        var body = await forbiddenInvite.Content.ReadFromJsonAsync<ProblemContract>();
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
        var createResponse = await client.PostAsJsonAsync("/api/tenant/tenants", new { code, name });
        Assert.Equal(HttpStatusCode.OK, createResponse.StatusCode);
        var tenant = await createResponse.Content.ReadFromJsonAsync<TenantContract>();
        Assert.NotNull(tenant);
        return tenant!;
    }

    private sealed record TenantContract(Guid id, string code, string name, string status);
    private sealed record InviteContract(Guid userProfileId, Guid tenantId, string email, string status);
    private sealed record ProblemContract(string? type, string? title, int status, string? detail, string? instance, string? traceId, string? errorCode);
}
