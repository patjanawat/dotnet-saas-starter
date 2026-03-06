using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace SaaS.IntegrationTests;

public sealed class TestApiFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        var inMemoryName = $"saas-test-{Guid.NewGuid():N}";
        builder.UseSetting("Database:UseInMemory", "true");
        builder.UseSetting("Database:InMemoryName", inMemoryName);
    }
}
