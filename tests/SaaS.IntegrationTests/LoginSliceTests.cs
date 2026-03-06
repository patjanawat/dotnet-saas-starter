using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace SaaS.IntegrationTests;

public sealed class LoginSliceTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public LoginSliceTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder => builder.UseSetting("Database:UseInMemory", "true"));
    }

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsOk_AndAuthCookie()
    {
        using var client = _factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false, HandleCookies = false });

        var response = await client.PostAsJsonAsync("/api/identity/auth/sign-in", new
        {
            userNameOrEmail = "admin@saas.local",
            password = "Admin!12345"
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(response.Headers.TryGetValues("Set-Cookie", out var cookies));
        Assert.Contains(cookies!, c => c.Contains(".AspNetCore.Identity.Application"));
    }

    [Fact]
    public async Task Login_WithInvalidCredentials_ReturnsUnauthorizedProblemDetails()
    {
        using var client = _factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

        var response = await client.PostAsJsonAsync("/api/identity/auth/sign-in", new
        {
            userNameOrEmail = "admin@saas.local",
            password = "wrong-password"
        });

        var body = await response.Content.ReadFromJsonAsync<ProblemContract>();
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.NotNull(body);
        Assert.Equal("identity.invalid_credentials", body!.errorCode);
        Assert.False(string.IsNullOrWhiteSpace(body.traceId));
    }

    private sealed record ProblemContract(string? type, string? title, int status, string? detail, string? instance, string? traceId, string? errorCode);
}
