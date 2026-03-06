# Testing Guide

This repository has two automated test suites:

- `tests/SaaS.UnitTests` for domain/unit behavior
- `tests/SaaS.IntegrationTests` for API/infrastructure slices

## Prerequisites

- .NET SDK 10.x installed
- Network access to NuGet (`https://api.nuget.org`) for restore

## Quick Start

From repository root:

```powershell
dotnet restore SaaS.Starter.sln
dotnet build SaaS.Starter.sln
dotnet test SaaS.Starter.sln
```

## Run By Suite

Unit tests only:

```powershell
dotnet test tests/SaaS.UnitTests/SaaS.UnitTests.csproj
```

Integration tests only:

```powershell
dotnet test tests/SaaS.IntegrationTests/SaaS.IntegrationTests.csproj
```

Run a single test class:

```powershell
dotnet test tests/SaaS.IntegrationTests/SaaS.IntegrationTests.csproj --filter "FullyQualifiedName~FoundationBaselineTests"
```

## API Smoke Checks

Start API:

```powershell
dotnet run --project src/SaaS.Api/SaaS.Api.csproj
```

Default dev URL from launch settings: `http://localhost:5207`

Smoke endpoints:

```powershell
curl http://localhost:5207/api/foundation/ping
curl http://localhost:5207/api/foundation/throw
```

Expected:

- `/api/foundation/ping` returns `200` and `{ "status": "ok" }`
- `/api/foundation/throw` returns an error response (`ProblemDetails`) via global exception middleware

## Coverage (Optional)

```powershell
dotnet test SaaS.Starter.sln --collect:"XPlat Code Coverage"
```

## Troubleshooting

- `NU1301` during restore/build/test:
  - Cause: NuGet feed unreachable or blocked.
  - Fix: verify internet/proxy/firewall and retry `dotnet restore`.
- IntelliSense/build shows stale errors after package/reference changes:
  - Run `dotnet clean`, then `dotnet restore`, then `dotnet build`.
  - In Visual Studio, use `Clean Solution` then `Rebuild Solution`.
