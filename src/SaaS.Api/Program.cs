using SaaS.Api.Baseline;
using SaaS.Api.Endpoints;
using SaaS.Api.Middleware;
using SaaS.Api.Security;
using SaaS.Api.Swagger;
using SaaS.Api.Telemetry;
using SaaS.Application.Contracts;
using SaaS.Infrastructure;
using Serilog;
using Serilog.Events;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

try
{
    Log.Information("Starting SaaS.Api host");

    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((context, services, loggerConfiguration) =>
    {
        loggerConfiguration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext();
    });

    builder.Services.AddSaaSSwagger();
    builder.Services.AddProblemDetails(options =>
    {
        options.CustomizeProblemDetails = context =>
        {
            context.ProblemDetails.Extensions.TryAdd("traceId", context.HttpContext.TraceIdentifier);
        };
    });
    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
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

    var app = builder.Build();

    if (app.Environment.IsDevelopment() || app.Configuration.GetValue<bool>("Swagger:Enabled"))
    {
        app.UseSaaSSwagger();
    }

    app.UseExceptionHandler();
    app.UseMiddleware<RequestContextLoggingMiddleware>();
    app.UseSerilogRequestLogging(options =>
    {
        options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
        options.GetLevel = (httpContext, elapsed, ex) =>
        {
            if (ex is not null || httpContext.Response.StatusCode >= 500)
            {
                return LogEventLevel.Error;
            }

            if (elapsed > 1000)
            {
                return LogEventLevel.Warning;
            }

            return LogEventLevel.Information;
        };

        options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
        {
            diagnosticContext.Set("TraceId", httpContext.TraceIdentifier);
            diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
            diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
        };
    });

    app.UseHttpsRedirection();
    app.UseAuthentication();
    app.UseAuthorization();

    app.MapFoundationHealthEndpoints();
    app.MapLoginEndpoints();
    app.MapTenantEndpoints();
    app.MapUserInvitationEndpoints();
    app.MapAuthorizationEndpoints();

    // Foundation probe endpoints used for baseline validation only.
    app.MapGet("/api/foundation/ping", () => Results.Ok(new { status = "ok" })).AllowAnonymous().ExcludeFromDescription();
    app.MapGet("/api/foundation/throw", (HttpContext _) => throw new InvalidOperationException("Simulated failure")).AllowAnonymous().ExcludeFromDescription();

    Log.Information("SaaS.Api host configured. Starting request loop");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "SaaS.Api host terminated unexpectedly");
    throw;
}
finally
{
    Log.CloseAndFlush();
}

public partial class Program { }
