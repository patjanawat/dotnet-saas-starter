# Autonomous Dev Loop Overview

Status: Draft

## Purpose
Define the mandatory autonomous delivery loop AI agents must use to implement this repository safely and incrementally.

## Scope
This loop applies to all AI-driven implementation tasks:
- New feature slices
- Bug fixes
- Refactors with behavior impact

## Operating Principles
- Work in small, safe, testable slices.
- Preserve architecture boundaries and tenant isolation.
- Run self-review before considering a slice complete.
- Retry with bounded fix cycles when validation fails.
- Produce explicit, auditable handoff output.

## Inputs
AI agents MUST align with:
- Foundation, BRD, PRD, MDD, SDD
- AI execution pack documents
- Code generation pipeline docs
- Module implementation packs
- AI guardrails

## Loop Phases
1. Select work slice
2. Execute autonomous task cycle
3. Run self-review protocol
4. Run fix-and-retry protocol if needed
5. Complete and hand off with traceability
