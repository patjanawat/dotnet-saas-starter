# Test Generation Template

Status: Draft

## Purpose
Define how AI agents generate tests for production-grade, spec-traceable implementation.

## Test Plan Metadata
- Feature/module:
- Requirement IDs covered:
- Test owner:
- Test layers in scope:

## Required Test Layers
1. Unit tests
- Domain invariants
- Application handler behavior
- Validation logic

2. Integration tests
- Persistence behavior with EF Core/PostgreSQL semantics
- Cross-layer behavior within a module
- Tenant filter enforcement for tenant-owned entities

3. API contract tests
- Success responses and schema shape
- Error responses using `ProblemDetails`
- AuthN/AuthZ behavior (`401`, `403`) and tenant mismatch behavior

4. Operational tests
- `/health/live` and `/health/ready`
- Audit logging for privileged/state-changing actions

## Mandatory Test Rules
- Every requirement ID in scope must map to at least one test case.
- Every mutating endpoint must have authorization and tenant-isolation tests.
- Every failure path must assert `errorCode` and `traceId` presence in error payload.
- Every state transition must have positive and negative path coverage.

## Test Case Template
- Test ID:
- Requirement ID:
- Type (`Unit/Integration/API/Operational`):
- Precondition:
- Action:
- Expected result:
- Notes:

## Exit Criteria
- Required tests are present and passing.
- No uncovered in-scope requirement IDs remain.
- No critical tenant/security regressions in changed scope.
