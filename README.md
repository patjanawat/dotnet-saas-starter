# SaaS.Starter

Spec-driven .NET SaaS starter repository for building a modular monolith with clear architecture boundaries and execution guided by Markdown specifications.

## Baselines

- Architecture: modular monolith
- Tenancy: shared database, shared schema, tenant isolation via `TenantId`
- Auth: ASP.NET Core Identity with secure cookies now, JWT-ready later
- Observability: OpenTelemetry-first (traces, metrics, logs)

## Repository Structure

- `src/`: application source projects
  - `SaaS.Api`: ASP.NET Core Web API host
  - `SaaS.Application`: application use cases and orchestration
  - `SaaS.Domain`: core domain model and rules
  - `SaaS.Infrastructure`: infrastructure adapters and integrations
- `tests/`: automated tests
  - `SaaS.UnitTests`
  - `SaaS.IntegrationTests`
- `specs/`: source of truth for product and system intent
  - `00-foundation`
  - `10-business`
  - `20-product`
  - `30-modules`
  - `40-system`
  - `50-execution`
- `docs/`: supporting documentation

`/specs` is the source of truth. Code should implement approved specs, not replace them.

## Getting Started

1. Install .NET SDK 10+ (or compatible with this repository).
2. Restore and build:
   - `dotnet restore SaaS.Starter.sln`
   - `dotnet build SaaS.Starter.sln`
3. Run API:
   - `dotnet run --project src/SaaS.Api`
4. Run tests:
   - `dotnet test SaaS.Starter.sln`

## Development Principles

- Spec-first: start with/update specs before implementation.
- Keep modules explicit: enforce boundaries between Domain, Application, Infrastructure, and API.
- Build incrementally: prefer small, reviewable changes per spec slice.
- Production-minded defaults: secure by default, observable by default, testable by default.
