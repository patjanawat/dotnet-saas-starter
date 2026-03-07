# Phase 2 - Step 2.16 - Logging Foundation

## Purpose
Establish a structured, production-friendly logging baseline for `SaaS.Api` using Serilog with minimal complexity.

## What Was Added or Updated
- Updated `src/SaaS.Api/Program.cs`
  - Serilog bootstrap logger is configured early to capture startup failures.
  - Serilog remains the host logger via `builder.Host.UseSerilog(...)`.
  - Added `app.UseSerilogRequestLogging()` with default concise structured request logging.
  - Middleware order keeps request logging early in the pipeline.
- Updated `src/SaaS.Api/SaaS.Api.csproj`
  - Added `Serilog.Settings.Configuration` package reference.
- Updated `Directory.Packages.props`
  - Added central package version for `Serilog.Settings.Configuration`.
- Updated `src/SaaS.Api/appsettings.Development.json`
  - Serilog minimum levels aligned to balanced defaults:
    - `Default`: `Information`
    - `Microsoft.AspNetCore`: `Warning`

## Logging Packages and Configuration
- `Serilog.AspNetCore`: host integration and request logging middleware.
- `Serilog.Settings.Configuration`: enables reading Serilog settings from appsettings.
- `Serilog.Sinks.Console`: structured console output for local development and baseline hosting scenarios.

## How Serilog Is Wired
1. A bootstrap logger is created before host build.
2. `UseSerilog(...)` wires Serilog into ASP.NET Core logging.
3. The logger reads from application configuration (`appsettings*.json` + environment variables).
4. Startup exceptions are captured in the `try/catch` with `Log.Fatal(...)`.
5. Shutdown flushes logs with `Log.CloseAndFlush()`.

## Request Logging Behavior
- `app.UseSerilogRequestLogging()` is enabled.
- Default Serilog request log includes method, path, status code, and elapsed time in a single structured event.
- No custom noisy per-request enrichment is added at this stage.

## Verification
Run from repository root:

```powershell
dotnet build
dotnet run --project src/SaaS.Api
```

In another shell, send a sample request:

```powershell
curl http://localhost:5000/api/foundation/ping
```

## Expected Logs
- On startup:
  - informational host startup logs from Serilog
- Per request:
  - one concise request completion log containing method, path, status code, and elapsed milliseconds

## Intentionally Not Included Yet
- No OpenTelemetry changes in this step.
- No external log aggregation sinks/platform integrations.
- No domain/business logging abstractions.
- No exception handling redesign or broader configuration refactor.
