# Scaffolding Order

Status: Draft

## Purpose
Define the strict generation order AI agents must follow to prevent architectural drift.

## Global Order
1. Shared solution scaffolding
2. SharedKernel primitives
3. Module domain layers
4. Module application layers
5. Module API layers
6. Module infrastructure layers
7. AppHost composition root
8. Database migrations
9. Tests

## Per-Module Order
For each module (`Identity`, `Tenant`, `User`, `Authorization`, `Observability`, `Notification`):
1. `Domain`
2. `Application`
3. `Api`
4. `Infrastructure`

## Dependency Rules
- `Domain` references only module-internal domain primitives and allowed SharedKernel abstractions.
- `Application` references only same-module `Domain` + SharedKernel contracts.
- `Api` references only same-module `Application` contracts.
- `Infrastructure` references same-module `Application`/`Domain` contracts.
- Cross-module access uses published contracts only.

## Order Guards
- Do not generate infrastructure repositories before domain entities and application contracts exist.
- Do not generate API handlers before command/query contracts exist.
- Do not generate cross-module consumers before source module contracts/events are generated.
- Do not generate migrations before EF models are finalized for the slice.

## Tenant and Security Gate in Scaffolding
Before generating API and persistence:
- Confirm `TenantId` strategy for each tenant-owned entity.
- Confirm authorization policy name and role mapping per endpoint.
- Confirm audit log event name for state-changing operations.
