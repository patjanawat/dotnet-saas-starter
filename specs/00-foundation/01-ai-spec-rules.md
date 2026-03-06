# AI Specification Authoring Rules

Status: Draft

## Purpose
Define mandatory rules for writing specifications that AI agents can convert into safe, deterministic implementation plans and code.

## 1. Avoid ambiguity
Write requirements with measurable and testable language.

Do not use vague phrases such as:
- "proper access"
- "secure password"
- "handle errors appropriately"

Use explicit statements instead. Example:
- Password minimum length is 10.
- Password must include at least one uppercase letter.
- Password must include at least one number.

## 2. Define explicit data structures
Every spec that introduces data must define:
- Entity name
- Fields with type
- Required vs optional
- Constraints (length, range, format, uniqueness)
- Default values

Example format:
- `User`
- `Id: Guid (required)`
- `Email: string (required, max 320, unique)`
- `TenantId: Guid (required)`

## 3. Define entity ownership
Each entity must have a clear owning module and lifecycle boundary.

Required statements:
- Owner module
- Which modules may read it
- Which modules may mutate it
- Cross-module access method (direct reference, API, domain event)

## 4. Define explicit rules
Business rules must be listed as numbered rules with clear conditions and outcomes.

Required format:
1. Condition
2. Validation rule
3. Action/result
4. Failure result

Avoid narrative-only descriptions for critical behavior.

## 5. Define state machines explicitly
When an entity has lifecycle states, define:
- Allowed states
- Allowed transitions
- Transition triggers
- Guard conditions

Example:
- States: `Pending`, `Active`, `Suspended`, `Deleted`
- Transition: `Pending -> Active` when email is verified
- Transition: `Active -> Suspended` when billing is overdue

## 6. Define error behavior
All failure scenarios must specify:
- Trigger condition
- HTTP status code (for APIs)
- Error code
- User-safe message
- Retry behavior (if any)

No silent failures. No unspecified fallback behavior.

## 7. Naming conventions
Use consistent naming across specs and code generation inputs.

Rules:
- Modules: `PascalCase` nouns (e.g., `Billing`, `Identity`)
- Entities: `PascalCase` singular nouns (e.g., `Tenant`, `Invoice`)
- Fields: `PascalCase` in C# domain models, `camelCase` in JSON payloads
- Commands/queries: verb + noun (e.g., `CreateTenant`, `GetInvoiceById`)
- Database tables: `snake_case` plural unless overridden by repository standard

## 8. API conventions
All HTTP APIs must define route, method, auth requirement, and request/response shape.

Route pattern:
- `/api/{module}/{resource}`

Examples:
- `GET /api/billing/invoices/{id}`
- `POST /api/identity/users`

Minimum API contract fields:
- Method and route
- Request schema
- Success response schema
- Error responses
- Authorization requirement

## 9. Standard error format
API errors must use `ProblemDetails` (RFC 7807 style).

Minimum fields:
- `type`
- `title`
- `status`
- `detail`
- `instance`

Required extension fields:
- `traceId`
- `errorCode`

## 10. Spec completeness checklist
Before implementation, every spec must pass this AI-ready checklist:

- [ ] Scope and intent are explicit and testable
- [ ] Entities and data structures are fully defined
- [ ] Ownership and module boundaries are defined
- [ ] Business rules are numbered and deterministic
- [ ] State transitions are explicitly modeled (if applicable)
- [ ] Error behavior is defined for expected failure cases
- [ ] Naming conventions are applied consistently
- [ ] API contracts follow `/api/{module}/{resource}`
- [ ] API errors use `ProblemDetails` with required fields
- [ ] Security and authorization requirements are explicit
- [ ] Acceptance criteria can be converted directly into tests
- [ ] No ambiguous wording remains
