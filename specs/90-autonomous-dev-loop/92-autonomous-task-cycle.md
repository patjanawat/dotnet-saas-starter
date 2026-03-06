# Autonomous Task Cycle

Status: Draft

## Purpose
Define the step-by-step execution cycle for each work slice.

## Task Cycle
1. Context load
- Read relevant requirement, module, system, and guardrail specs.
- Confirm scope lock and exclusions.

2. Plan and traceability map
- Map requirement IDs to intended code and tests.
- Identify impacted files by module and layer.

3. Implement
- Apply smallest viable change first.
- Preserve module boundaries and tenant isolation rules.
- Keep contracts explicit (`ProblemDetails`, auth policies, DTO boundaries).

4. Validate
- Run build/tests/checks available for impacted scope.
- Validate guardrails and definition-of-done criteria.

5. Record results
- Capture pass/fail status and evidence for each validation step.
- Update slice status.

## Mandatory Execution Rules
- No code without requirement traceability.
- No protected endpoint without explicit authorization policy decision.
- No tenant-owned data access without `TenantId` scoping.
- No silent failure handling.

## Exit Conditions
- `Complete`: all required checks pass.
- `Retry`: checks fail and fix path exists.
- `Stop`: blocked by guardrail or missing clarification.
