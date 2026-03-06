using SaaS.Api.Baseline;
using SaaS.Application.Common;

namespace SaaS.Api.Middleware;

public sealed class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception captured. TraceId={TraceId}", context.TraceIdentifier);
            var problem = ex is AppException appException
                ? new ErrorModel(appException.Code, appException.Message, appException.StatusCode).ToProblem(context.TraceIdentifier)
                : ex.ToProblem(context.TraceIdentifier);
            await problem.ExecuteAsync(context);
        }
    }
}
