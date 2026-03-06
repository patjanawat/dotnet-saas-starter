# Observability Module Pack

Status: Draft

## Purpose
Define implementation instructions for telemetry, health, and audit capabilities across the platform.

Scope:
- Standardize OpenTelemetry traces/metrics integration.
- Standardize structured logging contracts.
- Manage audit event recording for privileged/state-changing operations.
- Provide operational health endpoint baseline.

## Domain Entities
- `AuditEvent`
- Fields: `Id`, `TenantId`, `ActorId`, `EventType`, `ResourceType`, `ResourceId`, `Outcome`, `OccurredAtUtc`, `TraceId`
- Rules: immutable append-only event records

- `OperationalSignal`
- Fields: `Id`, `SignalType`, `Severity`, `Name`, `Value`, `CapturedAtUtc`, `TraceId`
- Rules: used for operational diagnostics and alerting support

## Use Cases
- Record audit events from module actions.
- Record telemetry markers for critical flows.
- Expose readiness and liveness health checks.
- Correlate logs, traces, and audit events by `TraceId`.

## Commands
- `RecordAuditEventCommand`
- `RecordOperationalSignalCommand`
- `RegisterHealthDependencyCommand`

Command rules:
- Audit events for privileged/state-changing actions are mandatory.
- Command handlers must not block critical business flows on non-critical telemetry failures.
- Failures in observability pipelines must be logged safely.

## Queries
- `GetAuditEventByIdQuery`
- `ListAuditEventsQuery`
- `GetOperationalSignalsQuery`
- `GetHealthStatusQuery`

Query rules:
- Access to audit data must be policy-protected.
- Tenant-scoped audit queries must enforce `TenantId`.

## API Endpoints
- `GET /api/observability/audit-events`
- `GET /api/observability/audit-events/{id}`
- `GET /api/observability/signals`
- `GET /health/live`
- `GET /health/ready`

Endpoint rules:
- `GET /health/live` returns process liveness only.
- `GET /health/ready` validates critical dependencies (for example database reachability).
- API failures must use `ProblemDetails`.

## Persistence
- PostgreSQL via Entity Framework Core for audit persistence.
- Tables:
- `audit_events`
- `operational_signals`

Persistence rules:
- `audit_events` is append-only.
- Index `trace_id`, `tenant_id`, `occurred_at_utc`.
- Protect audit integrity and prohibit normal mutation/deletion paths.

## Tests
- Unit:
- Audit event immutability and validation rules.

- Integration:
- Audit persistence and query behavior with tenant scoping.
- Health readiness behavior with dependency failure simulation.

- API:
- Health endpoint responses for healthy/unhealthy states.
- Audit endpoint authorization and contract behavior.

- Observability:
- Ensure `traceId` correlation appears in logs and audit records.
