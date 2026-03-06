using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace SaaS.IntegrationTests;

public sealed class TestApiFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        var inMemoryName = $"saas-test-{Guid.NewGuid():N}";
        builder.UseSetting("Database:UseInMemory", "true");
        builder.UseSetting("Database:InMemoryName", inMemoryName);
        builder.ConfigureServices(services =>
        {
            services.PostConfigure<CookieAuthenticationOptions>(IdentityConstants.ApplicationScheme, options =>
            {
                options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
            });
        });
    }
}
