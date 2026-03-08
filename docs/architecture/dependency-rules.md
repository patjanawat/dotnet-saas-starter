# Dependency Rules

## Purpose
Define allowed and forbidden project references to prevent architecture drift.

## Allowed Dependencies
- `SaaS.Api` -> `SaaS.Application`, `SaaS.Infrastructure`, `SaaS.Contracts`
- `SaaS.Application` -> `SaaS.Domain`, `SaaS.Contracts`
- `SaaS.Infrastructure` -> `SaaS.Application`, `SaaS.Domain`
- `SaaS.Domain` -> none
- `SaaS.Contracts` -> none

## Forbidden Dependencies
- `SaaS.Domain` -> `SaaS.Infrastructure`
- `SaaS.Domain` -> `SaaS.Api`
- `SaaS.Domain` -> `SaaS.Application`
- `SaaS.Application` -> `SaaS.Api`
- `SaaS.Infrastructure` -> `SaaS.Api`
- `SaaS.Contracts` -> any project

## Additional Rules
- API models stay in API/contracts, not Domain.
- EF Core implementation stays in Infrastructure.
- Domain must remain framework-light and persistence-agnostic.
- Cross-module calls should happen through application handlers/services, not direct internal object access.

## Review Checklist
Before adding a reference:
1. Does it match allowed directions above?
2. Can this be inverted through an abstraction in Application?
3. Does this leak transport/persistence details into inner layers?
