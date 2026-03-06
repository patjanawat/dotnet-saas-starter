using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SaaS.Domain.Modules.Authorization;
using SaaS.Domain.Modules.Identity;
using SaaS.Domain.Modules.Tenant;
using SaaS.Domain.Modules.User;
using SaaS.Infrastructure.Identity;

namespace SaaS.Infrastructure.Persistence;

public sealed class AppDbContext : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<TenantConfiguration> TenantConfigurations => Set<TenantConfiguration>();
    public DbSet<UserProfile> UserProfiles => Set<UserProfile>();
    public DbSet<UserMembership> UserMemberships => Set<UserMembership>();
    public DbSet<RoleDefinition> RoleDefinitions => Set<RoleDefinition>();
    public DbSet<IdentitySessionRecord> IdentitySessionRecords => Set<IdentitySessionRecord>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Tenant>(entity =>
        {
            entity.ToTable("tenants");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Code).HasMaxLength(100).IsRequired();
            entity.Property(x => x.Name).HasMaxLength(250).IsRequired();
            entity.HasIndex(x => x.Code).IsUnique();
        });

        builder.Entity<TenantConfiguration>(entity =>
        {
            entity.ToTable("tenant_configurations");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Key).HasMaxLength(120).IsRequired();
            entity.Property(x => x.Value).HasMaxLength(4000).IsRequired();
            entity.HasIndex(x => new { x.TenantId, x.Key }).IsUnique();
        });

        builder.Entity<UserProfile>(entity =>
        {
            entity.ToTable("users");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Email).HasMaxLength(320).IsRequired();
            entity.Property(x => x.DisplayName).HasMaxLength(250).IsRequired();
            entity.HasIndex(x => new { x.TenantId, x.Email }).IsUnique();
        });

        builder.Entity<UserMembership>(entity =>
        {
            entity.ToTable("user_memberships");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.RoleKey).HasMaxLength(100).IsRequired();
            entity.HasIndex(x => new { x.UserProfileId, x.TenantId, x.RoleKey }).IsUnique();
        });

        builder.Entity<RoleDefinition>(entity =>
        {
            entity.ToTable("roles");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Key).HasMaxLength(100).IsRequired();
            entity.Property(x => x.Name).HasMaxLength(200).IsRequired();
            entity.Property(x => x.Scope).HasMaxLength(100).IsRequired();
            entity.HasIndex(x => x.Key).IsUnique();
        });

        builder.Entity<IdentitySessionRecord>(entity =>
        {
            entity.ToTable("identity_session_records");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.SessionId).HasMaxLength(120).IsRequired();
            entity.HasIndex(x => x.SessionId).IsUnique();
            entity.HasIndex(x => x.UserId);
        });
    }
}
