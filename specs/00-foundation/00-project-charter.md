# SaaS.Starter Project Charter

Status: Draft

## 1. Project Overview
SaaS.Starter is a spec-driven .NET SaaS starter repository. Markdown specifications in `/specs` are the source of truth for system behavior, architecture, and execution planning. This charter defines the technical baseline for consistent implementation by humans and AI agents.

## 2. System Type
Multi-tenant SaaS application starter focused on business web APIs and internal application workflows.

## 3. Architecture Style
Modular Monolith.

- Single deployable backend with clear internal module boundaries.
- Separation of concerns across Domain, Application, Infrastructure, and API layers.
- Module boundaries are enforced through project structure and dependency direction.

## 4. Technology Stack
- Backend: ASP.NET Core Web API
- Language: C#
- Database: PostgreSQL
- ORM: Entity Framework Core
- Testing: xUnit
- Containerization: Docker
- Observability: OpenTelemetry
- Logging: Structured logging

## 5. Tenancy Model
Shared database, shared schema, tenant isolation by `TenantId` column.

Baseline rules:
- Tenant-scoped data must include `TenantId`.
- Queries and commands must enforce tenant scoping by default.
- Cross-tenant access is prohibited unless explicitly approved and audited.

## 6. Authentication Strategy
- Primary approach: ASP.NET Core Identity
- Session model: secure cookie authentication
- Future extension: JWT support for public APIs (not MVP baseline)

## 7. Authorization Strategy
- Role-Based Access Control (RBAC)
- Policy-based authorization using ASP.NET Core authorization policies

Baseline rules:
- Endpoints and application actions require explicit authorization decisions.
- Authorization policies should be centralized and reusable.

## 8. Observability
OpenTelemetry-first baseline for telemetry instrumentation.

Minimum expectations:
- Distributed traces for request and critical workflow paths
- Core service metrics for health and performance
- Structured logs correlated with trace/context identifiers

## 9. Security Baseline
Security controls required from the foundation phase:
- Password hashing
- Secure cookies
- Request validation
- Rate limiting
- Audit logging

## 10. Quality Standards
Minimum quality gates:
- Unit tests for domain and application logic
- Integration tests for API and infrastructure behavior

## 11. Deployment Model
- Containerized deployment using Docker
- Environment strategy: `dev`, `staging`, `prod`
- Configuration must be environment-aware and externalized

## 12. Out-of-Scope (MVP)
The following are explicitly out of scope for MVP baseline:
- Microservices decomposition
- Multi-database tenancy strategies (database-per-tenant or schema-per-tenant)
- Full JWT-based ecosystem for all clients
- Advanced distributed infrastructure beyond modular monolith needs
