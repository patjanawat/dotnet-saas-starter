using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SaaS.Api.Swagger;

public static class SwaggerExtensions
{
    public static IServiceCollection AddSaaSSwagger(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "SaaS Starter API",
                Version = "v1",
                Description = "Production-grade SaaS API contract for platform and module endpoints."
            });

            options.DocInclusionPredicate(static (docName, _) => string.Equals(docName, "v1", StringComparison.OrdinalIgnoreCase));

            var bearerScheme = new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Description = "Bearer Authentication using JWT. Example: 'Bearer {token}'",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT"
            };

            options.AddSecurityDefinition("Bearer", bearerScheme);
            options.OperationFilter<AuthorizeOperationFilter>();

            IncludeXmlCommentsIfPresent(options, Assembly.GetExecutingAssembly());
        });

        return services;
    }

    public static IApplicationBuilder UseSaaSSwagger(this IApplicationBuilder app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.RoutePrefix = "swagger";
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "SaaS Starter API v1");
            options.DocumentTitle = "SaaS Starter API Docs";
        });

        return app;
    }

    private static void IncludeXmlCommentsIfPresent(SwaggerGenOptions options, Assembly assembly)
    {
        var xmlFileName = $"{assembly.GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFileName);
        if (File.Exists(xmlPath))
        {
            options.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
        }
    }
}

public sealed class AuthorizeOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var endpointMetadata = context.ApiDescription.ActionDescriptor.EndpointMetadata ?? [];
        var hasAllowAnonymous = endpointMetadata.OfType<IAllowAnonymous>().Any();
        var hasAuthorize = endpointMetadata.OfType<IAuthorizeData>().Any();

        if (hasAllowAnonymous || !hasAuthorize)
        {
            return;
        }

        operation.Security ??= new List<OpenApiSecurityRequirement>();
        operation.Security.Add(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecuritySchemeReference("Bearer", null, null),
                new List<string>()
            }
        });

        operation.Responses ??= new OpenApiResponses();
        operation.Responses.TryAdd("401", new OpenApiResponse { Description = "Unauthorized" });
        operation.Responses.TryAdd("403", new OpenApiResponse { Description = "Forbidden" });
    }
}

public static class EndpointMetadataExtensions
{
    public static RouteHandlerBuilder WithStandardProblemResponses(
        this RouteHandlerBuilder builder,
        bool includeValidation = false,
        bool includeUnauthorized = false,
        bool includeForbidden = false,
        bool includeNotFound = false,
        bool includeConflict = false)
    {
        if (includeValidation)
        {
            builder.Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest, "application/problem+json");
        }
        else
        {
            builder.Produces<ProblemDetails>(StatusCodes.Status400BadRequest, "application/problem+json");
        }

        if (includeUnauthorized)
        {
            builder.Produces<ProblemDetails>(StatusCodes.Status401Unauthorized, "application/problem+json");
        }

        if (includeForbidden)
        {
            builder.Produces<ProblemDetails>(StatusCodes.Status403Forbidden, "application/problem+json");
        }

        if (includeNotFound)
        {
            builder.Produces<ProblemDetails>(StatusCodes.Status404NotFound, "application/problem+json");
        }

        if (includeConflict)
        {
            builder.Produces<ProblemDetails>(StatusCodes.Status409Conflict, "application/problem+json");
        }

        builder.Produces<ProblemDetails>(StatusCodes.Status500InternalServerError, "application/problem+json");

        return builder;
    }
}
