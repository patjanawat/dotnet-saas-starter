using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SaaS.Application.Contracts;
using SaaS.Application.Identity;
using SaaS.Application.User;
using SaaS.Infrastructure.Identity;
using SaaS.Infrastructure.Persistence;
using SaaS.Infrastructure.Seed;
using SaaS.Infrastructure.Services;

namespace SaaS.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddSaaSInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var useInMemory = configuration.GetValue("Database:UseInMemory", false);
        if (useInMemory)
        {
            services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("saas-starter"));
        }
        else
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? "Host=localhost;Port=5432;Database=saas_starter;Username=postgres;Password=postgres";
            services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));
        }

        services.AddIdentity<AppUser, IdentityRole<Guid>>(options =>
            {
                options.Password.RequiredLength = 10;
                options.Password.RequireUppercase = true;
                options.Password.RequireDigit = true;
                options.Password.RequireNonAlphanumeric = true;
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

        services.ConfigureApplicationCookie(options =>
        {
            options.Cookie.HttpOnly = true;
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            options.Cookie.SameSite = SameSiteMode.Lax;
            options.SlidingExpiration = true;
            options.ExpireTimeSpan = TimeSpan.FromHours(8);
        });

        services.AddScoped<IIdentityService, IdentityService>();
        services.AddScoped<ILoginCommandHandler, IdentityService>();
        services.AddScoped<ITenantService, TenantService>();
        services.AddScoped<ICreateTenantCommandHandler, TenantService>();
        services.AddScoped<IGetTenantByIdQueryHandler, TenantService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IInviteUserCommandHandler, UserService>();
        services.AddScoped<IAuthorizationService, AuthorizationService>();
        services.AddScoped<IAccessControlService, AccessControlService>();
        services.AddScoped<IInvitationEmailSender, LoggingInvitationEmailSender>();

        services.AddHostedService<IdentitySeedHostedService>();
        services.AddSingleton(TimeProvider.System);

        return services;
    }
}
