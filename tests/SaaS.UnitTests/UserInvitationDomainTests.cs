using SaaS.Domain.Modules.Identity;
using SaaS.Domain.Modules.User;

namespace SaaS.UnitTests;

public sealed class UserInvitationDomainTests
{
    [Fact]
    public void UserProfile_Invite_CreatesInvitedUser()
    {
        var profile = UserProfile.Invite(Guid.NewGuid(), Guid.NewGuid(), "invited@tenant.test", "Invited User", DateTime.UtcNow);

        Assert.Equal(UserStatus.Invited, profile.Status);
        Assert.Equal("invited@tenant.test", profile.Email);
    }

    [Fact]
    public void UserMembership_Create_UsesTenantUserRole()
    {
        var membership = UserMembership.Create(Guid.NewGuid(), Guid.NewGuid(), RoleKeys.TenantUser, DateTime.UtcNow);

        Assert.Equal(RoleKeys.TenantUser, membership.RoleKey);
        Assert.Equal(UserStatus.Active, membership.Status);
    }
}
