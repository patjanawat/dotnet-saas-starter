# Security Guardrails

Status: Draft

## Purpose
Define mandatory security controls for AI-generated code.

## Mandatory Rules
1. Protected endpoints MUST declare explicit authorization policy.
2. Authentication MUST use ASP.NET Core Identity + secure cookie baseline.
3. Authorization MUST enforce RBAC and policy-based decisions.
4. Sensitive data MUST NOT be returned in API responses or logs.
5. Input validation MUST be explicit for all command/query/API inputs.
6. Failure responses MUST use `ProblemDetails` with `traceId` and `errorCode`.

## Cookie Baseline
- `HttpOnly=true`
- `SecurePolicy=Always` for non-local
- `SameSite=Lax` or stricter per deployment policy

## Security Blocking Conditions
- Missing auth policy on protected endpoint
- Direct exposure of credentials/secrets
- Unvalidated external input reaching persistence/business logic
