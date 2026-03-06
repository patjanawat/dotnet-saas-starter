# Invite User Slice

Status: Draft

## Purpose
Define the first user onboarding slice: invite a user into a tenant and create initial membership context.

## Specs Used
- `specs/20-product/20-prd.md` (FR-003, FR-004, FR-006)
- `specs/30-modules/30-mdd.md` (User and Tenant module interaction)
- `specs/40-system/40-sdd.md` (tenant isolation, authz, API error contract)
- `specs/70-module-implementation/user-module-pack.md`
- `specs/80-ai-guardrails/83-multi-tenant-guardrails.md`

## Scope
In scope:
- Create invited user record scoped to target tenant.
- Create initial user membership record.
- Enforce tenant admin/platform admin authorization.
- Emit invitation audit event.

Out of scope:
- Email delivery workflow implementation
- Invitation token acceptance UI flow
- Full identity registration completion flow

## Domain
- Entities: `User`, `UserMembership`.
- Domain rule: email must be unique per tenant.
- Domain rule: invited user starts in `Invited` status.

## Application
- Implement `CreateUserCommand` for invitation scenario.
- Validate tenant context and authorization before mutation.
- Create user + default membership atomically.
- Emit `UserInvited` event for downstream notification integration.

## API
- Endpoint: `POST /api/user/users`
- Request DTO: tenant identifier, email, display name, initial role key.
- Success response: user invitation summary DTO.
- Failure response: `ProblemDetails` with `traceId` and `errorCode`.
- Tenant mismatch or missing permission returns deterministic forbidden failure.

## Persistence
- EF Core + PostgreSQL tables:
- `users`
- `user_memberships`

Persistence rules:
- `users` unique index on (`tenant_id`, `email`).
- Required non-null `tenant_id` on both tables.
- Membership FK consistency with tenant-scoped user.

## Tests
- Unit:
- Invitation status initialization and uniqueness validation.

- Integration:
- User + membership atomic persistence.
- Duplicate tenant-email rejection.

- API:
- Tenant admin invite success path.
- Unauthorized/forbidden and tenant mismatch paths.
- `ProblemDetails` contract validation.

- Security:
- Ensure no cross-tenant invitation is accepted.

## Acceptance Criteria
1. Authorized tenant admin can invite user in their tenant.
2. Invited user is created with `Invited` status and tenant-scoped membership.
3. Duplicate email in same tenant is rejected with deterministic `ProblemDetails`.
4. Invitation action is audit-logged and emits integration event.
