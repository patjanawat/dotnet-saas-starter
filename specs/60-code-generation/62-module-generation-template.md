# Module Generation Template

Status: Draft

## Purpose
Provide a strict template for AI generation of module code in the modular monolith.

## Module Metadata
- Module name:
- Owner team:
- Source requirements (IDs):
- In-scope entities:
- In-scope commands/queries:
- In-scope events:

## Generation Steps
1. Define domain model
- Create entities, value objects, and invariants.
- Mark tenant-owned entities with required `TenantId`.
- Define domain events for state transitions.

2. Define application contracts
- Create command/query DTOs and handlers.
- Add validators for all command/query inputs.
- Enforce policy and tenant preconditions before mutation.

3. Define API surface
- Create endpoint mappings using `/api/{module}/{resource}`.
- Map API requests/responses to application contracts only.
- Standardize errors with `ProblemDetails`.

4. Define infrastructure adapters
- Add repository implementations and EF mappings.
- Add provider adapters behind interfaces.
- Add transactional and retry behavior as required.

5. Add observability hooks
- Add structured logs with `traceId` and `tenantId` when available.
- Add audit logging for privileged/state-changing actions.

## Module Rules
- No direct cross-module entity references.
- No business rules in API or infrastructure classes.
- No persistence types exposed outside infrastructure.
- No endpoint without explicit auth policy decision.
- No tenant-owned operation without tenant context validation.

## Required Deliverables
- Domain files
- Application files
- API files
- Infrastructure files
- Tests mapped to requirement IDs
- Traceability notes
