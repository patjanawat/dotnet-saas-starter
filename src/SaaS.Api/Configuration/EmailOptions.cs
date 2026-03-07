using System.ComponentModel.DataAnnotations;

namespace SaaS.Api.Configuration;

public sealed class EmailOptions
{
    public const string SectionName = "Email";

    [Required]
    public string Provider { get; set; } = "Disabled";

    [Required]
    [EmailAddress]
    public string FromAddress { get; set; } = "noreply@example.local";

    [Required]
    public string FromName { get; set; } = "SaaS Starter";

    [Required]
    public string Host { get; set; } = "localhost";

    [Range(1, 65535)]
    public int Port { get; set; } = 2525;

    public bool UseSsl { get; set; }

    [Required]
    public string Username { get; set; } = "placeholder-user";

    [Required]
    public string Password { get; set; } = "placeholder-password";
}
