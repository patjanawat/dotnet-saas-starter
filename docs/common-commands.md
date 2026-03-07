# Common Commands

Frequently used commands for local development and baseline verification.
Run all commands from the repository root unless stated otherwise.

## Environment Setup

```powershell
dotnet --info
dotnet --list-sdks
dotnet restore SaaS.Starter.sln
dotnet build SaaS.Starter.sln -c Debug
```

## Deterministic Baseline Verification

```powershell
$env:DOTNET_SKIP_FIRST_TIME_EXPERIENCE='1'
$env:DOTNET_CLI_HOME="$PWD\\.dotnet-cli"

dotnet --version
dotnet restore SaaS.Starter.sln -m:1 -p:RestoreDisableParallel=true
dotnet build SaaS.Starter.sln -c Debug -m:1
dotnet test tests/SaaS.UnitTests/SaaS.UnitTests.csproj -c Debug --no-build
dotnet test tests/SaaS.IntegrationTests/SaaS.IntegrationTests.csproj -c Debug --no-build
```

## Run API

```powershell
dotnet run --project src/SaaS.Api/SaaS.Api.csproj
```

Default local URL: `http://localhost:5207`

## Smoke Checks

```powershell
curl http://localhost:5207/health/live
curl http://localhost:5207/health/ready
curl http://localhost:5207/api/foundation/ping
```

## Test Commands

```powershell
dotnet test SaaS.Starter.sln
dotnet test tests/SaaS.UnitTests/SaaS.UnitTests.csproj
dotnet test tests/SaaS.IntegrationTests/SaaS.IntegrationTests.csproj
```

Run a targeted test:

```powershell
dotnet test tests/SaaS.IntegrationTests/SaaS.IntegrationTests.csproj --filter "FullyQualifiedName~FoundationBaselineTests"
```

## Dependency and Reference Checks

```powershell
dotnet list src/SaaS.Api/SaaS.Api.csproj package
dotnet list src/SaaS.Application/SaaS.Application.csproj package
dotnet list src/SaaS.Infrastructure/SaaS.Infrastructure.csproj package
dotnet list tests/SaaS.UnitTests/SaaS.UnitTests.csproj package
dotnet list tests/SaaS.IntegrationTests/SaaS.IntegrationTests.csproj package
```

```powershell
dotnet list src/SaaS.Api/SaaS.Api.csproj reference
dotnet list src/SaaS.Application/SaaS.Application.csproj reference
dotnet list src/SaaS.Infrastructure/SaaS.Infrastructure.csproj reference
dotnet list src/SaaS.Domain/SaaS.Domain.csproj reference
dotnet list src/SaaS.Contracts/SaaS.Contracts.csproj reference
dotnet list tests/SaaS.UnitTests/SaaS.UnitTests.csproj reference
dotnet list tests/SaaS.IntegrationTests/SaaS.IntegrationTests.csproj reference
```

## Clean Rebuild

```powershell
dotnet clean SaaS.Starter.sln
dotnet restore SaaS.Starter.sln
dotnet build SaaS.Starter.sln -c Debug
```

## Docker Baseline

```powershell
docker compose up --build
docker compose down
```

## Troubleshooting

Detailed runbook: `docs/runbooks/troubleshooting.md`
Global exception handling runbook: `docs/runbooks/exception-handling.md`

List NuGet sources:

```powershell
dotnet nuget list source
```

If `NU1301` appears:
- Verify access to `https://api.nuget.org`.
- Retry `dotnet restore`.

If `MSB3021` or `MSB3027` appears (locked DLL):

```powershell
Get-Process dotnet | Select-Object Id,ProcessName,MainWindowTitle
Get-Process dotnet | Stop-Process -Force
dotnet build SaaS.Starter.sln -c Debug -m:1
```

If `CS1591` appears:
- It indicates missing XML comments on public API types/members.
- It is a documentation warning, not a runtime failure.

## Phase Note

- In Phase 2, this file was normalized and used as the single command baseline.
- Step-specific command notes were merged here to avoid duplicate command docs.
