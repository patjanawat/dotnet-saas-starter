using SaaS.Domain.Modules.Tenant;
using SaaS.Domain.Modules.User;

namespace SaaS.UnitTests;

public sealed class DomainRulesTests
{
    [Fact]
    public void Tenant_Create_SetsPendingStatus()
    {
        var tenant = Tenant.Create("acme", "Acme", DateTime.UtcNow);

        Assert.Equal(TenantStatus.Pending, tenant.Status);
        Assert.Equal("acme", tenant.Code);
    }

    [Fact]
    public void UserProfile_Invite_SetsInvitedStatus()
    {
        var profile = UserProfile.Invite(Guid.NewGuid(), Guid.NewGuid(), "user@acme.test", "User", DateTime.UtcNow);

        Assert.Equal(UserStatus.Invited, profile.Status);
        Assert.Equal("user@acme.test", profile.Email);
    }

    [Fact]
    public void UserMembership_Create_RequiresRole()
    {
        Assert.Throws<ArgumentException>(() =>
            UserMembership.Create(Guid.NewGuid(), Guid.NewGuid(), string.Empty, DateTime.UtcNow));
    }
}
