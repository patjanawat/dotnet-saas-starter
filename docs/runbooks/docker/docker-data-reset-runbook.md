# Docker Data Reset Runbook

## 1. Purpose
Provide controlled procedures to reset local PostgreSQL data in Docker with clear risk levels.

## 2. When to Use This
- Local DB schema/data is corrupted
- Local seed data is inconsistent
- You need a clean local DB for repeatable testing
- Credentials/config changed and old persisted data conflicts

## 3. Warnings
- Some commands in this runbook permanently delete local DB data
- Always choose the lowest reset level that can solve the issue
- If you need to keep data, do not run destructive commands

## 4. Data Reset Levels
### Level 1: Non-destructive restart (safe)
- Restarts containers only
- Keeps volumes/data

### Level 2: Rebuild without deleting volume (mostly safe)
- Rebuilds images and recreates containers
- Keeps DB volume/data

### Level 3: Full volume reset (destructive)
- Deletes containers and volumes
- Removes all local PostgreSQL data

## 5. Commands
### Level 1 - Non-destructive restart

```bash
docker compose restart
```

### Level 2 - Rebuild containers, keep data

```bash
docker compose down
docker compose up --build -d
```

### Level 3 - Full reset (data loss)

```bash
docker compose down -v
docker compose up --build -d
```

Optional explicit volume removal:

```bash
docker volume ls
docker volume rm dotnet-saas-starter_saas_postgres_data
```

Note: actual volume name may include compose project prefix. Confirm with `docker volume ls` first.

## 6. Post-Reset Recovery
1. Wait for postgres healthcheck to become healthy:

```bash
docker compose ps
docker compose logs -f postgres
```

2. Verify API starts:

```bash
docker compose logs -f api
```

3. Verify endpoints:

```bash
curl http://localhost:5000/health/live
curl http://localhost:5000/health/ready
```

4. Verify DB state from container:

```bash
docker compose exec postgres psql -U saas -d saasdb -c "\dt"
```

## 7. Verify Section
Expected after successful reset:
- Postgres container is healthy
- API container is up
- API readiness endpoint is reachable
- New clean DB state is present (or recreated by app startup behavior)

## 8. FAQ
### Why old data still appears?
- You used `docker compose down` without `-v`, so volume was preserved.

### Why credentials appear ignored?
- Postgres env vars initialize only on first DB init. Existing volume keeps previous state.

### Why recreating container did not reset DB?
- Container recreation alone does not delete named volumes.

### Why API still cannot connect after reset?
- Check compose DB host/port wiring. Inside network API should use `Host=postgres;Port=5432`.

## Step Status
- Current step: Docker data reset runbook creation
- Files created/updated:
  - `docs/runbooks/docker/docker-data-reset-runbook.md`
- Assumptions:
  - Local data reset applies to development only
  - Compose-managed named volumes are used
- Verification commands:
  - `docker compose ps`
  - `docker compose logs -f postgres`
  - `curl http://localhost:5000/health/ready`
- Known gaps / follow-up recommendations:
  - Standardize DB port mapping and API connection string to reduce reset-time confusion
