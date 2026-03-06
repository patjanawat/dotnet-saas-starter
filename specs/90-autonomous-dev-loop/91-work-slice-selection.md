# Work Slice Selection

Status: Draft

## Purpose
Define how AI agents choose the next implementation slice.

## Slice Selection Rules
1. A slice MUST map to explicit requirement IDs.
2. A slice MUST fit within one primary module boundary when possible.
3. A slice MUST be independently testable.
4. A slice MUST have clear completion criteria before execution starts.
5. Large work MUST be decomposed into smaller sequential slices.

## Slice Size Guidance
- Preferred: one use case or one API behavior change.
- Acceptable: one entity + related command/query + tests.
- Avoid: multi-module broad changes unless contract-first and explicitly approved.

## Priority Order
1. Security and tenant isolation corrections
2. Failing test repairs in in-scope work
3. Core functional requirement slices
4. Observability and hardening follow-ups
5. Non-critical improvements

## Slice Definition Template
- Slice ID:
- Requirement IDs:
- Module:
- In scope:
- Out of scope:
- Expected files:
- Required tests:
- Done criteria:

## Blocking Condition
If requirement mapping or ownership is unclear, AI agent MUST stop and request clarification.
