using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using SaaS.Application.Authorization;
using SaaS.Application.Common;
using SaaS.Application.Contracts;
using SaaS.Application.Identity;
using SaaS.Application.Tenant;
using SaaS.Application.User;
using SaaS.Domain.Modules.Identity;
using SaaS.Infrastructure;
using SaaS.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();
builder.Services.AddSaaSInfrastructure(builder.Configuration);

builder.Services.AddAuthentication();
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("PlatformAdmin", policy => policy.RequireRole(RoleKeys.PlatformAdmin));
});

builder.Services.AddHealthChecks();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation())
    .WithMetrics(metrics => metrics
        .AddAspNetCoreInstrumentation()
        .AddRuntimeInstrumentation());

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseExceptionHandler();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/health/live", () => Results.Ok(new { status = "live" }))
    .AllowAnonymous();

app.MapGet("/health/ready", async (AppDbContext db, CancellationToken cancellationToken) =>
{
    var canConnect = await db.Database.CanConnectAsync(cancellationToken);
    return canConnect
        ? Results.Ok(new { status = "ready" })
        : Results.Problem(title: "Dependency unavailable", detail: "Database connection failed.", statusCode: 503, extensions: Extensions("health.db_unavailable"));
}).AllowAnonymous();

app.MapPost("/api/identity/auth/sign-in", async Task<Results<Ok<SignInResponse>, ProblemHttpResult>> (
    SignInRequest request,
    IIdentityService identityService,
    HttpContext httpContext,
    ILoggerFactory loggerFactory,
    CancellationToken cancellationToken) =>
{
    var result = await identityService.SignInAsync(new SignInIdentityCommand(request.UserNameOrEmail, request.Password), cancellationToken);
    if (!result.Succeeded)
    {
        return result.ToProblem(401, httpContext.TraceIdentifier);
    }

    loggerFactory.CreateLogger("Audit").LogInformation(
        "Audit event: login success user={UserId} traceId={TraceId}",
        result.Value!.UserId,
        httpContext.TraceIdentifier);

    return TypedResults.Ok(result.Value!);
}).AllowAnonymous();

app.MapPost("/api/tenant/tenants", async Task<Results<Ok<TenantResponse>, ProblemHttpResult>> (
    CreateTenantRequest request,
    ClaimsPrincipal user,
    ITenantService tenantService,
    HttpContext httpContext,
    ILoggerFactory loggerFactory,
    CancellationToken cancellationToken) =>
{
    if (!user.IsInRole(RoleKeys.PlatformAdmin))
    {
        return TypedResults.Problem(title: "Forbidden", detail: "Platform admin role is required.", statusCode: 403, extensions: Extensions("authorization.forbidden", httpContext.TraceIdentifier));
    }

    var actorUserId = user.GetUserId();
    if (actorUserId is null)
    {
        return TypedResults.Problem(title: "Unauthorized", detail: "Authenticated user ID not found.", statusCode: 401, extensions: Extensions("identity.missing_user", httpContext.TraceIdentifier));
    }

    var result = await tenantService.CreateTenantAsync(new CreateTenantCommand(request.Code, request.Name, actorUserId.Value), cancellationToken);
    if (!result.Succeeded)
    {
        return result.ToProblem(400, httpContext.TraceIdentifier);
    }

    loggerFactory.CreateLogger("Audit").LogInformation(
        "Audit event: tenant created actor={ActorUserId} tenant={TenantId} traceId={TraceId}",
        actorUserId,
        result.Value!.Id,
        httpContext.TraceIdentifier);

    return TypedResults.Ok(result.Value!);
}).RequireAuthorization("PlatformAdmin");

app.MapPost("/api/user/users", async Task<Results<Ok<InviteUserResponse>, ProblemHttpResult>> (
    InviteUserRequest request,
    ClaimsPrincipal user,
    IAccessControlService accessControlService,
    IUserService userService,
    HttpContext httpContext,
    ILoggerFactory loggerFactory,
    CancellationToken cancellationToken) =>
{
    var actorUserId = user.GetUserId();
    if (actorUserId is null)
    {
        return TypedResults.Problem(title: "Unauthorized", detail: "Authenticated user ID not found.", statusCode: 401, extensions: Extensions("identity.missing_user", httpContext.TraceIdentifier));
    }

    var hasAccess = user.IsInRole(RoleKeys.PlatformAdmin) || await accessControlService.CanManageTenantAsync(actorUserId.Value, request.TenantId, cancellationToken);
    if (!hasAccess)
    {
        return TypedResults.Problem(title: "Forbidden", detail: "Tenant admin or platform admin access required.", statusCode: 403, extensions: Extensions("authorization.forbidden", httpContext.TraceIdentifier));
    }

    var result = await userService.InviteUserAsync(
        new InviteUserCommand(request.TenantId, request.Email, request.DisplayName, request.InitialRoleKey, actorUserId.Value),
        cancellationToken);

    if (!result.Succeeded)
    {
        return result.ToProblem(400, httpContext.TraceIdentifier);
    }

    loggerFactory.CreateLogger("Audit").LogInformation(
        "Audit event: user invited actor={ActorUserId} tenant={TenantId} userProfile={UserProfileId} traceId={TraceId}",
        actorUserId,
        request.TenantId,
        result.Value!.UserProfileId,
        httpContext.TraceIdentifier);

    return TypedResults.Ok(result.Value!);
}).RequireAuthorization();

app.MapPost("/api/user/users/{id:guid}/roles", async Task<Results<Ok<AssignRoleResponse>, ProblemHttpResult>> (
    Guid id,
    AssignRoleRequest request,
    ClaimsPrincipal user,
    IAccessControlService accessControlService,
    IAuthorizationService authorizationService,
    HttpContext httpContext,
    ILoggerFactory loggerFactory,
    CancellationToken cancellationToken) =>
{
    var actorUserId = user.GetUserId();
    if (actorUserId is null)
    {
        return TypedResults.Problem(title: "Unauthorized", detail: "Authenticated user ID not found.", statusCode: 401, extensions: Extensions("identity.missing_user", httpContext.TraceIdentifier));
    }

    var hasAccess = user.IsInRole(RoleKeys.PlatformAdmin) || await accessControlService.CanManageTenantAsync(actorUserId.Value, request.TenantId, cancellationToken);
    if (!hasAccess)
    {
        return TypedResults.Problem(title: "Forbidden", detail: "Tenant admin or platform admin access required.", statusCode: 403, extensions: Extensions("authorization.forbidden", httpContext.TraceIdentifier));
    }

    var result = await authorizationService.AssignRoleAsync(
        new AssignRoleCommand(id, request.TenantId, request.RoleKey, actorUserId.Value),
        cancellationToken);

    if (!result.Succeeded)
    {
        return result.ToProblem(400, httpContext.TraceIdentifier);
    }

    loggerFactory.CreateLogger("Audit").LogInformation(
        "Audit event: role assigned actor={ActorUserId} tenant={TenantId} userProfile={UserProfileId} role={RoleKey} traceId={TraceId}",
        actorUserId,
        request.TenantId,
        id,
        request.RoleKey,
        httpContext.TraceIdentifier);

    return TypedResults.Ok(result.Value!);
}).RequireAuthorization();

app.Run();

public partial class Program { }

internal static class ApiExtensions
{
    public static ProblemHttpResult ToProblem<T>(
        this ApplicationResult<T> result,
        int defaultStatusCode,
        string traceId)
    {
        return TypedResults.Problem(
            title: "Request failed",
            detail: result.Error?.Message ?? "The request failed.",
            statusCode: defaultStatusCode,
            extensions: Extensions(result.Error?.Code ?? "request.failed", traceId));
    }

    public static Guid? GetUserId(this ClaimsPrincipal principal)
    {
        var idValue = principal.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(idValue, out var userId) ? userId : null;
    }

    public static Dictionary<string, object?> Extensions(string errorCode, string? traceId = null) =>
        new()
        {
            ["errorCode"] = errorCode,
            ["traceId"] = traceId
        };
}

internal sealed record SignInRequest(string UserNameOrEmail, string Password);
internal sealed record CreateTenantRequest(string Code, string Name);
internal sealed record InviteUserRequest(Guid TenantId, string Email, string DisplayName, string InitialRoleKey);
internal sealed record AssignRoleRequest(Guid TenantId, string RoleKey);
