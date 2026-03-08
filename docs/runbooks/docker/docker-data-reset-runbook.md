# Docker Data Reset Runbook

## Purpose
Reset local PostgreSQL data safely when development state is stale or corrupted.

## Scope
- Container restart/rebuild
- Optional full volume reset
- Post-reset validation

## Procedure
### Reset levels
1. Non-destructive restart
2. Rebuild without deleting volume
3. Full volume reset (destructive)

### Level 1: Non-destructive restart

```bash
docker compose restart
```

### Level 2: Rebuild, keep volume

```bash
docker compose down
docker compose up --build -d
```

### Level 3: Full reset (destructive)

```bash
docker compose down -v
docker compose up --build -d
```

Optional explicit volume check/remove:

```bash
docker volume ls
docker volume rm dotnet-saas-starter_saas_postgres_data
```

## Verification

```bash
docker compose ps
docker compose logs --tail=200 postgres
docker compose logs --tail=200 api
curl http://localhost:5000/health/ready
```

Expected:
- postgres healthy
- api up
- readiness endpoint reachable

## Recovery
If old data still appears:
- you likely did not remove the volume

If credentials seem ignored:
- existing volume keeps previous DB initialization state

If API still cannot connect:
- verify compose DB host/port wiring
- inside network use service host `postgres`
- check whether API connection string port matches Postgres container port
