using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;
using SaaS.Api.Authorization;
using SaaS.Api.Baseline;
using SaaS.Application.Authorization;
using SaaS.Application.Common;
using SaaS.Application.Contracts;
using SaaS.Domain.Modules.Identity;

namespace SaaS.Api.Endpoints;

public static class AuthorizationEndpoints
{
    public static IEndpointRouteBuilder MapAuthorizationEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/api/user/users/{id:guid}/roles", async Task<Results<Ok<AssignRoleResponseModel>, ProblemHttpResult>> (
            Guid id,
            AssignRoleRequest request,
            ClaimsPrincipal user,
            IAccessControlService accessControlService,
            IAssignRoleCommandHandler handler,
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

            var canAssignRole = user.IsInRole(RoleKeys.PlatformAdmin)
                || await accessControlService.CanManageTenantAsync(actorUserId.Value, request.TenantId, cancellationToken);

            if (!canAssignRole)
            {
                return new ErrorModel("authorization.forbidden", "Tenant admin or platform admin access required.", 403)
                    .ToProblem(httpContext.TraceIdentifier);
            }

            var result = await handler.HandleAsync(
                new AssignRoleCommand(id, request.TenantId, request.RoleKey, actorUserId.Value),
                cancellationToken);

            if (!result.Succeeded)
            {
                return result.Error!.ToProblem(httpContext.TraceIdentifier);
            }

            var value = result.Value!;
            loggerFactory.CreateLogger("Audit").LogInformation(
                "Audit event: role assignment actor={ActorUserId} tenant={TenantId} userProfile={UserProfileId} role={RoleKey} traceId={TraceId}",
                actorUserId,
                value.TenantId,
                value.UserProfileId,
                value.RoleKey,
                httpContext.TraceIdentifier);

            return TypedResults.Ok(new AssignRoleResponseModel(value.UserProfileId, value.TenantId, value.RoleKey));
        }).RequireAuthorization("RoleAssignmentPolicy");

        return endpoints;
    }

    private static Guid? GetUserId(ClaimsPrincipal principal)
    {
        var idValue = principal.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(idValue, out var userId) ? userId : null;
    }
}
