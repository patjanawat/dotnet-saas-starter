using Microsoft.AspNetCore.Http.HttpResults;
using SaaS.Api.Baseline;
using SaaS.Api.Identity;
using SaaS.Api.Swagger;
using SaaS.Application.Identity;

namespace SaaS.Api.Endpoints;

public static class LoginEndpoints
{
    public static IEndpointRouteBuilder MapLoginEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/api/identity/auth/sign-in", async Task<Results<Ok<LoginResponse>, ProblemHttpResult>> (
            LoginRequest request,
            ILoginCommandHandler handler,
            HttpContext httpContext,
            ILoggerFactory loggerFactory,
            CancellationToken cancellationToken) =>
        {
            var result = await handler.HandleAsync(new SignInIdentityCommand(request.UserNameOrEmail, request.Password), cancellationToken);
            if (!result.Succeeded)
            {
                return result.Error!.ToProblem(httpContext.TraceIdentifier);
            }

            var value = result.Value!;
            loggerFactory.CreateLogger("Audit").LogInformation(
                "Audit event: login success user={UserId} traceId={TraceId}",
                value.UserId,
                httpContext.TraceIdentifier);

            return TypedResults.Ok(new LoginResponse(value.UserId, value.Email, value.DisplayName));
        })
        .WithTags("Auth")
        .WithGroupName("v1")
        .WithSummary("Sign in user")
        .WithDescription("Authenticates a user and returns a login contract for the active session.")
        .Produces<LoginResponse>(StatusCodes.Status200OK)
        .WithStandardProblemResponses(includeUnauthorized: true)
        .AllowAnonymous();

        return endpoints;
    }
}
