# Docker Operations Index

## Purpose
Central index for Docker local development and recovery runbooks for this repository.

## Quick Navigation

| Runbook | What it is for | Who should use it | When to use it | Safety level |
|---|---|---|---|---|
| [Docker Local Dev Runbook](./docker-local-dev-runbook.md) | Daily start/stop/rebuild/logging workflow for API + Postgres | Developers, QA | Normal local work | Mostly safe (non-destructive by default) |
| [Docker Troubleshooting Runbook](./docker-troubleshooting-runbook.md) | Incident-style diagnosis and recovery for Docker/Compose issues | Developers, QA, maintainers | When stack fails to start or connect | Mixed (includes destructive options) |
| [Docker Data Reset Runbook](./docker-data-reset-runbook.md) | Controlled PostgreSQL data reset and recovery | Developers, QA | When local DB state is corrupted/stale | High risk (data-loss options included) |

## Safe vs Destructive Commands

### Safe (no database data deletion)
- `docker compose up --build`
- `docker compose up -d`
- `docker compose stop`
- `docker compose start`
- `docker compose restart`
- `docker compose logs -f`
- `docker compose ps`
- `docker compose down`

### Destructive (database data may be deleted)
- `docker compose down -v`
- `docker volume rm <volume-name>`
- `docker system prune --volumes` (broad cleanup; use with caution)

## Current Compose State Note
- Active file: `docker-compose.yml`
- Current mismatch to be aware of:
  - Postgres image listens on container port `5432`, but compose currently maps `5433:5433` and API container uses `Host=postgres;Port=5433`.
- Minimal safe correction:
  - Change host/container mapping to `"5433:5432"` (or `"5432:5432"`)
  - Change API connection string to `Host=postgres;Port=5432;...`

## How to Use This Index
1. Start with the local-dev runbook for daily workflows.
2. If startup fails, jump to troubleshooting.
3. If DB state is broken, follow the data-reset runbook and apply the minimum reset level first.

## Step Status
- Current step: Docker runbooks index creation
- Files created/updated:
  - `docs/runbooks/docker-operations-index.md`
- Assumptions:
  - Docker Desktop + Compose v2 are installed
  - `docker-compose.yml` is the active compose file
- Verification commands:
  - `docker compose config`
  - `docker compose ps`
- Known gaps / follow-up recommendations:
  - Align compose port/config mismatch described above to avoid DB connectivity confusion
