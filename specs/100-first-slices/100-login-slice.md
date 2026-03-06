# Login Slice

Status: Draft

## Purpose
Define the first authentication slice: user login using ASP.NET Core Identity with secure cookie authentication.

## Specs Used
- `specs/20-product/20-prd.md` (FR-004, FR-006)
- `specs/30-modules/30-mdd.md` (Identity, Authorization, Observability interactions)
- `specs/40-system/40-sdd.md` (AuthN/AuthZ baseline, API error contract, observability)
- `specs/70-module-implementation/identity-module-pack.md`
- `specs/80-ai-guardrails/82-security-guardrails.md`

## Scope
In scope:
- Validate login request credentials.
- Sign in valid users using secure cookie auth.
- Return deterministic success/failure API contracts.
- Emit audit/telemetry for login outcomes.

Out of scope:
- Registration flow
- External social identity providers
- MFA

## Domain
- Use ASP.NET Core Identity user model as primary credential source.
- Add `IdentitySessionRecord` persistence for session tracking if required by module pack.
- Domain rule: revoked/expired session is invalid.

## Application
- Implement `SignInIdentityCommand` + handler.
- Validate required inputs (`email/username`, `password`).
- Map known failures to deterministic application result codes.
- Emit audit event for success/failure attempt.

## API
- Endpoint: `POST /api/identity/auth/sign-in`
- Request DTO: login identifier + password.
- Success response: authenticated principal summary DTO.
- Failure response: `ProblemDetails` with `traceId` and `errorCode`.
- Unauthorized credentials must return deterministic failure without leaking secret details.

## Persistence
- EF Core + PostgreSQL.
- Use Identity tables for credential verification.
- Persist session record (`identity_session_records`) when login succeeds (if session tracking enabled).
- Index `session_id` and `user_id`.

## Tests
- Unit:
- Command validation and failure mapping.

- Integration:
- Identity sign-in flow with cookie issuance.
- Session record persistence on success.

- API:
- Success response contract test.
- Invalid credential failure returns `ProblemDetails`.

- Security:
- Ensure response/log does not expose password or secret values.

## Acceptance Criteria
1. Valid credentials return success and create authenticated session context.
2. Invalid credentials return failure using `ProblemDetails` with `traceId` and `errorCode`.
3. Login outcome is audit-logged.
4. Tenant and authorization downstream context can be derived from authenticated principal claims.
