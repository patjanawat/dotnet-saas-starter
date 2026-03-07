using Microsoft.AspNetCore.Diagnostics;
using SaaS.Application.Common;

namespace SaaS.Api.Baseline;

public sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var error = ExceptionMapping.Map(exception);

        if (error.StatusCode >= 500)
        {
            _logger.LogError(exception, "Unhandled exception captured. TraceId={TraceId}", httpContext.TraceIdentifier);
        }
        else
        {
            _logger.LogWarning(exception, "Handled exception mapped. TraceId={TraceId}", httpContext.TraceIdentifier);
        }

        var problem = error.ToProblem(httpContext.TraceIdentifier);
        await problem.ExecuteAsync(httpContext);
        return true;
    }
}
