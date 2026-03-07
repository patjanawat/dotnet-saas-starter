# Phase 2 Step 2.16: Logging Foundation

## Purpose
Establish a structured, production-friendly logging baseline for `SaaS.Api` using Serilog with minimal complexity.

## What Was Added
- Serilog host bootstrap logger during startup (`Program.cs`).
- Serilog host integration via `builder.Host.UseSerilog(...)`.
- HTTP request logging middleware via `app.UseSerilogRequestLogging(...)`.
- Environment-aware log level defaults from configuration:
  - `appsettings.json`: `Information` default, `Warning` for Microsoft/System noise.
  - `appsettings.Development.json`: lower default level (`Debug`) for local diagnostics.
- Console sink only (no external vendor sinks).

## Design Notes
- Startup logging captures failures before app host is fully built.
- Request logging emits one structured event per request with status code and elapsed time.
- Request logs are leveled by outcome:
  - `Error` for unhandled exception / 5xx responses
  - `Warning` for slow requests (> 1000 ms)
  - `Information` otherwise
- Existing request context scope middleware is preserved and complements structured request logs.

## Verification
Run from repository root:

```powershell
$env:DOTNET_SKIP_FIRST_TIME_EXPERIENCE='1'
$env:DOTNET_CLI_HOME="$PWD\\.dotnet-cli"

dotnet build SaaS.Starter.sln
dotnet run --project src/SaaS.Api/SaaS.Api.csproj
```

In another shell, send a request:

```powershell
curl http://localhost:5207/health/live
```

Optional error-path check:

```powershell
curl http://localhost:5207/api/foundation/throw
```

## Expected Logs
- On startup:
  - host start message
  - application boot/configuration message
  - framework hosting messages
- Per request:
  - `HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed} ms`
  - structured properties including trace and request metadata
- On unhandled exception:
  - error from global exception middleware with trace identifier
