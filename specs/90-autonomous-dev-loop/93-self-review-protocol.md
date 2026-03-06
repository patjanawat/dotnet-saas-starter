# Self-Review Protocol

Status: Draft

## Purpose
Define mandatory self-review checks before a slice can be marked complete.

## Self-Review Checklist
1. Traceability
- Every changed behavior maps to requirement IDs.
- No unscoped behavior was introduced.

2. Architecture
- Module boundaries preserved.
- No forbidden project references.
- No direct cross-module data mutation.

3. Security and Tenant Isolation
- Auth policies defined for protected endpoints.
- Tenant validation present for tenant-scoped operations.
- No sensitive data leakage in API/log output.

4. API and Error Contracts
- DTO boundaries preserved.
- Errors use `ProblemDetails` with `traceId` and `errorCode`.

5. Observability
- Audit logging exists for privileged/state-changing actions.
- Health and telemetry behavior remains valid.

6. Testing
- Required unit/integration/API tests exist and pass.
- Negative path tests exist for auth/tenant/error behavior.

## Review Outcome
- `Pass`: proceed to completion/handoff.
- `Pass with Follow-Up`: non-blocking gap captured as explicit future item.
- `Fail`: run fix-and-retry protocol.

## Blocking Findings
- Any architecture, security, or tenant-isolation violation is immediate fail.
