using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace SaaS.Api.Telemetry;

public static class TelemetryExtensions
{
    public static IServiceCollection AddSaaSTelemetry(this IServiceCollection services)
    {
        services.AddOpenTelemetry()
            .WithTracing(tracing => tracing
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation())
            .WithMetrics(metrics => metrics
                .AddAspNetCoreInstrumentation()
                .AddRuntimeInstrumentation());

        return services;
    }
}
