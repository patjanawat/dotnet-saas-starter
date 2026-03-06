# AI Code Generation Pipeline

Status: Draft

## Purpose
Define the mandatory pipeline AI agents must follow to generate production-grade .NET SaaS code from repository specs.

## Inputs
AI agents must read these inputs before generating code:
- `specs/00-foundation/01-ai-spec-rules.md`
- `specs/10-business/10-brd.md`
- `specs/20-product/20-prd.md`
- `specs/30-modules/30-mdd.md`
- `specs/40-system/40-sdd.md`
- `specs/50-execution/50-ai-implementation-pack.md`
- `specs/50-execution/52-definition-of-done.md`
- `specs/50-execution/53-error-prevention-checklist.md`

## Pipeline Stages
1. Spec parse and traceability map
- Extract requirement IDs and acceptance criteria.
- Map each requirement to module, layer, API, data model, and test type.

2. Scope lock
- Declare in-scope and out-of-scope behavior.
- Reject generation if requirements are ambiguous or conflicting.

3. Scaffolding
- Generate projects/files in the order defined by `61-scaffolding-order.md`.
- Enforce project reference and layering rules from SDD.

4. Feature implementation
- Generate domain and application behavior first.
- Generate API endpoints and contracts after application contracts are stable.
- Generate infrastructure adapters and persistence last for each feature slice.

5. Test generation
- Generate unit, integration, and API contract tests per `64-test-generation-template.md`.

6. Validation and review
- Run static checks/build/tests as available.
- Validate using `65-code-review-checklist.md`.
- Block output if critical checklist items fail.

## Non-Negotiable Constraints
- Preserve modular monolith boundaries.
- Enforce tenant isolation with `TenantId` on tenant-owned data.
- Use `ProblemDetails` with `traceId` and `errorCode` for API failures.
- Apply ASP.NET Core Identity cookie authentication baseline.
- Apply RBAC plus policy-based authorization on protected endpoints.
- Add audit logging for privileged or state-changing actions.

## Blocking Conditions
AI generation must stop and request spec clarification if:
- Requirement IDs are missing for target behavior.
- Module ownership is unclear.
- Tenant boundary rules are undefined for new entities/endpoints.
- Error contract rules are not specified for new API behavior.

## Output Contract
Every generation run must output:
- Requirement traceability table (`requirement -> files -> tests`).
- List of generated/modified files grouped by module.
- List of applied architectural rules.
- List of unresolved risks and assumptions.
