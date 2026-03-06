using System.Security.Claims;

namespace SaaS.Api.Middleware;

public sealed class RequestContextLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestContextLoggingMiddleware> _logger;

    public RequestContextLoggingMiddleware(RequestDelegate next, ILogger<RequestContextLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        context.Request.Headers.TryGetValue("X-Tenant-Id", out var tenantId);

        using var scope = _logger.BeginScope(new Dictionary<string, object?>
        {
            ["TraceId"] = context.TraceIdentifier,
            ["UserId"] = userId,
            ["TenantId"] = tenantId.ToString(),
            ["Method"] = context.Request.Method,
            ["Path"] = context.Request.Path.Value
        });

        await _next(context);
    }
}
