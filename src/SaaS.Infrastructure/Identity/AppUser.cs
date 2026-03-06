using Microsoft.AspNetCore.Identity;

namespace SaaS.Infrastructure.Identity;

public sealed class AppUser : IdentityUser<Guid>
{
    public string DisplayName { get; set; } = string.Empty;
}
