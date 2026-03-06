using SaaS.Domain.Modules.Tenant;

namespace SaaS.UnitTests;

public sealed class TenantDomainTests
{
    [Theory]
    [InlineData("acme", "acme")]
    [InlineData("Acme", "acme")]
    [InlineData("acme-001", "acme-001")]
    [InlineData("tenant-abc-123", "tenant-abc-123")]
    public void TenantSlug_TryNormalize_ReturnsTrue_ForValidSlugs(string slug, string expectedNormalized)
    {
        var ok = TenantSlug.TryNormalize(slug, out var normalized);

        Assert.True(ok);
        Assert.Equal(expectedNormalized, normalized);
    }

    [Theory]
    [InlineData("acme_1")]
    [InlineData("-acme")]
    [InlineData("acme-")]
    [InlineData("acme--x")]
    [InlineData("")]
    public void TenantSlug_TryNormalize_ReturnsFalse_ForInvalidSlugs(string slug)
    {
        var ok = TenantSlug.TryNormalize(slug, out _);

        Assert.False(ok);
    }

    [Fact]
    public void Tenant_Create_Throws_ForInvalidSlug()
    {
        Assert.Throws<ArgumentException>(() => Tenant.Create("Invalid_Slug", "Acme", DateTime.UtcNow));
    }
}
