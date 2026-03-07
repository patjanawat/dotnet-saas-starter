using SaaS.Application.Common;

namespace SaaS.Api.Baseline;

public static class ExceptionMapping
{
    public static ErrorModel Map(Exception exception) =>
        exception switch
        {
            AppException appException => new ErrorModel(
                appException.Code,
                appException.Message,
                appException.StatusCode),
            BadHttpRequestException => new ErrorModel(
                "request.bad_request",
                "The request is invalid.",
                StatusCodes.Status400BadRequest),
            _ => new ErrorModel(
                "system.unhandled_exception",
                "An unexpected error occurred.",
                StatusCodes.Status500InternalServerError)
        };
}
