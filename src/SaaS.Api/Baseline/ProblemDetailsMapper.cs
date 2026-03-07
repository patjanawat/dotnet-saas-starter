using Microsoft.AspNetCore.Http.HttpResults;
using SaaS.Application.Common;

namespace SaaS.Api.Baseline;

public static class ProblemDetailsMapper
{
    public static ProblemHttpResult ToProblem(this ErrorModel error, string traceId) =>
        TypedResults.Problem(
            type: $"https://httpstatuses.com/{error.StatusCode}",
            title: error.StatusCode >= StatusCodes.Status500InternalServerError
                ? "Internal Server Error"
                : "Request Failed",
            detail: error.Message,
            statusCode: error.StatusCode,
            extensions: new Dictionary<string, object?>
            {
                ["errorCode"] = error.Code,
                ["traceId"] = traceId
            });
}
