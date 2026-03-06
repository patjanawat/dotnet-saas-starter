# User Module Pack

Status: Draft

## Purpose
Define implementation instructions for tenant-scoped user profile and membership management.

Scope:
- Manage user profile lifecycle in tenant context.
- Manage user membership roles per tenant.
- Provide user read models for authorized module consumers.

## Domain Entities
- `User`
- Fields: `Id`, `TenantId`, `Email`, `DisplayName`, `Status`, `CreatedAtUtc`, `UpdatedAtUtc`
- Rules: unique email per tenant

- `UserMembership`
- Fields: `Id`, `UserId`, `TenantId`, `RoleKey`, `Status`, `CreatedAtUtc`
- Rules: unique by (`UserId`, `TenantId`, `RoleKey`) for active memberships

## Use Cases
- Create user under a tenant.
- Update user profile fields.
- Disable/reactivate user access.
- Assign/remove tenant membership role.

## Commands
- `CreateUserCommand`
- `UpdateUserProfileCommand`
- `DisableUserCommand`
- `ReactivateUserCommand`
- `AssignUserRoleCommand`
- `RemoveUserRoleCommand`

Command rules:
- Require tenant context validation before mutation.
- Require authorization policy for membership and status changes.
- Emit audit events for role and status changes.

## Queries
- `GetUserByIdQuery`
- `GetUserByEmailQuery`
- `ListUsersByTenantQuery`
- `GetUserMembershipsQuery`

Query rules:
- Must enforce tenant scoping with `TenantId`.
- Must not return cross-tenant data.

## API Endpoints
- `POST /api/user/users`
- `PUT /api/user/users/{id}`
- `POST /api/user/users/{id}/disable`
- `POST /api/user/users/{id}/reactivate`
- `POST /api/user/users/{id}/roles`
- `DELETE /api/user/users/{id}/roles/{roleKey}`
- `GET /api/user/users/{id}`
- `GET /api/user/users`

Endpoint rules:
- Must validate route/resource tenant ownership.
- Must return standard `ProblemDetails` on failures.
- Must include `traceId` and `errorCode` for errors.

## Persistence
- PostgreSQL via Entity Framework Core.
- Tables:
- `users`
- `user_memberships`

Persistence rules:
- `users.tenant_id` is required and indexed.
- Unique index on (`tenant_id`, `email`).
- Membership foreign keys must enforce tenant-consistent relationships.

## Tests
- Unit:
- User status transition guards.
- Membership uniqueness and validation rules.

- Integration:
- Tenant-scoped query filtering behavior.
- Role assignment/removal persistence behavior.

- API:
- Authorization and tenant mismatch failures.
- Profile and membership endpoint contract tests.

- Security:
- Ensure cross-tenant user access is rejected.
