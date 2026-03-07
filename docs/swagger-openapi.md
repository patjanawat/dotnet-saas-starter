# Swagger / OpenAPI Notes

## Purpose
- Expose a machine-readable OpenAPI contract and interactive Swagger UI for API exploration.
- Keep production exposure controlled by environment policy.

## Routes
- Swagger UI: `/swagger`
- OpenAPI JSON: `/swagger/v1/swagger.json`

## Environment Policy
- Swagger is enabled by default in `Development`.
- For non-development environments, enable explicitly with configuration:

```json
{
  "Swagger": {
    "Enabled": true
  }
}
```

- Keep `Swagger:Enabled` unset or `false` in production unless there is an approved operational need.

## Metadata Conventions
- Endpoints should include tags, summary, description, and response metadata.
- Protected endpoints should include auth metadata and standard problem response metadata.
- Use `ProblemDetails` and `ValidationProblemDetails` for error contracts.

## XML Comments
- XML documentation is enabled for `SaaS.Api`.
- Swagger includes XML comments when the generated XML file is present at runtime.
- If `CS1591` warnings appear during build, it means public API models/endpoints still need XML comments (`/// <summary>...`).
- Prioritize adding comments on:
  - request/response contracts
  - endpoint mapping extensions
  - public middleware and security abstractions exposed from `SaaS.Api`
