# Data Access Guardrails

Status: Draft

## Purpose
Control EF Core and PostgreSQL access patterns to keep data safe and consistent.

## Mandatory Rules
1. Data access MUST occur through module-owned repositories/data services.
2. Queries MUST be scoped to module ownership and tenant context rules.
3. Raw SQL MUST be parameterized and justified.
4. Migrations MUST be deterministic and reviewed.
5. Schema changes MUST include backward-compatibility and rollback notes.

## EF Core Rules
- Entity configuration MUST enforce required fields and constraints.
- Tenant-owned entities MUST map `TenantId` as required.
- Indexes MUST support tenant-scoped lookup and uniqueness needs.

## Transaction Rules
- State-changing use cases MUST define explicit transactional boundaries.
- Cross-module transactional coupling SHOULD be avoided; use events/contracts when possible.

## Blocking Conditions
- Direct table updates across module boundaries
- Missing tenant filter on tenant-owned data access
- Non-parameterized SQL against external input
