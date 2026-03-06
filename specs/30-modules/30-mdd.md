# Module Design Document (MDD)

Status: Draft

## Module Overview
This document defines the modular monolith structure for a reusable ASP.NET Core SaaS backend platform.

Design principles:
- Modules represent domain boundaries and own their data and business rules.
- Cross-module interaction is explicit through contracts (application service calls, integration events, or read models).
- Multi-tenant safety is a default behavior, not an optional feature.
- Specifications are written to be deterministic so AI agents can generate module scaffolds, entities, handlers, and tests.

## Module List
- Identity
- Tenant
- User
- Authorization
- Observability
- Notification

## Module Responsibilities
- Identity
- Authenticate principals and manage credential lifecycle abstractions.
- Issue and validate identity claims used by downstream modules.
- Expose identity session/context integration points.

- Tenant
- Manage tenant lifecycle (create, activate, suspend, archive).
- Provide tenant configuration and tenant context metadata.
- Enforce tenant-level status checks for platform access.

- User
- Manage tenant-scoped user profiles and user lifecycle.
- Link users to tenant memberships and profile preferences.
- Provide user query/read models for other modules through defined contracts.

- Authorization
- Evaluate access policies based on role, permission, and context.
- Enforce policy checks for protected operations.
- Provide centralized policy registration and evaluation APIs.

- Observability
- Standardize logs, metrics, traces, and audit event contracts.
- Capture operation telemetry and diagnostic correlation (`traceId`).
- Provide health and readiness contract surfaces for operational checks.

- Notification
- Manage notification intents, templates, channels, and delivery status.
- Dispatch tenant-aware notifications through pluggable providers.
- Provide retry and dead-letter behavior contracts for failed delivery attempts.

## Domain Entities
- Identity module
- `IdentityCredential` (id, principalId, credentialType, status, createdAt, updatedAt)
- `IdentitySession` (id, principalId, issuedAt, expiresAt, revokedAt)

- Tenant module
- `Tenant` (id, code, name, status, createdAt, activatedAt, suspendedAt)
- `TenantConfiguration` (id, tenantId, key, value, updatedAt)

- User module
- `User` (id, tenantId, email, displayName, status, createdAt, updatedAt)
- `UserMembership` (id, userId, tenantId, roleKey, status, createdAt)

- Authorization module
- `Role` (id, key, name, scope, isSystem)
- `Permission` (id, key, resource, action)
- `PolicyBinding` (id, roleId, permissionId, conditionExpression)

- Observability module
- `AuditEvent` (id, tenantId, actorId, eventType, resourceType, resourceId, occurredAt, traceId)
- `TelemetryRecord` (id, traceId, spanName, severity, timestamp)

- Notification module
- `NotificationTemplate` (id, tenantId, channel, templateKey, subject, body, version, status)
- `NotificationMessage` (id, tenantId, recipient, channel, payload, status, scheduledAt, sentAt, failureCode)

## Data Contracts
- Contract naming
- Commands: `VerbNounCommand` (example: `CreateTenantCommand`)
- Queries: `GetNounQuery` (example: `GetUserByIdQuery`)
- Events: `NounPastTenseEvent` (example: `TenantActivatedEvent`)

- API contract baseline
- Route pattern: `/api/{module}/{resource}`
- Error shape: RFC 7807 `ProblemDetails` with `traceId` and `errorCode`
- Tenant-aware endpoints must require tenant context resolution before execution.

- Cross-module contract examples
- `TenantActivatedEvent` published by Tenant, consumed by User and Notification.
- `UserCreatedEvent` published by User, consumed by Notification.
- `AuthorizationPolicyEvaluated` telemetry event emitted to Observability.

## Module Interactions
Interaction rules:
1. A module may read another module only through published contract surfaces.
2. A module may not mutate another module's entities directly.
3. Side effects across modules should be handled via integration events or explicit application service contracts.
4. Tenant context must be propagated in all cross-module requests and events.

Primary interaction paths:
- Identity -> Authorization: identity claims are evaluated against policies.
- Tenant -> User: tenant lifecycle controls user activation eligibility.
- User -> Authorization: membership role assignments determine effective permissions.
- All modules -> Observability: emit audit and telemetry records for traceability.
- Tenant/User -> Notification: business events trigger notification workflows.

## State Machines
- Tenant state machine
- States: `Pending`, `Active`, `Suspended`, `Archived`
- Transitions:
- `Pending -> Active` when onboarding checks pass.
- `Active -> Suspended` when policy or billing gate fails.
- `Suspended -> Active` when suspension conditions are cleared.
- `Suspended -> Archived` when archival action is approved.

- User state machine
- States: `Invited`, `Active`, `Disabled`, `Deleted`
- Transitions:
- `Invited -> Active` when invitation is accepted and identity is verified.
- `Active -> Disabled` when access is revoked.
- `Disabled -> Active` when reinstated by authorized admin.
- `Disabled -> Deleted` when data retention policy allows removal.

- Notification message state machine
- States: `Queued`, `Processing`, `Sent`, `Failed`, `DeadLettered`
- Transitions:
- `Queued -> Processing` when worker accepts the job.
- `Processing -> Sent` when provider returns success.
- `Processing -> Failed` when provider returns failure.
- `Failed -> Processing` on retry attempt within policy limits.
- `Failed -> DeadLettered` when retry limit is exceeded.

## Module Boundaries
- Identity owns authentication concerns and credential/session abstractions only.
- Tenant owns tenant lifecycle and tenant configuration only.
- User owns user profile and membership lifecycle only.
- Authorization owns role/permission/policy evaluation only.
- Observability owns telemetry and audit event models only.
- Notification owns notification templates, queueing, and delivery lifecycle only.

Boundary enforcement requirements:
- No shared mutable entity classes across modules.
- No direct database table updates outside owner module.
- All boundary crossings must use contracts declared in this document.

## Module Ownership
- Identity module owner: Platform Security team
- Tenant module owner: Core Platform team
- User module owner: Identity and Access team
- Authorization module owner: Platform Security team
- Observability module owner: Platform Operations team
- Notification module owner: Communication Platform team

Ownership responsibilities per module:
- Maintain entity model and invariants.
- Approve contract changes and versioning strategy.
- Define acceptance tests for module behavior.
- Review AI-generated changes for boundary compliance.

## Future Module Extensions
- Billing: subscription lifecycle, plan enforcement, invoicing contracts.
- FeatureFlags: tenant and user level feature gating with rollout rules.
- Files: tenant-scoped document storage and metadata lifecycle.
- Workflow: long-running business processes and orchestration contracts.
- Webhooks: outbound event delivery with signature and retry policies.
