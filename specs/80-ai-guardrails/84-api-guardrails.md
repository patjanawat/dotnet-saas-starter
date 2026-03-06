# API Guardrails

Status: Draft

## Purpose
Standardize API generation and modification behavior.

## Mandatory Rules
1. Routes MUST follow `/api/{module}/{resource}` (or approved versioned form).
2. Request/response contracts MUST use DTOs, never domain entities.
3. API errors MUST use `ProblemDetails` contract.
4. Validation failures MUST use `ValidationProblemDetails`.
5. Protected endpoints MUST enforce explicit auth policy.
6. Tenant-scoped endpoints MUST enforce tenant context validation.

## Error Contract Requirements
- Required fields: `type`, `title`, `status`, `detail`, `instance`
- Required extensions: `traceId`, `errorCode`

## API Review Gate
- Missing `ProblemDetails` contract mapping is blocking.
- Missing auth/tenant checks on protected tenant-scoped endpoint is blocking.
