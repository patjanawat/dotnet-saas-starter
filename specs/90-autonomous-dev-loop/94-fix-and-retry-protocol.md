# Fix and Retry Protocol

Status: Draft

## Purpose
Define a controlled retry loop when a slice fails validation or self-review.

## Retry Trigger Conditions
- Build/test failures in in-scope files
- Guardrail violations
- Missing traceability or missing required tests
- Contract mismatch (`ProblemDetails`, auth, tenant rules)

## Retry Steps
1. Classify failure
- `Design issue`, `Implementation defect`, `Test gap`, `Config/environment issue`

2. Apply minimal fix
- Change only files required to resolve the classified issue.
- Preserve slice scope unless explicit escalation is approved.

3. Re-validate
- Re-run impacted checks.
- Re-run full required slice checks before completion.

4. Re-review
- Re-run self-review protocol fully.

## Retry Limits
- Default maximum retry attempts per slice: `3`
- If still failing at max attempts, AI agent MUST stop and hand off blocker details.

## Retry Record Requirements
- Failure summary
- Root cause classification
- Fix applied
- Validation evidence after retry
- Remaining risk notes
