# Create Tenant Slice

Status: Draft

## Purpose
Define the initial tenant onboarding slice: create a tenant with baseline configuration.

## Specs Used
- `specs/20-product/20-prd.md` (FR-001, FR-002, FR-003, FR-006)
- `specs/30-modules/30-mdd.md` (Tenant module state model and ownership)
- `specs/40-system/40-sdd.md` (tenant isolation rules, API conventions, persistence baseline)
- `specs/70-module-implementation/tenant-module-pack.md`
- `specs/80-ai-guardrails/83-multi-tenant-guardrails.md`

## Scope
In scope:
- Create tenant entity in `Pending` state.
- Create baseline tenant configuration entries.
- Enforce platform-level authorization for tenant creation.
- Return deterministic API response and errors.

Out of scope:
- Tenant activation workflow
- Billing integration
- Advanced provisioning orchestration

## Domain
- Entities: `Tenant`, `TenantConfiguration`.
- Domain rule: tenant code must be globally unique.
- Domain rule: new tenant starts in `Pending` state.

## Application
- Implement `CreateTenantCommand` + handler.
- Validate tenant name/code and required baseline configuration.
- Enforce platform admin policy before mutation.
- Emit `TenantCreated` integration event and audit log entry.

## API
- Endpoint: `POST /api/tenant/tenants`
- Request DTO: tenant code, name, baseline options.
- Success response: tenant summary DTO with initial status.
- Failure response: `ProblemDetails` with `traceId` and `errorCode`.
- Duplicate code must map to deterministic business failure.

## Persistence
- EF Core + PostgreSQL tables:
- `tenants`
- `tenant_configurations`

Persistence rules:
- `tenants.code` unique index.
- `tenant_configurations` unique index on (`tenant_id`, `key`).
- UTC timestamps for creation and update fields.

## Tests
- Unit:
- Tenant code uniqueness and initial state rule.

- Integration:
- Command persists tenant + baseline config in one transaction.
- Duplicate tenant code failure behavior.

- API:
- Authorized creation success path.
- Unauthorized/forbidden path.
- Error contract for duplicate code.

- Multi-tenant:
- Ensure created tenant context is isolated by `TenantId`.

## Acceptance Criteria
1. Authorized platform admin can create tenant successfully.
2. Created tenant is persisted in `Pending` state with baseline configuration.
3. Duplicate tenant code returns deterministic `ProblemDetails` failure.
4. Creation action emits audit log with actor, tenant, trace context.
