# Observability Guardrails

Status: Draft

## Purpose
Ensure operational visibility and auditability for generated code.

## Mandatory Rules
1. Structured logs MUST include `traceId` and relevant context fields.
2. Privileged and state-changing operations MUST emit audit events.
3. Health endpoints MUST remain implemented and stable:
- `GET /health/live`
- `GET /health/ready`
4. Critical execution paths SHOULD emit OpenTelemetry traces/metrics.
5. Sensitive values MUST be redacted from logs and telemetry.

## Audit Requirements
- Include actor, action, target resource, tenant context, outcome, timestamp.
- Audit records MUST be append-only in normal runtime paths.

## Health Requirements
- `/health/live` checks process liveness only.
- `/health/ready` checks critical dependencies (for example PostgreSQL connectivity).

## Blocking Conditions
- Removal of audit logging from privileged/state-changing path
- Broken or missing `/health/live` or `/health/ready`
