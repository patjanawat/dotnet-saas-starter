# Autonomous Change Log Template

Status: Draft

## Purpose
Provide a standard log format for AI slice execution, review, retries, and handoff.

## Change Log Entry Template
- Date/Time:
- Slice ID:
- Requirement IDs:
- Module:
- Scope summary:
- Files changed:
- Tests executed:
- Validation results:
- Self-review outcome:
- Retry count:
- Risks/follow-ups:
- Final status (`Complete` | `Blocked`):

## Usage Rules
1. One entry per completed or blocked slice.
2. Include exact requirement IDs and file paths.
3. Include explicit pass/fail evidence for validation steps.
4. If retries occur, include each retry summary and reason.
5. Store entries in chronological order.

## Example (Placeholder)
- Date/Time: `YYYY-MM-DD HH:MM UTC`
- Slice ID: `SLICE-001`
- Requirement IDs: `FR-002`, `FR-006`
- Module: `User`
- Scope summary: `Add tenant-scoped user profile update validation`
- Files changed: `src/Modules/User/...`
- Tests executed: `Unit: X`, `Integration: Y`, `API: Z`
- Validation results: `All required checks passed`
- Self-review outcome: `Pass`
- Retry count: `1`
- Risks/follow-ups: `None`
- Final status: `Complete`
