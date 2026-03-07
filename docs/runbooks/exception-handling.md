# Phase 2 - Step 2.17 - Global Exception Handling

## Overview

Global exception handling is implemented using ASP.NET Core's recommended `IExceptionHandler` approach and `ProblemDetails`.
Unhandled exceptions are captured in one place and returned as consistent `application/problem+json` responses.

## How It Works

1. `builder.Services.AddProblemDetails(...)` registers ProblemDetails support.
2. `builder.Services.AddExceptionHandler<GlobalExceptionHandler>()` registers the global handler.
3. `app.UseExceptionHandler()` is added early in the middleware pipeline to catch downstream errors.
4. `GlobalExceptionHandler`:
   - logs the exception with request context
   - maps exception to a safe HTTP status/title/detail/type
   - writes a ProblemDetails response through `IProblemDetailsService`

## ProblemDetails Response Fields

Returned error responses include:

- `type`
- `title`
- `status`
- `detail`
- `traceId` (in `extensions`)

`traceId` is derived from the current request context (`Activity.Current?.Id` fallback to `HttpContext.TraceIdentifier`).

## Logging Behavior

- Unexpected exceptions are logged at `Error` level.
- Request-related failures (for example bad requests) are logged at `Warning` level.
- Log entries include request method, request path, and trace identifier.

## Environment Behavior

- Development:
  - keeps the standard ProblemDetails shape
  - adds minimal diagnostics via `extensions.exception` (exception type name)
  - 500 detail uses exception message for easier local debugging
- Production:
  - does not include stack traces
  - 500 detail is a safe generic message (`An unexpected error occurred.`)

## Verification

1. Build and run:

```powershell
dotnet build
dotnet run --project src/SaaS.Api
```

2. Trigger a test exception:

```powershell
curl http://localhost:5000/api/foundation/error
```

Expected result:

- HTTP status code: `500`
- Content-Type: `application/problem+json`
- JSON contains `type`, `title`, `status`, `detail`, and `traceId`

Example shape:

```json
{
  "type": "https://httpstatuses.com/500",
  "title": "Internal Server Error",
  "status": 500,
  "detail": "An unexpected error occurred.",
  "traceId": "..."
}
```

## Intentionally Not Included Yet

- No domain-specific exception taxonomy
- No validation framework integration
- No advanced error framework or custom envelope system
