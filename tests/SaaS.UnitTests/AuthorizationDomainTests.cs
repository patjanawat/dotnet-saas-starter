using SaaS.Domain.Modules.Authorization;

namespace SaaS.UnitTests;

public sealed class AuthorizationDomainTests
{
    [Fact]
    public void RoleDefinition_Create_NormalizesKeyAndScope()
    {
        var role = RoleDefinition.Create(" Tenant_Admin ", "Tenant Admin", " Tenant ");

        Assert.Equal("tenant_admin", role.Key);
        Assert.Equal("tenant", role.Scope);
    }

    [Fact]
    public void RoleDefinition_Create_ThrowsWhenKeyMissing()
    {
        Assert.Throws<ArgumentException>(() => RoleDefinition.Create("", "No Key", "tenant"));
    }
}
