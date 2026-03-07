# Local Development (Docker Compose)

## Purpose
Run API + PostgreSQL locally with a simple and stable container setup.

## Services

- `postgres`
  - image: `postgres:16`
  - container: `saas-starter-postgres`
  - port: `5432`
- `api`
  - build: `src/SaaS.Api/Dockerfile`
  - container: `saas-starter-api`
  - port: `8080`

## Configuration Used by Compose

`docker-compose.yml` sets:

- `ASPNETCORE_ENVIRONMENT=Development`
- `Database__UseInMemory=false`
- `ConnectionStrings__DefaultConnection=Host=postgres;Port=5432;Database=saas_starter;Username=saas;Password=saas_dev_password`

This ensures API uses PostgreSQL service from compose network.

## Start the Stack

From repository root:

```powershell
docker compose up --build
```

Run detached:

```powershell
docker compose up --build -d
```

Stop/remove:

```powershell
docker compose down
```

## Verify

When services are up:

- PostgreSQL should be healthy.
- API should listen on `http://localhost:8080`.

Quick checks:

```powershell
curl http://localhost:8080/health/live
curl http://localhost:8080/health/ready
curl http://localhost:8080/api/foundation/ping
```

Success looks like:

- `postgres` status is `healthy`
- API endpoints return `200` for health/ping
- API logs show normal startup and request logs

## Common Troubleshooting

- Port `5432` or `8080` already in use:
  - stop conflicting service or change host ports in `docker-compose.yml`
- Database connection errors:
  - confirm `Database__UseInMemory=false`
  - confirm connection string host is `postgres` (not `localhost`) for API container
- Clean reset data:

```powershell
docker compose down -v
docker compose up --build
```
