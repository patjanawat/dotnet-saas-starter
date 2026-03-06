# Task Breakdown Template

Status: Draft

## Purpose
Provide a deterministic template for converting approved specs into executable implementation tasks.

Use this template for every implementation slice.

## Work Item Metadata
- Work item title:
- Work item ID:
- Author:
- Date:
- Target module(s):
- Source requirements (IDs):
- Priority:
- Estimated complexity (S/M/L):
- Dependencies (work items or contracts):

## Task List
Use one row per task.

| Task ID | Task Description | Module/Layer | Requirement ID(s) | Output Artifact(s) | Owner | Status |
|---|---|---|---|---|---|---|
| T-001 | Example: Add tenant context resolver middleware check | AppHost/API | FR-002 | Middleware + tests | AI/Dev | Pending |

Task writing rules:
1. Each task maps to at least one requirement ID.
2. Each task names expected output artifacts.
3. Tasks must be independently verifiable.
4. Tasks must declare module and layer scope.

## Dependencies
List blocking dependencies with explicit direction.

Template:
- `T-xxx` depends on `T-yyy` because:
- Contract dependency:
- Schema dependency:
- External dependency:

Dependency rules:
1. No task starts if a hard dependency is unresolved.
2. Contract and schema dependencies must be implemented before consumers.

## Risks
List implementation risks and mitigation steps.

Template:
- Risk:
- Impact:
- Likelihood:
- Mitigation:
- Fallback:

Minimum required risk categories:
- Module boundary risk
- Tenant isolation risk
- Authorization/security risk
- Migration/data risk
- Operational/observability risk

## Validation Steps
Define exact validation for each task cluster.

Required validation checklist:
- Build succeeds for impacted projects.
- Unit tests pass for changed domain/application behavior.
- Integration/API tests pass for changed contracts.
- `ProblemDetails` shape is validated for failure paths.
- Tenant isolation and authorization behavior are validated.
- Audit logging exists for privileged/state-changing actions.

## Rollback Considerations
For each risky change, define rollback approach.

Template:
- Change group:
- Rollback trigger:
- Rollback method:
- Data rollback method (if schema/data affected):
- Verification after rollback:

Rollback rules:
1. Schema-impacting tasks must define backward-safe rollback.
2. Rollback must preserve tenant data integrity.
3. Rollback instructions must be executable without hidden assumptions.
