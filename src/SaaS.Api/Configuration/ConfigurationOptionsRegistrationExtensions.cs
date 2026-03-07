using Microsoft.Extensions.Options;

namespace SaaS.Api.Configuration;

public static class ConfigurationOptionsRegistrationExtensions
{
    public static IServiceCollection AddSaaSConfigurationOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<JwtOptions>()
            .Bind(configuration.GetSection(JwtOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddOptions<EmailOptions>()
            .Bind(configuration.GetSection(EmailOptions.SectionName))
            .ValidateDataAnnotations()
            .Validate(options => options.Provider.Equals("Disabled", StringComparison.OrdinalIgnoreCase) || options.Port > 0, "Email port must be greater than 0.")
            .ValidateOnStart();

        return services;
    }
}
