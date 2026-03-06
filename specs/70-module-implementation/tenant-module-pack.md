# Tenant Module Pack

Status: Draft

## Purpose
Define implementation instructions for tenant lifecycle and tenant configuration management.

Scope:
- Manage tenant creation, activation, suspension, and archival.
- Provide tenant context metadata required by tenant-scoped modules.
- Enforce tenant status checks before protected operations.

## Domain Entities
- `Tenant`
- Fields: `Id`, `Code`, `Name`, `Status`, `CreatedAtUtc`, `ActivatedAtUtc`, `SuspendedAtUtc`, `ArchivedAtUtc`
- Rules: `Code` unique across platform

- `TenantConfiguration`
- Fields: `Id`, `TenantId`, `Key`, `Value`, `UpdatedAtUtc`
- Rules: unique by (`TenantId`, `Key`)

## Use Cases
- Create tenant with baseline defaults.
- Activate tenant after onboarding checks pass.
- Suspend/reactivate tenant based on policy decision.
- Update tenant configuration by authorized administrators.

## Commands
- `CreateTenantCommand`
- `ActivateTenantCommand`
- `SuspendTenantCommand`
- `ReactivateTenantCommand`
- `UpdateTenantConfigurationCommand`

Command rules:
- Enforce allowed state transitions.
- Require platform-level authorization policy for lifecycle operations.
- Emit audit events for all state changes.

## Queries
- `GetTenantByIdQuery`
- `GetTenantByCodeQuery`
- `GetTenantConfigurationQuery`
- `ListTenantsQuery`

Query rules:
- Platform-scoped queries require platform admin authorization.
- Tenant-level reads must validate caller permissions.

## API Endpoints
- `POST /api/tenant/tenants`
- `POST /api/tenant/tenants/{id}/activate`
- `POST /api/tenant/tenants/{id}/suspend`
- `POST /api/tenant/tenants/{id}/reactivate`
- `GET /api/tenant/tenants/{id}`
- `GET /api/tenant/tenants/{id}/configuration`
- `PUT /api/tenant/tenants/{id}/configuration`

Endpoint rules:
- Enforce explicit authorization policies.
- Return state transition failures via `ProblemDetails`.
- Include `traceId` and `errorCode` on errors.

## Persistence
- PostgreSQL via Entity Framework Core.
- Tables:
- `tenants`
- `tenant_configurations`

Persistence rules:
- `tenant_configurations.tenant_id` is required FK.
- Add indexes on `code`, `status`, and `tenant_id`.
- Use optimistic concurrency for mutable lifecycle/configuration rows.

## Tests
- Unit:
- Tenant state machine transition guards.
- Configuration key uniqueness rules.

- Integration:
- Tenant lifecycle persistence and transition behavior.
- Authorization policy checks on lifecycle commands.

- API:
- State transition success/failure paths.
- `ProblemDetails` contract for invalid transitions.

- Multi-tenant:
- Ensure tenant config reads/writes remain scoped to authorized contexts.
