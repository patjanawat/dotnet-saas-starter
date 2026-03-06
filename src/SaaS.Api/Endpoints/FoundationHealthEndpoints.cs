using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using SaaS.Infrastructure.Persistence;

namespace SaaS.Api.Endpoints;

public static class FoundationHealthEndpoints
{
    public static IEndpointRouteBuilder MapFoundationHealthEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/health/live", () => Results.Ok(new { status = "live" }))
            .WithTags("Health")
            .WithGroupName("v1")
            .WithSummary("Liveness probe")
            .WithDescription("Returns process liveness for container and orchestrator probes.")
            .Produces(StatusCodes.Status200OK)
            .AllowAnonymous();

        endpoints.MapGet("/health/ready", async (HttpContext httpContext, AppDbContext db, CancellationToken cancellationToken) =>
        {
            var canConnect = await db.Database.CanConnectAsync(cancellationToken);
            return canConnect
                ? Results.Ok(new { status = "ready" })
                : Results.Problem(
                    title: "Dependency unavailable",
                    detail: "Database connection failed.",
                    statusCode: 503,
                    extensions: new Dictionary<string, object?>
                    {
                        ["errorCode"] = "health.db_unavailable",
                        ["traceId"] = httpContext.TraceIdentifier
                    });
        })
        .WithTags("Health")
        .WithGroupName("v1")
        .WithSummary("Readiness probe")
        .WithDescription("Returns readiness state based on critical dependency connectivity.")
        .Produces(StatusCodes.Status200OK)
        .Produces<ProblemDetails>(StatusCodes.Status503ServiceUnavailable, "application/problem+json")
        .AllowAnonymous();

        return endpoints;
    }
}
