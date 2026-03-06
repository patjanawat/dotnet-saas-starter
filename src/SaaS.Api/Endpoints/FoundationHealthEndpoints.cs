using Microsoft.EntityFrameworkCore;
using SaaS.Infrastructure.Persistence;

namespace SaaS.Api.Endpoints;

public static class FoundationHealthEndpoints
{
    public static IEndpointRouteBuilder MapFoundationHealthEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/health/live", () => Results.Ok(new { status = "live" }))
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
        }).AllowAnonymous();

        return endpoints;
    }
}
