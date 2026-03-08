# Docker Operations Index

## Purpose
Index for all Docker operational runbooks in this repository.

## Scope
Covers local Docker workflows for API + PostgreSQL using `docker compose`.

## Procedure
Use this order:

1. Daily workflow: [Docker Local Dev Runbook](./docker-local-dev-runbook.md)
2. DB reset workflow: [Docker Data Reset Runbook](./docker-data-reset-runbook.md)
3. EF migration workflow: [Docker Migrations Runbook](./docker-migrations-runbook.md)
4. Incident workflow: [Docker Troubleshooting Runbook](./docker-troubleshooting-runbook.md)

Quick navigation:

| Runbook | Use when | Audience | Safety |
|---|---|---|---|
| [docker-local-dev-runbook.md](./docker-local-dev-runbook.md) | Start/stop/rebuild stack | Dev/QA | Safe by default |
| [docker-data-reset-runbook.md](./docker-data-reset-runbook.md) | Need clean DB state | Dev/QA | Can be destructive |
| [docker-migrations-runbook.md](./docker-migrations-runbook.md) | Create/apply EF migrations | Dev | Mostly safe |
| [docker-troubleshooting-runbook.md](./docker-troubleshooting-runbook.md) | Stack is failing | Dev/QA/Maintainer | Mixed |

## Verification

```bash
docker compose config
```

Expected: compose config renders without errors.

## Recovery
If links/paths break after doc moves:

1. Run `git grep "docker-.*runbook" docs`
2. Update links to relative paths from `docs/runbooks/docker/`
3. Re-run `docker compose config` and key commands from local-dev runbook.
