# Module Boundaries

## Purpose
Define ownership boundaries for upcoming modules to keep a clean modular monolith.

## Health Module
Responsible for:
- liveness/readiness endpoints
- dependency availability reporting

Does not own:
- tenant/user/auth business rules
- domain workflows

## Tenants Module
Responsible for:
- tenant lifecycle use cases
- tenant-level metadata/config ownership

Does not own:
- user credential/authentication workflows
- global role policy definitions unrelated to tenant lifecycle

## Users Module
Responsible for:
- user profile lifecycle within tenant context
- invitation/onboarding user records (non-auth protocol)

Does not own:
- role policy definitions
- token/session issuance

## Roles Module
Responsible for:
- role assignment/use cases
- role-related authorization mapping at application level

Does not own:
- login/session/token flows
- tenant creation lifecycle

## Auth Module
Responsible for:
- sign-in/sign-out/session/token flows
- identity credential checks

Does not own:
- tenant business lifecycle
- non-auth user profile/domain ownership

## Communication Rules Between Modules
- Communicate via Application layer use cases/contracts.
- Do not read/write another module's internal persistence model directly.
- Avoid exposing internal module entities outside module boundaries.
- Share only stable contracts or explicit application APIs.
