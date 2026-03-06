using Microsoft.AspNetCore.Http.HttpResults;
using SaaS.Api.Baseline;
using SaaS.Api.Tenant;
using SaaS.Application.Common;
using SaaS.Application.Tenant;
using SaaS.Domain.Modules.Identity;
using System.Security.Claims;

namespace SaaS.Api.Endpoints;

public static class TenantEndpoints
{
    public static IEndpointRouteBuilder MapTenantEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/api/tenant/tenants", async Task<Results<Ok<TenantResponseModel>, ProblemHttpResult>> (
            CreateTenantRequest request,
            ClaimsPrincipal user,
            ICreateTenantCommandHandler handler,
            HttpContext httpContext,
            CancellationToken cancellationToken) =>
        {
            if (!user.IsInRole(RoleKeys.PlatformAdmin))
            {
                return new ErrorModel("authorization.forbidden", "Platform admin role is required.", 403)
                    .ToProblem(httpContext.TraceIdentifier);
            }

            var result = await handler.HandleAsync(new CreateTenantCommand(request.Code, request.Name, Guid.Empty), cancellationToken);
            if (!result.Succeeded)
            {
                return result.Error!.ToProblem(httpContext.TraceIdentifier);
            }

            var tenant = result.Value!;
            return TypedResults.Ok(new TenantResponseModel(tenant.Id, tenant.Code, tenant.Name, tenant.Status));
        }).RequireAuthorization();

        endpoints.MapGet("/api/tenant/tenants/{id:guid}", async Task<Results<Ok<TenantResponseModel>, ProblemHttpResult>> (
            Guid id,
            ClaimsPrincipal user,
            IGetTenantByIdQueryHandler handler,
            HttpContext httpContext,
            CancellationToken cancellationToken) =>
        {
            if (!user.IsInRole(RoleKeys.PlatformAdmin))
            {
                return new ErrorModel("authorization.forbidden", "Platform admin role is required.", 403)
                    .ToProblem(httpContext.TraceIdentifier);
            }

            var result = await handler.HandleAsync(new GetTenantByIdQuery(id), cancellationToken);
            if (!result.Succeeded)
            {
                return result.Error!.ToProblem(httpContext.TraceIdentifier);
            }

            var tenant = result.Value!;
            return TypedResults.Ok(new TenantResponseModel(tenant.Id, tenant.Code, tenant.Name, tenant.Status));
        }).RequireAuthorization();

        return endpoints;
    }
}
