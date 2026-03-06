# Authorization Module Pack

Status: Draft

## Purpose
Define implementation instructions for RBAC and policy-based authorization behavior.

Scope:
- Manage role/permission models.
- Evaluate policy requirements for protected operations.
- Provide authorization decision contracts for API/application layers.

## Domain Entities
- `Role`
- Fields: `Id`, `Key`, `Name`, `Scope`, `IsSystem`
- Rules: unique `Key`

- `Permission`
- Fields: `Id`, `Key`, `Resource`, `Action`
- Rules: unique by (`Resource`, `Action`)

- `PolicyBinding`
- Fields: `Id`, `RoleId`, `PermissionId`, `ConditionExpression`
- Rules: unique by (`RoleId`, `PermissionId`)

## Use Cases
- Define platform and tenant roles.
- Bind permissions to roles.
- Evaluate access decision for operation and resource context.
- Audit policy evaluation outcomes for sensitive operations.

## Commands
- `CreateRoleCommand`
- `CreatePermissionCommand`
- `BindPermissionToRoleCommand`
- `UnbindPermissionFromRoleCommand`
- `EvaluateAuthorizationCommand`

Command rules:
- Role/permission mutations require high-privilege policy.
- Evaluation command must be deterministic and auditable.
- Changes must emit audit logs.

## Queries
- `GetRoleByKeyQuery`
- `ListRolesQuery`
- `GetPermissionByKeyQuery`
- `ListPermissionsQuery`
- `GetRolePermissionsQuery`

Query rules:
- Must return stable DTO contracts.
- Must include scope information for role interpretation.

## API Endpoints
- `POST /api/authorization/roles`
- `GET /api/authorization/roles`
- `POST /api/authorization/permissions`
- `GET /api/authorization/permissions`
- `POST /api/authorization/roles/{roleKey}/permissions`
- `DELETE /api/authorization/roles/{roleKey}/permissions/{permissionKey}`

Endpoint rules:
- Must enforce policy protection for all mutation endpoints.
- Must return `ProblemDetails` for policy violations and validation failures.
- Must log authorization model changes via audit logging.

## Persistence
- PostgreSQL via Entity Framework Core.
- Tables:
- `roles`
- `permissions`
- `policy_bindings`

Persistence rules:
- Add unique indexes for keys and binding uniqueness.
- Enforce FK constraints for role-permission relationships.
- Keep seed baseline deterministic for default system roles/permissions.

## Tests
- Unit:
- Permission binding rules and evaluation logic.

- Integration:
- Role/permission persistence and uniqueness constraints.
- Policy evaluation behavior by scope and context.

- API:
- Forbidden mutation behavior for insufficient privilege.
- Model mutation contract tests and error responses.

- Security:
- Verify protected endpoints deny access without required policy.
