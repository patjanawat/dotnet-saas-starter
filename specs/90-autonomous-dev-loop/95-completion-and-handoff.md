# Completion and Handoff

Status: Draft

## Purpose
Define mandatory completion and handoff output for each autonomous slice.

## Completion Criteria
A slice is complete only if:
1. Requirement traceability is complete (`requirement -> files -> tests`).
2. Self-review status is `Pass` or `Pass with Follow-Up`.
3. Required validation checks are green.
4. Guardrail checks are satisfied.
5. No unresolved blocking risks remain.

## Handoff Package
Each slice handoff MUST include:
- Slice ID and requirement IDs
- Summary of implemented behavior
- Changed files grouped by module/layer
- Tests added/updated and pass status
- Security/tenant checks performed
- Audit/observability impact notes
- Known follow-up items and rationale

## Handoff Rules
- Use explicit file paths and deterministic statements.
- Avoid vague language such as "fixed" or "improved" without evidence.
- Mark incomplete items clearly as follow-up, not hidden in summary text.

## Escalation Handoff
If blocked, handoff MUST include:
- Blocking condition
- Attempts made
- Why autonomous retry could not resolve
- Minimum required clarification to continue
