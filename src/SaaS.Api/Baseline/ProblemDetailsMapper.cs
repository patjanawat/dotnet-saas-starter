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

    private static Dictionary<string, object?> CreateExtensions(string errorCode, string traceId)
    {
        return new Dictionary<string, object?>
        {
            ["errorCode"] = errorCode,
            ["traceId"] = traceId
        };
    }
}
