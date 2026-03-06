# AI Code Review Checklist

Status: Draft

## Purpose
Provide the final mandatory review gate for AI-generated code before merge.

## 1. Spec Traceability
- All generated behavior maps to approved requirement IDs.
- No out-of-scope behavior was introduced.
- Acceptance criteria are represented by tests.

## 2. Architecture Integrity
- Modular monolith boundaries are preserved.
- No forbidden project references were introduced.
- Cross-module interactions use contracts/events only.

## 3. Tenant Isolation
- Tenant-owned entities include non-null `TenantId`.
- Tenant filters are applied on tenant-scoped queries and mutations.
- No cross-tenant access path exists without explicit platform-level policy.

## 4. API Contract Quality
- Routes follow repository conventions.
- Request/response DTOs are explicit and stable.
- Errors return `ProblemDetails` with `traceId` and `errorCode`.
- Validation failures return `ValidationProblemDetails`.

## 5. Authentication and Authorization
- Endpoint auth requirements are explicit.
- RBAC and policy checks are present for protected operations.
- Tenant membership/context checks run before business mutation.

## 6. Observability and Audit
- Structured logs include correlation fields.
- Audit logs exist for privileged/state-changing actions.
- Health endpoints remain valid: `/health/live`, `/health/ready`.

## 7. Data and Migrations
- EF mappings respect schema conventions and ownership boundaries.
- Migration changes are deterministic and reviewed.
- Unique constraints include `TenantId` where tenant-scoped uniqueness is required.

## 8. Test Coverage
- Unit, integration, and API tests cover in-scope requirement IDs.
- Security and tenant isolation tests are present for changed endpoints.
- Critical edge cases from specs are covered.

## 9. Release Safety
- Configuration changes are documented.
- Backward compatibility and rollback notes exist for risky changes.
- Definition of Done checks are satisfied.

## Review Outcome
- `PASS`: all critical items satisfied.
- `CONDITIONAL PASS`: non-critical gaps with follow-up tasks recorded.
- `FAIL`: critical architecture, tenant, security, or traceability violations.
