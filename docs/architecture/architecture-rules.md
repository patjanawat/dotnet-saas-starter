# Architecture Rules

## Purpose
Define non-negotiable architecture guardrails for this .NET 10 SaaS starter.

## Architecture Style
- Modular Monolith
- Clean Architecture layering
- Vertical feature slices inside module boundaries

## Layer Responsibilities

### `SaaS.Api`
Owns:
- HTTP endpoints, request/response contracts at API boundary
- authentication/authorization wiring
- middleware pipeline, ProblemDetails, logging integration
- composition root and DI wiring orchestration

Must stay out:
- business decision logic
- persistence/query logic
- domain invariants

### `SaaS.Application`
Owns:
- use cases (commands/queries)
- orchestration of domain and infrastructure-facing abstractions
- application policies, validation at use-case boundary
- DTOs/contracts for application flow (not transport-specific API models)

Must stay out:
- HTTP concerns
- EF Core implementation details
- framework-heavy infrastructure concerns

### `SaaS.Domain`
Owns:
- core business model and invariants
- entities/value objects/domain rules
- domain events (if introduced later)

Must stay out:
- EF Core attributes/configuration
- HTTP/serialization concerns
- external service calls

### `SaaS.Infrastructure`
Owns:
- EF Core DbContext and mappings
- persistence implementations
- identity/email/other external integrations
- concrete adapters for application abstractions

Must stay out:
- API endpoint decisions
- direct ownership of business rules that belong in domain/application

## General Guardrails
- Prefer explicit module boundaries over shared "misc" utilities.
- Keep dependencies inward (toward Domain/Application abstractions).
- Add complexity only when a concrete need appears.
- Avoid cross-module internal data access; use application contracts/use cases.
