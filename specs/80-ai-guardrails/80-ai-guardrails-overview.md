# AI Guardrails Overview

Status: Draft

## Purpose
Define mandatory guardrails for AI-generated and AI-modified code in this repository.

## Guardrail Scope
These guardrails apply to:
- New code generation
- Existing code modification
- Refactoring that changes behavior, boundaries, or contracts

## Enforcement Model
- `MUST`: mandatory, blocking if violated
- `SHOULD`: expected, requires justification if not followed
- `MAY`: optional, non-blocking

## Mandatory Input Specs
AI agents MUST align outputs with:
- `specs/30-modules/30-mdd.md`
- `specs/40-system/40-sdd.md`
- `specs/60-code-generation/60-ai-code-generation-pipeline.md`
- `specs/70-module-implementation/*.md`

## Baseline Architecture Context
- Modular monolith
- ASP.NET Core Web API
- PostgreSQL + EF Core
- ASP.NET Core Identity (cookie authentication)
- RBAC + policy-based authorization
- OpenTelemetry + structured logs + health checks + audit logging
- Shared database/shared schema with `TenantId` tenant partition key

## Blocking Rule
If a generated change violates architecture boundaries, security rules, or tenant isolation rules, AI agents MUST stop and request clarification or revision before continuing.
