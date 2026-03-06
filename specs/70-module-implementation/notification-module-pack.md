# Notification Module Pack

Status: Draft

## Purpose
Define implementation instructions for tenant-aware notification templates and message delivery lifecycle.

Scope:
- Manage notification templates by channel.
- Queue and dispatch notification messages.
- Track delivery outcomes and retry/dead-letter behavior.

## Domain Entities
- `NotificationTemplate`
- Fields: `Id`, `TenantId`, `Channel`, `TemplateKey`, `Subject`, `Body`, `Version`, `Status`
- Rules: unique by (`TenantId`, `Channel`, `TemplateKey`, `Version`)

- `NotificationMessage`
- Fields: `Id`, `TenantId`, `Recipient`, `Channel`, `Payload`, `Status`, `ScheduledAtUtc`, `SentAtUtc`, `FailureCode`
- Rules: status machine (`Queued`, `Processing`, `Sent`, `Failed`, `DeadLettered`)

## Use Cases
- Create/update notification templates.
- Enqueue notification messages from domain events.
- Dispatch messages through provider adapters.
- Retry failed dispatches with bounded policy.

## Commands
- `CreateNotificationTemplateCommand`
- `UpdateNotificationTemplateCommand`
- `QueueNotificationMessageCommand`
- `DispatchNotificationMessageCommand`
- `RetryFailedNotificationCommand`

Command rules:
- Mutations require tenant-admin or platform policy.
- Dispatch/retry flows must emit audit and telemetry events.
- Retry count and transition guards must be enforced.

## Queries
- `GetNotificationTemplateQuery`
- `ListNotificationTemplatesQuery`
- `GetNotificationMessageByIdQuery`
- `ListNotificationMessagesQuery`

Query rules:
- Tenant-scoped queries must enforce `TenantId`.
- Provider internal error details must not leak to public APIs.

## API Endpoints
- `POST /api/notification/templates`
- `PUT /api/notification/templates/{id}`
- `GET /api/notification/templates`
- `POST /api/notification/messages`
- `GET /api/notification/messages/{id}`
- `GET /api/notification/messages`

Endpoint rules:
- Must enforce tenant authorization and scoping.
- Must return `ProblemDetails` with required extensions on failure.
- Must prevent access to cross-tenant template/message data.

## Persistence
- PostgreSQL via Entity Framework Core.
- Tables:
- `notification_templates`
- `notification_messages`

Persistence rules:
- `tenant_id` required and indexed for both tables.
- Add indexes for status and scheduled processing fields.
- Persist provider outcome metadata safely (no secrets).

## Tests
- Unit:
- Message state machine transitions and retry guard behavior.
- Template uniqueness and validation rules.

- Integration:
- Queue-to-dispatch flow with provider adapter stubs.
- Tenant isolation for template/message queries.

- API:
- Template/message endpoint contract tests.
- Failure mapping to `ProblemDetails`.

- Operational:
- Verify retry exhaustion transitions to `DeadLettered`.
