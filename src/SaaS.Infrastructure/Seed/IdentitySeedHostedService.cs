using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SaaS.Domain.Modules.Authorization;
using SaaS.Domain.Modules.Identity;
using SaaS.Infrastructure.Identity;
using SaaS.Infrastructure.Persistence;

namespace SaaS.Infrastructure.Seed;

public sealed class IdentitySeedHostedService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<IdentitySeedHostedService> _logger;

    public IdentitySeedHostedService(IServiceProvider serviceProvider, ILogger<IdentitySeedHostedService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await dbContext.Database.EnsureCreatedAsync(cancellationToken);

        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();

        var identityRoles = new[] { RoleKeys.PlatformAdmin, RoleKeys.TenantAdmin, RoleKeys.TenantUser };
        foreach (var key in identityRoles)
        {
            if (!await roleManager.RoleExistsAsync(key))
            {
                await roleManager.CreateAsync(new IdentityRole<Guid>(key));
            }
        }

        if (!await dbContext.RoleDefinitions.AnyAsync(cancellationToken))
        {
            dbContext.RoleDefinitions.Add(RoleDefinition.Create(RoleKeys.PlatformAdmin, "Platform Admin", "platform"));
            dbContext.RoleDefinitions.Add(RoleDefinition.Create(RoleKeys.TenantAdmin, "Tenant Admin", "tenant"));
            dbContext.RoleDefinitions.Add(RoleDefinition.Create(RoleKeys.TenantUser, "Tenant User", "tenant"));
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        var adminEmail = "admin@saas.local";
        var admin = await userManager.FindByEmailAsync(adminEmail);
        if (admin is null)
        {
            admin = new AppUser
            {
                Id = Guid.NewGuid(),
                UserName = adminEmail,
                Email = adminEmail,
                DisplayName = "Platform Admin",
                EmailConfirmed = true
            };

            var create = await userManager.CreateAsync(admin, "Admin!12345");
            if (!create.Succeeded)
            {
                var errors = string.Join(", ", create.Errors.Select(x => x.Description));
                throw new InvalidOperationException($"Failed to create seed admin user: {errors}");
            }
        }

        if (!await userManager.IsInRoleAsync(admin, RoleKeys.PlatformAdmin))
        {
            await userManager.AddToRoleAsync(admin, RoleKeys.PlatformAdmin);
        }

        _logger.LogInformation("Seed complete: admin user {Email}", adminEmail);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
