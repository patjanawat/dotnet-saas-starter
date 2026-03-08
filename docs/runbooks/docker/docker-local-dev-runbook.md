# Docker Local Dev Runbook

## Purpose
Run the API and PostgreSQL locally with Docker Compose for daily development.

## Scope
- Uses `docker-compose.yml` in repository root
- API container built from `src/SaaS.Api/Dockerfile`
- Covers start/stop/rebuild/log checks and endpoint checks

## Procedure
### Prerequisites

```bash
docker --version
docker compose version
```

### Topology (current repo state)
- Services: `postgres`, `api`
- API port mapping: `5000:8080`
- Postgres port mapping in current compose: `5433:5433`
- Volume: `saas_postgres_data`
- API DB env in current compose: `Host=postgres;Port=5433;Database=saasdb;Username=saas;Password=dev100%`

### Start stack

```bash
docker compose up --build
```

Detached mode:

```bash
docker compose up --build -d
```

Rebuild only API:

```bash
docker compose build api
docker compose up -d api
```

### Day-to-day operations

```bash
docker compose stop
docker compose start
docker compose restart
docker compose down
docker compose logs -f api
docker compose logs -f postgres
docker compose exec api sh
docker compose exec postgres psql -U saas -d saasdb
```

## Verification

```bash
docker compose ps
docker compose logs --tail=200 api
curl http://localhost:5000/health/live
curl http://localhost:5000/health/ready
curl http://localhost:5000/api/foundation/ping
```

Expected:
- `postgres` is healthy
- `api` is up
- endpoints return `200`
- Serilog logs appear in `docker compose logs`

## Recovery
### Common issue: DB connectivity mismatch
Current compose maps and uses Postgres port `5433`. Postgres image default internal port is `5432`.

Minimal safe correction (recommended):
1. Change postgres port mapping to `"5433:5432"` (or `"5432:5432"`)
2. Change API connection string in compose to use `Host=postgres;Port=5432;...`

Then apply:

```bash
docker compose down
docker compose up --build -d
```

### Data-loss warning
Destructive command:

```bash
docker compose down -v
```

This removes `saas_postgres_data` and deletes local DB data.
