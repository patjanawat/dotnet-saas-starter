using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace SaaS.Api.Baseline;

public sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;
    private readonly IProblemDetailsService _problemDetailsService;
    private readonly IHostEnvironment _environment;

    public GlobalExceptionHandler(
        ILogger<GlobalExceptionHandler> logger,
        IProblemDetailsService problemDetailsService,
        IHostEnvironment environment)
    {
        _logger = logger;
        _problemDetailsService = problemDetailsService;
        _environment = environment;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var traceId = Activity.Current?.Id ?? httpContext.TraceIdentifier;
        var (statusCode, type, title, detail, errorCode) = MapException(exception, _environment.IsDevelopment());

        if (statusCode >= StatusCodes.Status500InternalServerError)
        {
            _logger.LogError(
                exception,
                "Unhandled exception for {RequestMethod} {RequestPath}. TraceId={TraceId}",
                httpContext.Request.Method,
                httpContext.Request.Path,
                traceId);
        }
        else
        {
            _logger.LogWarning(
                exception,
                "Request failed for {RequestMethod} {RequestPath}. TraceId={TraceId}",
                httpContext.Request.Method,
                httpContext.Request.Path,
                traceId);
        }

        var problemDetails = new ProblemDetails
        {
            Type = type,
            Title = title,
            Status = statusCode,
            Detail = detail,
            Instance = httpContext.Request.Path
        };

        problemDetails.Extensions["traceId"] = traceId;
        problemDetails.Extensions["errorCode"] = errorCode;

        if (_environment.IsDevelopment())
        {
            problemDetails.Extensions["exception"] = exception.GetType().Name;
        }

        httpContext.Response.StatusCode = statusCode;

        var handled = await _problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = problemDetails,
            Exception = exception
        });

        if (!handled)
        {
            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
        }

        return true;
    }

    private static (int StatusCode, string Type, string Title, string Detail, string ErrorCode) MapException(Exception exception, bool isDevelopment)
    {
        return exception switch
        {
            BadHttpRequestException => (
                StatusCodes.Status400BadRequest,
                "https://httpstatuses.com/400",
                "Bad Request",
                "The request is invalid.",
                "request.bad_request"),
            _ => (
                StatusCodes.Status500InternalServerError,
                "https://httpstatuses.com/500",
                "Internal Server Error",
                isDevelopment
                    ? exception.Message
                    : "An unexpected error occurred.",
                "system.unhandled_exception")
        };
    }
}
