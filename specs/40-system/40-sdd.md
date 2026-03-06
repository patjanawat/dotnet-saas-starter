# System Design Document (SDD)

Status: Draft

## 1. System Overview
This document defines the production-grade technical baseline for a reusable .NET SaaS starter platform.

Architecture baseline:
- Modular monolith deployed as a single application process.
- Multi-tenant model uses shared database and shared schema with `TenantId` on tenant-owned data.
- Primary authentication uses ASP.NET Core Identity with secure cookie authentication.
- Authorization uses RBAC plus policy-based authorization.
- Persistence uses PostgreSQL with Entity Framework Core.
- Observability uses OpenTelemetry, structured logging, health checks, and audit logging.
- Deployment baseline uses Docker.

System design goals:
- Deterministic implementation behavior for AI-assisted code generation.
- Strong tenant isolation and secure-by-default module patterns.
- Clear project and dependency rules that prevent architectural erosion.
- Starter-friendly defaults that can be extended without breaking baseline contracts.

## 2. Solution Structure
Repository conventions:
- Source root: `src/`
- Tests root: `tests/`
- Specs root: `specs/`

Project layout baseline:
- `src/AppHost/` (web host, DI composition root, middleware, endpoint wiring)
- `src/SharedKernel/` (cross-module primitives, base abstractions, shared value objects only)
- `src/Modules/Identity/`
- `src/Modules/Tenant/`
- `src/Modules/User/`
- `src/Modules/Authorization/`
- `src/Modules/Observability/`
- `src/Modules/Notification/`
- `src/Infrastructure/` (EF Core, PostgreSQL integration, external providers, background jobs)

Per-module internal layering:
- `Domain/` (entities, value objects, domain events, invariants)
- `Application/` (commands, queries, handlers, validators, DTOs)
- `Infrastructure/` (repository implementations, provider adapters, integration handlers)
- `Api/` (route registrations, request/response models, endpoint filters)

Project reference rules:
1. Module `Domain` must not reference EF Core, web, or infrastructure packages.
2. Module `Application` may reference only its own `Domain` and `SharedKernel`.
3. Module `Infrastructure` may reference its module `Application` and `Domain`.
4. Module `Api` may reference only its module `Application` contracts.
5. Cross-module references must go through published contracts, never direct entity references.
6. `AppHost` composes module endpoints and DI, but does not contain business rules.
7. `Infrastructure` may provide shared technical services but must not own module business logic.

## 3. Module-to-Project Mapping
- Identity module
- Project set: `Identity.Domain`, `Identity.Application`, `Identity.Infrastructure`, `Identity.Api`
- Owns: credentials/session abstractions, identity integration contracts.

- Tenant module
- Project set: `Tenant.Domain`, `Tenant.Application`, `Tenant.Infrastructure`, `Tenant.Api`
- Owns: tenant lifecycle and tenant configuration.

- User module
- Project set: `User.Domain`, `User.Application`, `User.Infrastructure`, `User.Api`
- Owns: tenant-scoped user profile and membership lifecycle.

- Authorization module
- Project set: `Authorization.Domain`, `Authorization.Application`, `Authorization.Infrastructure`, `Authorization.Api`
- Owns: roles, permissions, policies, and policy evaluation contracts.

- Observability module
- Project set: `Observability.Domain`, `Observability.Application`, `Observability.Infrastructure`, `Observability.Api`
- Owns: audit contracts, telemetry abstractions, and operational visibility contracts.

- Notification module
- Project set: `Notification.Domain`, `Notification.Application`, `Notification.Infrastructure`, `Notification.Api`
- Owns: notification templates, dispatch lifecycle, and delivery status.

## 4. API Design
API conventions:
- Base route pattern: `/api/{module}/{resource}`
- Endpoint naming uses resource-oriented routes and HTTP verbs.
- API versioning starts with `v1` path segment when exposed externally: `/api/v1/{module}/{resource}`.
- All endpoints return JSON.
- Use UTC timestamps in ISO 8601 format.

Request/response rules:
- Request models use explicit DTOs, never domain entities.
- Response models are stable contracts and should not leak internal persistence fields.
- Pagination contract for list endpoints: `page`, `pageSize`, `totalCount`, `items`.

Error contract rules:
- Standard error response must use `ProblemDetails` (RFC 7807 style).
- Required fields: `type`, `title`, `status`, `detail`, `instance`.
- Required extensions: `traceId`, `errorCode`.
- Validation errors use `ValidationProblemDetails` with field-level errors.

Idempotency and concurrency:
- PUT and DELETE operations must be idempotent.
- Mutating operations should support optimistic concurrency where entity versioning exists.

## OpenAPI / Swagger Technical Design
Documentation standard:
- API Documentation must be published using OpenAPI 3.x as the machine-readable contract standard.
- OpenAPI output must be deterministic for the same endpoint set and metadata inputs.

Implementation baseline:
- `Swashbuckle.AspNetCore` is the standard implementation for OpenAPI document generation and Swagger UI hosting.
- Swagger UI is the standard interactive API explorer for internal development and integration validation workflows.

Documentation routes:
- OpenAPI JSON route baseline: `/swagger/{documentName}/swagger.json`.
- Swagger UI route baseline: `/swagger`.
- Document naming baseline starts with `v1` and remains extensible for additional API versions.

Security documentation:
- OpenAPI definitions must include Bearer Authentication using JWT bearer scheme (`Authorization: Bearer <token>`).
- Protected endpoints must declare security requirements so authentication visibility is explicit in Swagger UI.

Error contract documentation:
- Reusable schema components must include `ProblemDetails` and `ValidationProblemDetails`.
- Endpoint metadata must map expected non-success responses to these standard error schemas.

Environment exposure policy:
- Local and approved non-production environments may expose Swagger UI and OpenAPI JSON by default.
- Production exposure of Swagger UI must be explicitly controlled by environment policy and disabled by default unless an approved operational need exists.
- If production exposure is enabled, it must be protected by appropriate access controls.

API Versioning readiness:
- OpenAPI configuration must support multiple documents (for example, `v1`, `v2`) without requiring route convention rewrites.
- Version grouping metadata must be attachable per endpoint to support progressive version rollout.

Endpoint metadata requirements:
- Endpoints must provide operation summary/description metadata.
- Endpoints must declare request body schema, parameter schema, and response contract metadata.
- Endpoints must declare auth requirements and standard error responses as part of contract metadata.

XML comments integration:
- XML documentation comments from API projects should be included in OpenAPI generation to improve operation and schema descriptions.
- Missing XML comments should be treated as documentation quality debt and addressed during module evolution.

Reusable schema components:
- Shared schema components should be centralized for common envelopes and metadata patterns.
- Schema reuse must be preferred over duplicating equivalent request/response definitions across modules.

## 5. Authentication and Authorization Design
Authentication baseline:
- ASP.NET Core Identity is the primary identity system.
- Cookie authentication is the default for web clients.
- Cookie settings:
- `HttpOnly=true`
- `SecurePolicy=Always`
- `SameSite=Lax` or stricter per deployment needs
- Sliding expiration enabled with bounded absolute lifetime.

Authorization baseline:
- RBAC roles are assigned through tenant-scoped memberships.
- Policy-based authorization is mandatory for protected operations.
- Policies may require role, permission claim, and tenant context checks.

Authorization rules:
1. Every protected endpoint must declare an authorization policy.
2. Tenant-scoped endpoints must verify authenticated user membership in target tenant.
3. Platform-level operations must require explicit platform admin policy.
4. Authorization failures must return `403` with `ProblemDetails`.

## 6. Database Design Baseline
Database baseline:
- PostgreSQL is the primary relational datastore.
- EF Core is the persistence ORM and migration mechanism.
- Default model is shared database, shared schema, tenant partition key via `TenantId`.

Tenant isolation rules:
1. All tenant-owned tables must include non-null `TenantId`.
2. Queries on tenant-owned entities must filter by resolved `TenantId`.
3. Unique indexes for tenant-owned data must include `TenantId` when uniqueness is tenant-scoped.
4. Cross-tenant reads/writes are forbidden unless explicitly marked as platform-level operation.
5. Background jobs must provide tenant context before accessing tenant-owned data.

Schema conventions:
- Use `snake_case` table and column naming by default.
- Primary keys use UUID (`uuid`) unless module explicitly requires alternative.
- Timestamps use UTC and include created/updated metadata where applicable.

Migration rules:
- Migrations are code-reviewed artifacts.
- Breaking schema changes require backward-compatibility notes and rollout plan.
- Seed data must be deterministic and environment-safe.

## 7. Application Layer Design
Application layer responsibilities:
- Implement use cases with command/query handlers.
- Enforce validation and authorization preconditions.
- Coordinate domain operations and persistence through abstractions.
- Publish integration events after successful transactional operations.

Pattern baseline:
- Command Query Responsibility Segregation (CQRS) at application layer (not separate services).
- Input validation via explicit validators per command/query.
- Use transaction boundaries per use case for consistency.

Handler conventions:
- One handler per command/query.
- Handlers return typed result contracts, not framework-specific responses.
- Business invariants live in domain model; orchestration lives in application layer.

## 8. Infrastructure Design
Infrastructure responsibilities:
- EF Core DbContext and repository implementations.
- External provider adapters (email/SMS/identity integrations).
- Background processing workers and scheduling infrastructure.
- Caching and distributed coordination primitives when required.

Infrastructure rules:
1. Infrastructure depends inward on application/domain contracts only.
2. Provider-specific SDK usage must be isolated behind interfaces.
3. Retries must use bounded policy with exponential backoff for transient failures.
4. Failures must be logged with correlation identifiers.

Configuration baseline:
- Use strongly typed options for module settings.
- Secrets come from environment or secret manager, never hardcoded.
- Environment-specific overrides must be explicit and minimal.

## 9. Observability and Operations
Observability baseline:
- OpenTelemetry for traces and metrics export.
- Structured logging with consistent fields (`timestamp`, `level`, `message`, `traceId`, `tenantId`, `module`, `operation`).
- Health checks for process and dependency readiness.
- Audit logging for security-sensitive and state-changing operations.

Health endpoint requirements:
- `GET /health/live` returns process liveness only.
- `GET /health/ready` returns readiness including critical dependencies (database, required provider connectivity checks).

Audit logging requirements:
1. Log actor identity, tenant context, action, target resource, outcome, and timestamp.
2. Include `traceId` for correlation with request and telemetry data.
3. Log both success and failure for privileged and state-changing actions.
4. Protect audit logs from unauthorized modification or deletion.

Operational rules:
- Do not expose sensitive payloads in logs.
- Emit metrics for request rate, latency, error rate, and background queue outcomes.
- Define alert thresholds in environment-specific operations config.

## 10. Security Design
Security baseline controls:
- Enforce HTTPS in all non-local environments.
- Use secure cookies and anti-forgery protections for state-changing browser requests.
- Sanitize and validate all external inputs.
- Apply least-privilege access for app credentials and infrastructure identities.

Data protection:
- Protect authentication cookies with ASP.NET Core Data Protection.
- Encrypt sensitive secrets at rest via platform secret management.
- Use parameterized queries (via EF Core) to prevent SQL injection.

Tenant security rules:
1. Tenant context resolution occurs before business logic execution.
2. Tenant ID from route/body must be validated against authenticated principal permissions.
3. System actions without tenant scope must be explicitly marked and policy-protected.

Vulnerability management:
- Dependency updates and security patches are part of standard maintenance.
- Security-relevant incidents must produce audit records and operational alerts.

## 11. Testing Strategy
Test layers:
- Unit tests for domain invariants and application handlers.
- Integration tests for persistence, module interaction, and tenant isolation.
- API contract tests for request/response and `ProblemDetails` error shape.
- End-to-end smoke tests for critical flows (auth, tenant onboarding, role-protected access).

Testing rules:
1. Every functional requirement should map to one or more automated tests.
2. Tenant isolation tests are mandatory for tenant-owned entities.
3. Authorization policy tests are mandatory for protected endpoints.
4. Health endpoints `/health/live` and `/health/ready` must be covered by integration tests.
5. Audit logging behavior must be validated for privileged and mutating actions.

Test data conventions:
- Use deterministic fixtures and tenant-specific test data sets.
- Avoid shared mutable test state across test cases.

## 12. Deployment and Environment Design
Deployment baseline:
- Package application as Docker image.
- Run as containerized service in target environment (local, staging, production).
- Externalize configuration via environment variables and mounted secrets.

Environment definitions:
- Local: developer workstation with local PostgreSQL and optional local telemetry backend.
- Staging: production-like validation environment with realistic integrations.
- Production: hardened environment with managed secrets, monitoring, backup, and alerting.

Container conventions:
- Multi-stage Docker build for reduced image size.
- Run container as non-root user where feasible.
- Include health check probe wiring for `/health/live` and `/health/ready`.

Release rules:
1. Database migrations run in controlled deployment step.
2. Rollback strategy must be defined for each release.
3. Breaking configuration changes require documented migration notes.
