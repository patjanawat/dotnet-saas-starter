# Project Charter

Status: Draft

## 1. Project Overview

This project defines a spec-driven .NET SaaS starter that provides a reliable, secure, and extensible baseline for multi-tenant business applications.  
The starter prioritizes clear boundaries, predictable architecture decisions, and implementation patterns that AI agents and human developers can follow consistently.

## 2. System Type

The system is a multi-tenant SaaS backend platform delivered as a web API application with supporting infrastructure concerns (identity, authorization, observability, security, and deployment baseline).

## 3. Architecture Style

The architecture style is a **Modular Monolith**.  
Core business capabilities are split into internal modules with explicit boundaries, while deployment remains a single application unit in early phases.

## 4. Technology Stack

- Backend framework: **ASP.NET Core Web API**
- Language: **C#**
- Database: **PostgreSQL**
- ORM: **Entity Framework Core**
- Testing: **xUnit**
- Containerization: **Docker**
- Observability instrumentation: **OpenTelemetry**
- Logging approach: **Structured logging**

## 5. Tenancy Model

The tenancy model for MVP is:

- Shared database
- Shared schema
- Tenant isolation via `TenantId` column

All tenant-scoped domain records must include `TenantId`, and application logic must enforce tenant filtering by default.

## 6. Authentication Strategy

Authentication baseline:

- **ASP.NET Core Identity** for user identity management
- **Secure cookie authentication** for first-party application access
- **JWT support** planned later for public API scenarios (not core MVP auth path)

## 7. Authorization Strategy

Authorization baseline combines:

- **RBAC (Role-Based Access Control)** for role assignment and coarse-grained permissions
- **Policy-based authorization** in ASP.NET Core for fine-grained access rules

Authorization checks must be enforced at API boundaries and, where needed, at service-level business operations.

## 8. Observability

Observability baseline includes:

- OpenTelemetry-based telemetry collection
- Structured application logs
- Correlation-friendly trace context across requests

The objective is to support troubleshooting, performance analysis, and operational visibility from the first implementation phase.

## 9. Security Baseline

Security controls required in MVP:

- Password hashing
- Secure cookies
- Request validation
- Rate limiting
- Audit logging

These controls are mandatory baseline requirements and must be included in implementation and review criteria.

## 10. Quality Standards

Quality baseline for MVP:

- Unit tests
- Integration tests

All modules should maintain test coverage for core logic and key API flows. New features are expected to include appropriate automated tests.

## 11. Deployment Model

Deployment baseline:

- Docker containers
- Environment separation for **dev**, **staging**, and **prod**

Configuration and operational settings must support environment-specific behavior without code changes.

## 12. Out-of-Scope (MVP)

The following are out of scope for MVP unless explicitly added by a later phase:

- Microservices decomposition
- Event-driven distributed architecture
- Multi-database or per-tenant database isolation strategies
- Full public API program and production-grade JWT ecosystem
- Advanced compliance certifications and enterprise-only controls beyond the baseline above
