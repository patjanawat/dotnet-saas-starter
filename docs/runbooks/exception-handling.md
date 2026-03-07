# Phase 2 Step 2.17: Global Exception Handling

## Scope
Introduce a production-friendly global exception flow in `SaaS.Api` with consistent `ProblemDetails` responses.
This step does not add business features.

## Error Handling Flow

1. API request enters middleware pipeline.
2. `UseExceptionHandler()` catches unhandled exceptions from downstream.
3. `GlobalExceptionHandler` maps exception types to an `ErrorModel`.
4. `ErrorModel` is returned as RFC7807 `ProblemDetails` JSON via `TypedResults.Problem`.
5. Logs are written with severity based on mapped status code.

## Mapping Structure

`ExceptionMapping.Map(Exception)` currently handles:

- `AppException` -> uses application code/message/status
- `BadHttpRequestException` -> `request.bad_request` (400)
- fallback `Exception` -> `system.unhandled_exception` (500)

This keeps the structure small now and extensible later for domain/application-specific exceptions.

## Response Shape

Global error responses follow `application/problem+json` with extensions:

- `errorCode`
- `traceId`

Example fields:

- `title`
- `detail`
- `status`
- `errorCode`
- `traceId`

Internal implementation details (for example exception type/stack trace) are not returned to clients.

## Verification

From repository root:

```powershell
$env:DOTNET_SKIP_FIRST_TIME_EXPERIENCE='1'
$env:DOTNET_CLI_HOME="$PWD\\.dotnet-cli"

dotnet build SaaS.Starter.sln
dotnet run --project src/SaaS.Api/SaaS.Api.csproj
```

Trigger an error:

```powershell
curl http://localhost:5207/api/foundation/throw
```

Expected behavior:

- HTTP status: `500`
- Content-Type: `application/problem+json`
- JSON contains `errorCode = "system.unhandled_exception"` and non-empty `traceId`
- logs contain matching `TraceId` and captured exception entry
