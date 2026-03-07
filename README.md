# SaaS.Starter

Spec-driven .NET 10 SaaS starter repository for a **Modular Monolith** using **Clean Architecture** boundaries.

## Project Overview

This repository provides a production-minded baseline for building SaaS capabilities incrementally.
Focus is on maintainable architecture, predictable operations, and AI-assisted development workflows.

## Architecture Style

- Modular Monolith
- Clean Architecture layers:
  - `SaaS.Api`
  - `SaaS.Application`
  - `SaaS.Domain`
  - `SaaS.Infrastructure`
  - `SaaS.Contracts`
  - `SaaS.UnitTests`
  - `SaaS.IntegrationTests`

## Tech Stack Summary

- .NET 10, ASP.NET Core Web API
- EF Core + PostgreSQL
- ASP.NET Core Identity (cookie auth now, JWT-ready configuration)
- Serilog structured logging
- OpenTelemetry tracing/metrics
- Swagger/OpenAPI
- xUnit integration/unit tests

## Solution Structure

- `src/`
  - `SaaS.Api`: API host, endpoints, middleware, runtime composition
  - `SaaS.Application`: use-case contracts and orchestration
  - `SaaS.Domain`: domain model and rules
  - `SaaS.Infrastructure`: persistence, identity, infra services
  - `SaaS.Contracts`: shared contracts
- `tests/`
  - `SaaS.UnitTests`
  - `SaaS.IntegrationTests`
- `docs/`
  - runbooks and operational notes
- `specs/`
  - product/system execution specs (source-of-truth intent)

## Restore, Build, Run

From repository root:

```powershell
dotnet restore SaaS.Starter.sln
dotnet build SaaS.Starter.sln
dotnet run --project src/SaaS.Api/SaaS.Api.csproj
```

Default local API URL (launch settings): `http://localhost:5207`

## Run with Docker

Run API + PostgreSQL stack:

```powershell
docker compose up --build
```

API URL via compose: `http://localhost:8080`

See runbook: `docs/runbooks/local-development.md`

## Run Tests

```powershell
dotnet test SaaS.Starter.sln
dotnet test tests/SaaS.UnitTests/SaaS.UnitTests.csproj
dotnet test tests/SaaS.IntegrationTests/SaaS.IntegrationTests.csproj
```

See: `docs/testing.md`

## EF Migrations

Create migration:

```powershell
dotnet ef migrations add InitialFoundation -p src/SaaS.Infrastructure/SaaS.Infrastructure.csproj -s src/SaaS.Api/SaaS.Api.csproj -o Persistence/Migrations
```

Apply migration:

```powershell
dotnet ef database update -p src/SaaS.Infrastructure/SaaS.Infrastructure.csproj -s src/SaaS.Api/SaaS.Api.csproj
```

List migrations:

```powershell
dotnet ef migrations list -p src/SaaS.Infrastructure/SaaS.Infrastructure.csproj -s src/SaaS.Api/SaaS.Api.csproj
```

See: `docs/runbooks/persistence-foundation.md`

## Repository Conventions

- Spec-first and incremental delivery
- Keep layer boundaries explicit
- Prefer practical defaults over over-engineering
- Use central package management (`Directory.Packages.props`)
- Avoid package version changes unless truly necessary
- Use runbooks for repeatable local operations/troubleshooting

See:
- `docs/common-commands.md`
- `docs/repository-standards.md`
- `docs/runbooks/troubleshooting.md`

## Foundation Summary

Current repository foundation includes:
- repository standards
- logging
- global exception handling
- strongly typed configuration/options
- persistence and migration readiness
- Docker Compose local stack

Detailed summary: `docs/foundation-summary.md`
