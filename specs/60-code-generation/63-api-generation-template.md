# API Generation Template

Status: Draft

## Purpose
Define strict rules for AI generation of HTTP APIs in this platform.

## Endpoint Metadata
- Endpoint name:
- Module:
- Route:
- Method:
- Requirement IDs:
- Auth policy:
- Tenant scoped (`Yes/No`):
- Audit required (`Yes/No`):

## Contract Rules
- Route must follow `/api/{module}/{resource}` (or approved versioned form).
- Request/response models must be DTOs.
- Domain entities must not be serialized directly.
- Time values must be UTC ISO 8601.

## Error Rules
- Failures must return `ProblemDetails`.
- Required fields: `type`, `title`, `status`, `detail`, `instance`.
- Required extensions: `traceId`, `errorCode`.
- Validation failures must return `ValidationProblemDetails`.

## Security Rules
- Protected endpoint must declare authorization policy.
- Tenant-scoped endpoint must resolve and validate `TenantId` before business logic.
- Endpoint must reject tenant mismatch between principal and target resource.

## Handler Flow
1. Resolve request context (identity, tenant, trace).
2. Validate request DTO.
3. Check authorization policy.
4. Execute application command/query.
5. Map result to response DTO.
6. Emit audit event for privileged/state-changing operations.
7. Return success or `ProblemDetails` error.

## API Completion Criteria
- Contract tests cover success and error scenarios.
- Unauthorized and forbidden scenarios are tested.
- Tenant isolation is tested for tenant-owned resources.
- Observability fields (`traceId`, `tenantId`) are included in logs.
