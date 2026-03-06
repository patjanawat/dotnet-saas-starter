using SaaS.Api.Endpoints;
using SaaS.Api.Middleware;
using SaaS.Api.Security;
using SaaS.Api.Telemetry;
using SaaS.Application.Contracts;
using SaaS.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();
builder.Services.AddSaaSInfrastructure(builder.Configuration);
builder.Services.AddSaaSTelemetry();
builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<ICurrentUser, HttpContextCurrentUser>();
builder.Services.AddScoped<ICurrentTenant, HttpContextCurrentTenant>();

builder.Services.AddAuthentication();
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RoleAssignmentPolicy", policy => policy.RequireAuthenticatedUser());
});
builder.Services.AddHealthChecks();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseMiddleware<RequestContextLoggingMiddleware>();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapFoundationHealthEndpoints();
app.MapLoginEndpoints();
app.MapTenantEndpoints();
app.MapUserInvitationEndpoints();
app.MapAuthorizationEndpoints();

// Foundation probe endpoints used for baseline validation only.
app.MapGet("/api/foundation/ping", () => Results.Ok(new { status = "ok" })).AllowAnonymous();
app.MapGet("/api/foundation/throw", (HttpContext _) => throw new InvalidOperationException("Simulated failure")).AllowAnonymous();

app.Run();

public partial class Program { }
