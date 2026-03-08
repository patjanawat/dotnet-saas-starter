# AI Module Generation Guide

## Purpose
Guide AI agents to generate new modules/slices without violating architecture.

## Generation Order (Required)
1. Confirm module boundary and non-goals.
2. Add/adjust Domain types and invariants.
3. Add Application command/query + handler + abstractions.
4. Add Infrastructure implementations/mappings (only if needed).
5. Add API endpoint/request/response wiring.
6. Add or update tests.
7. Update docs.

## Rules AI Must Follow
- Respect dependency rules in `docs/architecture/dependency-rules.md`.
- Keep business logic out of API.
- Keep EF Core and provider specifics in Infrastructure.
- Keep Domain free of transport/persistence concerns.
- Keep changes slice-scoped and minimal.

## What Not To Generate Unless Explicitly Requested
- broad abstractions/framework layers
- cross-module refactors
- unrelated migrations
- CI/CD pipeline changes
- production deployment artifacts

## Drift Prevention Checklist
Before finalizing AI output:
1. Are project references still valid per dependency rules?
2. Did any layer receive concerns it should not own?
3. Are module boundaries respected (no internal leakage)?
4. Are non-goals still excluded?
5. Are docs/tests updated only for this slice?

## Output Expectations for AI Tasks
- concise implementation summary
- explicit verification steps
- clear list of files changed
- known limitations and follow-up suggestions
