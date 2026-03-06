# Identity Module Pack

Status: Draft

## Purpose
Define implementation instructions for identity integration capabilities used by the platform.

Scope:
- Integrate ASP.NET Core Identity as primary authentication system.
- Provide identity context contracts for other modules.
- Enforce secure cookie authentication baseline and sign-in/session lifecycle behavior.

## Domain Entities
- `IdentityProviderLink`
- Fields: `Id`, `UserId`, `Provider`, `ProviderSubject`, `CreatedAtUtc`
- Rules: unique by (`Provider`, `ProviderSubject`)

- `IdentitySessionRecord`
- Fields: `Id`, `UserId`, `SessionId`, `IssuedAtUtc`, `ExpiresAtUtc`, `RevokedAtUtc`
- Rules: revoked sessions are invalid for authentication

## Use Cases
- Register user credentials through ASP.NET Core Identity.
- Authenticate user and issue secure authentication cookie.
- Sign out and revoke active session records.
- Resolve authenticated principal context for downstream modules.

## Commands
- `RegisterIdentityCommand`
- `SignInIdentityCommand`
- `SignOutIdentityCommand`
- `RevokeSessionCommand`

Command rules:
- Validate required fields.
- Return explicit failure with `ProblemDetails` contract mapping.
- Emit audit event for sign-in/sign-out/session revocation outcomes.

## Queries
- `GetIdentityByUserIdQuery`
- `GetSessionBySessionIdQuery`
- `GetIdentityProviderLinksQuery`

Query rules:
- Must not leak credential secrets.
- Must return DTO projections only.

## API Endpoints
- `POST /api/identity/auth/register`
- `POST /api/identity/auth/sign-in`
- `POST /api/identity/auth/sign-out`
- `GET /api/identity/auth/session`

Endpoint rules:
- Use DTO request/response contracts.
- Failure responses must be `ProblemDetails` with `traceId` and `errorCode`.
- Cookie settings must be secure (`HttpOnly`, `Secure`, controlled `SameSite`).

## Persistence
- PostgreSQL via Entity Framework Core.
- Tables:
- `identity_provider_links`
- `identity_session_records`

Persistence rules:
- Use UTC timestamps.
- Index `user_id`, `session_id`, and provider subject keys.
- Keep ASP.NET Core Identity tables managed through Identity EF mappings.

## Tests
- Unit:
- Session validity and revocation rules.

- Integration:
- Sign-in/sign-out flow with cookie authentication.
- Session revocation persistence behavior.

- API:
- Success/failure contract validation.
- Unauthorized/forbidden response behavior.

- Security:
- Ensure secrets are never returned in API DTOs.
