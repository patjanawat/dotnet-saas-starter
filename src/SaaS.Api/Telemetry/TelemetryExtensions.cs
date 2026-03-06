using OpenTelemetry.Resources;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace SaaS.Api.Telemetry;

public static class TelemetryExtensions
{
    public static IServiceCollection AddSaaSTelemetry(this IServiceCollection services)
    {
        services.AddOpenTelemetry()
            .ConfigureResource(resource => resource.AddService("SaaS.Api"))
            .WithTracing(tracing =>
            {
                tracing
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddConsoleExporter();
            })
            .WithMetrics(metrics =>
            {
                metrics
                .AddAspNetCoreInstrumentation()
                .AddRuntimeInstrumentation()
                .AddConsoleExporter();
            });

        return services;
    }
}
