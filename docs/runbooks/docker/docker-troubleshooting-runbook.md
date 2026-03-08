# Docker Troubleshooting Runbook

## Purpose
Diagnose and recover Docker Compose issues for local API + PostgreSQL stack.

## Scope
- Compose config and runtime failures
- Startup failures, healthcheck failures, and connectivity issues
- Safe and destructive recovery options

## Procedure
### Symptom-to-cause matrix

| Symptom | Likely cause | Check | Fix |
|---|---|---|---|
| `docker compose up` fails parsing config | compose syntax/path issue | `docker compose config` | fix compose file |
| API up but DB calls fail | wrong DB host/port env | `docker compose config` + api logs | use `Host=postgres` and correct port |
| Postgres never healthy | init/auth/startup issue | postgres logs + inspect health | fix env or volume state |
| Build fails for API | wrong Dockerfile path/context | compose config + build output | keep `context: .`, `dockerfile: src/SaaS.Api/Dockerfile` |
| Endpoint unreachable | port mapping confusion | `docker compose ps`, `docker compose port` | verify `5000:8080` |

### Triage commands

```bash
docker compose config
docker compose ps
docker compose logs --tail=200 postgres
docker compose logs --tail=200 api
docker inspect --format='{{json .State.Health}}' saas-postgres
docker compose port api 8080
```

### Root-cause patterns in this repo
- host/container port confusion
- using `localhost` inside container network
- Postgres internal port mismatch (`5433` configured vs typical `5432`)
- stale volumes preserving old DB state

## Verification

```bash
docker compose ps
curl http://localhost:5000/health/live
curl http://localhost:5000/health/ready
```

Expected:
- postgres healthy
- api up
- readiness endpoint returns expected status

## Recovery
### Safe restart

```bash
docker compose restart
```

### Rebuild stack (keep data)

```bash
docker compose down
docker compose up --build -d
```

### Reset only API container

```bash
docker compose stop api
docker compose rm -f api
docker compose up -d api
```

### Destructive reset (data loss)

```bash
docker compose down -v
docker compose up --build -d
```

Data lost: PostgreSQL data in `saas_postgres_data`.
