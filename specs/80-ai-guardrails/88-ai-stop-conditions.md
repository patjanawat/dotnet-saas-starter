# AI Stop Conditions

Status: Draft

## Purpose
Define mandatory stop conditions where AI agents must halt generation/modification and request clarification.

## Stop Conditions
1. Conflicting requirements across BRD/PRD/MDD/SDD with no clear precedence resolution.
2. Missing module ownership for a targeted entity or API.
3. Undefined tenant isolation rule for new tenant-owned behavior.
4. Undefined authorization policy for a protected endpoint.
5. Requested change requires breaking architecture guardrails.
6. Requested change introduces cross-module direct data mutation.
7. Requested change cannot satisfy security baseline with available constraints.
8. Migration risk is high and rollback strategy is undefined.

## Stop Procedure
1. Halt implementation before writing unsafe code.
2. Report the exact blocking condition and affected files/modules.
3. Provide minimum clarification questions.
4. Resume only after explicit resolution is documented.

## High-Severity Immediate Stops
- Potential cross-tenant data exposure
- Potential secret/credential leakage
- Potential bypass of authorization controls
- Potential removal of audit logging from privileged/state-changing flow
