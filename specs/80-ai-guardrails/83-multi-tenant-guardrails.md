# Multi-Tenant Guardrails

Status: Draft

## Purpose
Enforce tenant isolation in shared database/shared schema architecture.

## Mandatory Rules
1. Tenant-owned entities MUST include non-null `TenantId`.
2. Tenant-scoped queries MUST filter by resolved `TenantId`.
3. Tenant-scoped mutations MUST validate tenant context before execution.
4. Cross-tenant read/write paths MUST be blocked by default.
5. Tenant mismatches between principal and resource MUST return failure.

## Data Model Rules
- Tenant-scoped unique constraints MUST include `TenantId` where uniqueness is per tenant.
- Foreign keys between tenant-owned records MUST remain tenant-consistent.

## API Rules
- Tenant context MUST be resolved before application handler invocation.
- Tenant mismatch MUST return `ProblemDetails` with explicit `errorCode`.

## Blocking Conditions
- Missing tenant filter on tenant-owned query
- Any path allowing unauthorized cross-tenant data access
