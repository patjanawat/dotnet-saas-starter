# Module Template

## Purpose
Reusable structure for creating a new module in this repository.

## Domain (`SaaS.Domain`)
Include:
- module entities/value objects
- domain rules/invariants
- minimal domain services only when needed

Do not include:
- EF Core mapping attributes/config
- transport models

## Application (`SaaS.Application`)
Include:
- commands/queries per feature slice
- handlers and application-level validation
- abstractions/interfaces needed by infrastructure

Do not include:
- endpoint/middleware code
- direct provider-specific infrastructure code

## API (`SaaS.Api`)
Include:
- endpoint definitions
- request/response models for HTTP boundary
- authorization attributes/policies at endpoint boundary

Do not include:
- business decision logic
- database access logic

## Infrastructure (`SaaS.Infrastructure`) (if needed)
Include:
- persistence/config mappings for module data
- implementations of application abstractions
- external service adapters

Do not include:
- API routing concerns
- domain policy ownership

## Tests
- Unit tests: domain/application rules
- Integration tests: slice behavior via API + persistence wiring
- Keep test names slice-oriented and outcome-based

## Docs
For each module, add:
- module overview (scope + non-goals)
- feature slice notes
- API behavior and verification notes
