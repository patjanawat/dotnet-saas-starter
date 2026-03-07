# Docker Local Dev Runbook

## 1. Purpose
Provide a practical, copy-paste workflow to run API + PostgreSQL locally using Docker Compose v2.

## 2. Scope
- Covers local development stack only
- Covers startup, rebuild, logs, health checks, and basic diagnostics
- Does not cover CI/CD, Kubernetes, or production deployment

## 3. Prerequisites
- Docker Desktop installed and running
- Repository cloned locally
- Run from repository root where `docker-compose.yml` exists

Check tools:

```bash
docker --version
docker compose version
```

Expected: both commands return version info without error.

## 4. Project Docker Topology
Source files:
- Compose: `docker-compose.yml`
- API image build: `src/SaaS.Api/Dockerfile`

Services:
- `postgres` (`postgres:16`)
- `api` (built from `src/SaaS.Api/Dockerfile`)

Ports (current compose state):
- API: host `5000` -> container `8080`
- Postgres: currently configured as `5433:5433` (see gotcha/correction below)

Volumes:
- `saas_postgres_data` -> `/var/lib/postgresql/data`

Network assumptions:
- Services use the default compose network
- API reaches DB by service name `postgres` (not `localhost`)

## 5. Startup Flow
Start stack (foreground):

```bash
docker compose up --build
```

Start stack (detached):

```bash
docker compose up --build -d
```

Rebuild API image and recreate:

```bash
docker compose build api
docker compose up -d api
```

## 6. Verification Flow
Check service status:

```bash
docker compose ps
```

Tail logs:

```bash
docker compose logs -f
```

Health verification:
- `postgres` should become `healthy` (compose healthcheck)
- API container should be `Up`

Endpoint verification:

```bash
curl http://localhost:5000/health/live
curl http://localhost:5000/health/ready
curl http://localhost:5000/api/foundation/ping
```

DB connection verification from API behavior:
- `/health/ready` returns `200` when API can connect to DB
- `/health/ready` returns `503` with ProblemDetails when DB is unavailable

## 7. Day-to-Day Commands
Start existing containers:

```bash
docker compose start
```

Stop containers:

```bash
docker compose stop
```

Restart containers:

```bash
docker compose restart
```

Bring down stack (keep volume/data):

```bash
docker compose down
```

Rebuild all:

```bash
docker compose build --no-cache
docker compose up -d
```

Tail specific service logs:

```bash
docker compose logs -f api
docker compose logs -f postgres
```

Inspect effective compose config:

```bash
docker compose config
```

Exec into containers:

```bash
docker compose exec api sh
docker compose exec postgres psql -U saas -d saasdb
```

## 8. Common Gotchas
- Host vs container port confusion:
  - API is exposed on host `5000`, not `8080`
- Service name vs localhost:
  - Inside compose network, API must use `Host=postgres`, not `Host=localhost`
- Volume persistence:
  - `docker compose down` does not delete DB data
- Healthcheck ordering:
  - `depends_on: condition: service_healthy` waits for healthcheck, not full app readiness

Current mismatch and minimal safe correction:
- Current compose maps Postgres `5433:5433` and API uses `Port=5433`
- Postgres container typically listens on `5432`
- Minimal safe correction:
  1. Change compose port mapping to `"5433:5432"` (or `"5432:5432"`)
  2. Change API compose env connection string to `Host=postgres;Port=5432;Database=saasdb;Username=saas;Password=dev100%`

## 9. Expected Outputs
Healthy state usually looks like:
- `docker compose ps` shows `postgres` as `healthy`
- `api` service is `Up`
- API logs include Serilog startup lines and request logs

Successful API checks:
- `/health/live` -> `200`
- `/health/ready` -> `200`
- `/api/foundation/ping` -> `200`

## 10. Safety Notes
Commands with data-loss risk:
- `docker compose down -v`
- `docker volume rm saas_postgres_data`

Use these only when you intentionally want to reset local DB state.

## 11. Verify Section
Run in order:

```bash
docker compose up --build -d
docker compose ps
docker compose logs -f api
curl http://localhost:5000/health/live
curl http://localhost:5000/health/ready
curl http://localhost:5000/api/foundation/ping
```

Success criteria:
- Containers stay up
- Postgres reports healthy
- API endpoints return expected statuses

## Step Status
- Current step: Docker local-dev runbook creation
- Files created/updated:
  - `docs/runbooks/docker-local-dev-runbook.md`
- Assumptions:
  - Docker Desktop and Compose v2 are available
  - `docker-compose.yml` is current source of truth
- Verification commands:
  - `docker compose up --build -d`
  - `docker compose ps`
  - `docker compose logs -f api`
- Known gaps / follow-up recommendations:
  - Apply compose port/connection-string correction to remove Postgres port mismatch
