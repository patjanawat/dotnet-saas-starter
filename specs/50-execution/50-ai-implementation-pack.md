# AI Implementation Pack

Status: Draft

## Purpose
Define the standard execution pack used by AI agents to convert approved specifications into safe, consistent implementation work.

This pack is mandatory for implementation planning and generation in this repository.

## Input Specs
AI agents must read and reconcile all of the following before generating code:
- `specs/10-business/10-brd.md` (business goals, constraints, risks)
- `specs/20-product/20-prd.md` (product behavior and functional/non-functional requirements)
- `specs/30-modules/30-mdd.md` (module boundaries, entities, interactions)
- `specs/40-system/40-sdd.md` (technical architecture, infrastructure, security, testing baseline)
- `specs/00-foundation/01-ai-spec-rules.md` (authoring and determinism rules)

Spec precedence rules:
1. If requirements conflict, apply stricter safety/security rule first.
2. `40-sdd.md` controls technical conventions and architecture constraints.
3. `30-mdd.md` controls module boundaries and ownership.
4. `20-prd.md` controls product behavior and acceptance semantics.
5. `10-brd.md` controls business intent and prioritization.
6. Ambiguities must be resolved in writing before code generation.

## Implementation Scope
Each implementation run must declare:
- Feature/module target.
- In-scope requirement IDs (for example, `FR-001`, `FR-006`).
- Out-of-scope items explicitly excluded.
- Target projects/files expected to change.
- Test types to be produced or updated.

Scope rules:
1. Do not implement behavior not traceable to an approved requirement.
2. Do not cross module boundaries without explicit contract updates.
3. Do not introduce new infrastructure patterns outside SDD baseline unless approved.
4. Do not bypass tenant isolation, authorization policy checks, or audit logging requirements.

## Constraints
Mandatory architecture constraints:
- Modular monolith structure must be preserved.
- Shared database/shared schema model with `TenantId` rules must be enforced.
- API error contract must use `ProblemDetails` with `traceId` and `errorCode`.
- AuthN/AuthZ must follow ASP.NET Core Identity + cookie auth + RBAC + policy checks.
- Observability must include structured logs, health checks, and audit logging.

Code generation constraints:
1. Generate DTOs for API contracts; do not expose domain entities directly.
2. Keep domain logic in domain layer and orchestration in application layer.
3. Keep infrastructure dependencies out of domain projects.
4. Keep endpoint routes consistent with `/api/{module}/{resource}` or approved versioned form.

## Execution Plan
Standard AI execution sequence:
1. Parse requirements and build a requirement-to-code map.
2. Identify impacted modules, contracts, and test surfaces.
3. Produce a task breakdown using `51-task-breakdown-template.md`.
4. Implement changes in smallest safe increments.
5. Add/adjust tests aligned to acceptance criteria.
6. Run validation checks (build, tests, lint/static checks as available).
7. Verify against `52-definition-of-done.md` and `53-error-prevention-checklist.md`.
8. Prepare handoff summary with traceability evidence.

Change sequencing rules:
- Apply contract changes before downstream consumers.
- Apply schema changes before code paths that depend on them.
- Apply security and tenant guards before exposing new endpoints.
- Apply observability hooks before finalizing implementation.

## Verification Strategy
Required verification outputs per implementation run:
- Requirement traceability table (requirement -> files -> tests).
- Evidence of tenant isolation checks for tenant-owned data access.
- Evidence of authorization policy enforcement on protected endpoints.
- Evidence that error handling returns `ProblemDetails`.
- Evidence that audit logging is present for privileged/state-changing operations.
- Test results summary (unit/integration/API contract as applicable).

Verification rules:
1. Missing traceability is a blocker.
2. Failing tests are a blocker.
3. Unreviewed cross-module dependency changes are a blocker.
4. Unspecified security behavior is a blocker.

## Handoff Notes
Every AI implementation handoff must include:
- Scope completed and excluded.
- Requirement IDs covered.
- Files changed by module and layer.
- Database/migration impact summary (if any).
- Operational impact summary (health, logging, metrics, alerts).
- Known risks, assumptions, and follow-up tasks.

Handoff format:
- Keep statements deterministic and testable.
- Use explicit paths and requirement IDs.
- Avoid vague phrases such as "handled", "optimized", or "secured" without evidence.
