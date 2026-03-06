using System.Net;
using System.Net.Http.Json;

namespace SaaS.IntegrationTests;

public sealed class FoundationBaselineTests : IClassFixture<TestApiFactory>
{
    private readonly TestApiFactory _factory;

    public FoundationBaselineTests(TestApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task HealthEndpoints_ReturnExpectedStatus()
    {
        using var client = _factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

        var live = await client.GetAsync("/health/live");
        var ready = await client.GetAsync("/health/ready");

        Assert.Equal(HttpStatusCode.OK, live.StatusCode);
        Assert.Equal(HttpStatusCode.OK, ready.StatusCode);
    }

    [Fact]
    public async Task UnhandledException_ReturnsProblemDetails()
    {
        using var client = _factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
        var response = await client.GetAsync("/api/foundation/throw");
        var body = await response.Content.ReadFromJsonAsync<ProblemContract>();

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        Assert.NotNull(body);
        Assert.Equal(500, body!.status);
        Assert.Equal("system.unhandled_exception", body.errorCode);
        Assert.False(string.IsNullOrWhiteSpace(body.traceId));
    }

    private sealed record ProblemContract(string? type, string? title, int status, string? detail, string? instance, string? traceId, string? errorCode);
}
