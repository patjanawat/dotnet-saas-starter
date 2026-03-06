using Microsoft.AspNetCore.Http.HttpResults;
using SaaS.Application.Common;

namespace SaaS.Api.Baseline;

public static class ProblemDetailsMapper
{
    public static ProblemHttpResult ToProblem(this ErrorModel error, string traceId) =>
        TypedResults.Problem(
            title: "Request failed",
            detail: error.Message,
            statusCode: error.StatusCode,
            extensions: CreateExtensions(error.Code, traceId));

    public static ProblemHttpResult ToProblem(this Exception exception, string traceId) =>
        TypedResults.Problem(
            title: "Unhandled server error",
            detail: "An unexpected error occurred.",
            statusCode: 500,
            extensions: CreateExtensions("system.unhandled_exception", traceId, exception.GetType().Name));

    private static Dictionary<string, object?> CreateExtensions(string errorCode, string traceId, string? exceptionType = null)
    {
        var extensions = new Dictionary<string, object?>
        {
            ["errorCode"] = errorCode,
            ["traceId"] = traceId
        };

        if (!string.IsNullOrWhiteSpace(exceptionType))
        {
            extensions["exceptionType"] = exceptionType;
        }

        return extensions;
    }
}
