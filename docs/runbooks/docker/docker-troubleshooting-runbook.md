# Docker Troubleshooting Runbook

## 1. Purpose
Provide incident-style triage and recovery for local Docker Compose issues in this repository.

## 2. Symptom-to-Cause Matrix

| Symptom | Likely cause | How to confirm | Recovery |
|---|---|---|---|
| `docker compose up` fails on config/volume errors | Compose syntax issue or undefined volume | `docker compose config` | Fix compose file and rerun |
| Postgres starts but API cannot connect | Wrong DB host/port in API env | `docker compose config` + `docker compose logs api` | Set host to `postgres` and container port to `5432` |
| API uses wrong host/port | `localhost` used inside container network or bad `ConnectionStrings__DefaultConnection` | Inspect env in `docker compose config` | Use `Host=postgres;Port=5432` |
| Postgres healthcheck never healthy | DB init failure, bad credentials, or wrong healthcheck assumptions | `docker compose logs postgres` + `docker inspect` health | Fix env/healthcheck and restart |
| Build fails (Dockerfile path/context) | Wrong build context or Dockerfile path | `docker compose config` + build logs | Ensure `context: .` and `dockerfile: src/SaaS.Api/Dockerfile` |
| Container is up but endpoint unreachable | Port mapping confusion or app startup failure | `docker compose ps` + `docker compose logs api` + curl | Verify `5000:8080`, inspect logs |
| DBeaver connects but API cannot connect | Host mapping works, but in-container connection string is wrong | Check API env in compose | Use service DNS `postgres` in API connection string |

## 3. Triage Workflow
1. Validate compose config:

```bash
docker compose config
```

2. Inspect service status:

```bash
docker compose ps
```

3. Inspect logs:

```bash
docker compose logs --tail=200 postgres
docker compose logs --tail=200 api
```

4. Inspect health:

```bash
docker inspect --format='{{json .State.Health}}' saas-postgres
```

5. Confirm port mappings:

```bash
docker compose port api 8080
docker compose port postgres 5432
```

6. Confirm effective env/config:

```bash
docker compose config
```

## 4. Root-Cause Patterns
- Mismatched volume name references
- Host/container port confusion (`host:container`)
- Wrong in-container DB port (`5433` vs Postgres default `5432`)
- Misuse of `localhost` inside compose network
- Stale containers/images/volumes preserving old state

## 5. Recovery Procedures
Safe restart:

```bash
docker compose restart
```

Rebuild containers:

```bash
docker compose build --no-cache
docker compose up -d
```

Reset only API container:

```bash
docker compose stop api
docker compose rm -f api
docker compose up -d api
```

Reset entire stack (keep data):

```bash
docker compose down
docker compose up --build -d
```

## 6. Destructive Recovery
High-risk reset:

```bash
docker compose down -v
docker compose up --build -d
```

What is lost:
- PostgreSQL data in named volumes (`saas_postgres_data`)
- Any local DB state, seed modifications, and test/dev records

## 7. Verify Section
Run:

```bash
docker compose ps
docker compose logs --tail=200 api
curl http://localhost:5000/health/live
curl http://localhost:5000/health/ready
```

Issue is resolved when:
- Postgres is healthy
- API remains up
- health endpoints respond as expected
- no repeating connection exceptions in API logs

## Step Status
- Current step: Docker troubleshooting runbook creation
- Files created/updated:
  - `docs/runbooks/docker/docker-troubleshooting-runbook.md`
- Assumptions:
  - Local stack is run via `docker compose`
  - Container names from current compose file are used
- Verification commands:
  - `docker compose config`
  - `docker compose ps`
  - `docker compose logs --tail=200 api`
- Known gaps / follow-up recommendations:
  - Update compose DB port wiring to standard container port `5432` for consistency
