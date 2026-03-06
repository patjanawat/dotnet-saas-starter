using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;
using SaaS.Api.Baseline;
using SaaS.Api.Swagger;
using SaaS.Api.UserInvitation;
using SaaS.Application.Common;
using SaaS.Application.Contracts;
using SaaS.Application.User;
using SaaS.Domain.Modules.Identity;

namespace SaaS.Api.Endpoints;

public static class UserInvitationEndpoints
{
    public static IEndpointRouteBuilder MapUserInvitationEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/api/user/users", async Task<Results<Ok<InviteUserResponseModel>, ProblemHttpResult>> (
            InviteUserRequest request,
            ClaimsPrincipal user,
            IAccessControlService accessControlService,
            IInviteUserCommandHandler handler,
            HttpContext httpContext,
            ILoggerFactory loggerFactory,
            CancellationToken cancellationToken) =>
        {
            var actorUserId = GetUserId(user);
            if (actorUserId is null)
            {
                return new ErrorModel("identity.missing_user", "Authenticated user ID not found.", 401)
                    .ToProblem(httpContext.TraceIdentifier);
            }

            var canManageTenant = user.IsInRole(RoleKeys.PlatformAdmin)
                || await accessControlService.CanManageTenantAsync(actorUserId.Value, request.TenantId, cancellationToken);

            if (!canManageTenant)
            {
                return new ErrorModel("authorization.forbidden", "Tenant admin or platform admin access required.", 403)
                    .ToProblem(httpContext.TraceIdentifier);
            }

            var result = await handler.HandleAsync(
                new InviteUserCommand(request.TenantId, request.Email, request.DisplayName, actorUserId.Value),
                cancellationToken);

            if (!result.Succeeded)
            {
                return result.Error!.ToProblem(httpContext.TraceIdentifier);
            }

            var value = result.Value!;
            loggerFactory.CreateLogger("Audit").LogInformation(
                "Audit event: user invitation actor={ActorUserId} tenant={TenantId} userProfile={UserProfileId} traceId={TraceId}",
                actorUserId,
                value.TenantId,
                value.UserProfileId,
                httpContext.TraceIdentifier);

            return TypedResults.Ok(new InviteUserResponseModel(value.UserProfileId, value.TenantId, value.Email, value.Status));
        })
        .WithTags("Users")
        .WithGroupName("v1")
        .WithSummary("Invite user to tenant")
        .WithDescription("Invites a user to a tenant when caller has tenant admin or platform admin access.")
        .Produces<InviteUserResponseModel>(StatusCodes.Status200OK)
        .WithStandardProblemResponses(includeUnauthorized: true, includeForbidden: true, includeValidation: true, includeConflict: true)
        .RequireAuthorization();

        return endpoints;
    }

    private static Guid? GetUserId(ClaimsPrincipal principal)
    {
        var idValue = principal.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(idValue, out var userId) ? userId : null;
    }
}
