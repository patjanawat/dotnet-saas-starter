# Assign Role Slice

Status: Draft

## Purpose
Define the first authorization management slice: assign a role to a tenant user membership.

## Specs Used
- `specs/20-product/20-prd.md` (FR-004, FR-006)
- `specs/30-modules/30-mdd.md` (Authorization and User interactions)
- `specs/40-system/40-sdd.md` (RBAC + policy-based auth, tenant checks, error contracts)
- `specs/70-module-implementation/authorization-module-pack.md`
- `specs/70-module-implementation/user-module-pack.md`
- `specs/80-ai-guardrails/82-security-guardrails.md`

## Scope
In scope:
- Assign role to existing tenant user membership.
- Enforce authorization policy for role assignment.
- Validate role existence and tenant consistency.
- Emit audit record for privilege change.

Out of scope:
- Role creation workflow
- Bulk role assignment
- Conditional policy expression editor

## Domain
- Entities involved: `UserMembership`, `Role`, `PolicyBinding`.
- Domain rule: role assignment must be tenant-consistent.
- Domain rule: duplicate active role assignment is not allowed.

## Application
- Implement `AssignUserRoleCommand` + handler.
- Validate actor policy, target membership, and role existence.
- Enforce idempotent behavior for already-assigned role.
- Emit `UserRoleAssigned` event and audit event.

## API
- Endpoint: `POST /api/user/users/{id}/roles`
- Request DTO: role key and tenant context identifier.
- Success response: updated membership role summary.
- Failure response: `ProblemDetails` with `traceId` and `errorCode`.
- Missing role or tenant mismatch returns deterministic domain failure.

## Persistence
- EF Core + PostgreSQL tables:
- `user_memberships`
- `roles`
- `policy_bindings`

Persistence rules:
- Enforce unique membership-role combination for active assignments.
- Ensure tenant-consistent membership and role scope checks before write.
- Persist audit entry in observability storage.

## Tests
- Unit:
- Role assignment validation and idempotency behavior.

- Integration:
- Membership-role persistence behavior.
- Missing role and duplicate assignment failure handling.

- API:
- Authorized assignment success path.
- Forbidden assignment for insufficient privilege.
- `ProblemDetails` contract for failure scenarios.

- Security:
- Ensure cross-tenant role assignment is rejected.

## Acceptance Criteria
1. Authorized actor can assign role to membership within tenant boundary.
2. Duplicate active assignment is handled deterministically (idempotent success or explicit business failure per contract).
3. Missing role or tenant mismatch returns `ProblemDetails` with `traceId` and `errorCode`.
4. Role assignment is audit-logged with actor, target, tenant, and trace context.
