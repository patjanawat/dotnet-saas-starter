# Architecture Guardrails

Status: Draft

## Purpose
Prevent architectural drift in the modular monolith.

## Mandatory Rules
1. AI agents MUST preserve module boundaries from MDD.
2. AI agents MUST keep domain logic inside module domain/application layers.
3. AI agents MUST NOT place business rules in controllers/endpoints/infrastructure adapters.
4. AI agents MUST enforce project reference constraints from SDD.
5. AI agents MUST use published contracts for cross-module interaction.
6. AI agents MUST NOT directly mutate another module's entities or tables.

## Project Reference Rules
- Domain MUST NOT reference infrastructure/web/EF packages.
- Application MUST reference only same-module domain + approved shared abstractions.
- API MUST reference same-module application contracts.
- Infrastructure MUST depend inward on application/domain contracts.

## Review Gate
Any direct cross-module entity dependency is a blocking violation.
