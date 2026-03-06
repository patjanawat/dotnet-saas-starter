# Error Prevention Checklist

Status: Draft

## Purpose
Provide a mandatory pre-flight and pre-merge checklist to prevent common implementation failures in AI-assisted delivery.

Use this checklist before code generation, before merge, and before release.

## Spec Alignment Checks
- Requirements are sourced from approved BRD/PRD/MDD/SDD documents.
- Functional requirement IDs are identified and mapped to tasks.
- Ambiguous statements are resolved into deterministic rules.
- In-scope/out-of-scope boundaries are explicitly documented.
- Acceptance criteria are testable and mapped to verification steps.

## Design Consistency Checks
- Module ownership and boundaries from MDD are preserved.
- Cross-module interactions use published contracts only.
- Project references follow SDD dependency rules.
- New endpoints follow route and API contract conventions.
- Failure responses use `ProblemDetails` standard contract.

## Code Quality Checks
- Domain, application, infrastructure, and API concerns are separated correctly.
- No business logic is hidden in controllers/endpoints or infrastructure adapters.
- Validation is explicit for command/query inputs.
- Error handling is explicit; no silent catch-and-ignore behavior.
- Naming follows established conventions and remains consistent.

## Data and Migration Checks
- Tenant-owned entities include and enforce `TenantId`.
- Tenant filters are applied to all tenant-scoped reads/writes.
- Unique indexes include `TenantId` when scope is per-tenant.
- Migrations are deterministic and reviewed.
- Breaking schema changes include compatibility and rollback notes.

## Runtime and Observability Checks
- Structured logs include correlation context (`traceId`, `tenantId` when available).
- Audit logs exist for privileged and state-changing operations.
- Health endpoints `/health/live` and `/health/ready` remain operational.
- Metrics/traces are emitted for changed critical paths.
- Sensitive values are excluded or redacted from logs.

## Final Review Checks
- Requirement -> code -> test traceability is complete.
- All required tests pass for impacted scope.
- Security and authorization checks pass for protected endpoints.
- Known risks and assumptions are documented in handoff notes.
- Definition of Done criteria in `52-definition-of-done.md` are fully satisfied.
