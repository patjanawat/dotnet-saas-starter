# Definition of Done

Status: Draft

## Purpose
Define mandatory completion gates for implementation work produced by AI agents and engineers.

No work item is complete unless all applicable gates below pass.

## Required Engineering Checks
- Requirement traceability exists from source requirement IDs to changed files.
- Architecture constraints from MDD/SDD are preserved.
- Module boundaries are respected; no direct cross-module entity mutation.
- Project reference rules are respected (no forbidden dependencies).
- API contracts use DTOs and do not expose persistence/domain internals.
- Error behavior uses `ProblemDetails` with required fields and extensions (`traceId`, `errorCode`).

Engineering gate:
- If any item fails, status remains `Not Done`.

## Required Testing Checks
- Unit tests added/updated for changed business/domain logic.
- Integration tests added/updated for module interactions and persistence behavior.
- API contract tests validate success and failure responses.
- Tenant isolation tests validate `TenantId` scoping rules.
- Authorization tests validate RBAC + policy behavior on protected endpoints.
- Health checks `/health/live` and `/health/ready` remain valid.

Testing gate:
- Failing or missing required tests block completion.

## Required Documentation Checks
- Task breakdown completed using `51-task-breakdown-template.md`.
- Requirement-to-test mapping documented.
- Migration notes included for schema changes.
- Operational notes included for logging/metrics/alerts impact.
- Assumptions and known limitations explicitly documented.

Documentation gate:
- Missing docs for impacted areas block completion.

## Security and Compliance Checks
- Authentication and authorization behavior matches SDD baseline.
- Tenant context is validated before tenant-owned data operations.
- Privileged/state-changing actions emit audit logs.
- Sensitive data is not logged in plaintext.
- New dependencies are reviewed for security risk.

Security gate:
- Any unresolved high-severity security issue blocks completion.

## Release Readiness Checks
- Build is green for impacted solution/project set.
- Required test suites pass in target CI pipeline scope.
- Config changes are documented and environment-safe.
- Rollback plan exists for risky or schema-affecting changes.
- Handoff summary includes scope, evidence, and residual risks.

Release gate:
- Work can be marked `Done` only when all applicable gates pass and evidence is attached.
