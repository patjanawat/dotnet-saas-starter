# Local Development (Docker Compose)

## Purpose
Run API + PostgreSQL locally with a simple and stable container setup.

## Dockerfile Design

API container image is defined in:

- `src/SaaS.Api/Dockerfile`

Design choices:

- multi-stage build (`sdk` build/publish stage + `aspnet` runtime stage)
- publishes `SaaS.Api` before runtime image copy
- targets .NET 10 images
- runtime exposes port `8080` inside container

## Services

- `postgres`
  - image: `postgres:16`
  - container: `saas-postgres`
  - host port: `5432`
  - named volume: `postgres-data`
- `api`
  - build: `src/SaaS.Api/Dockerfile`
  - container: `saas-api`
  - host port: `5000` (mapped to container `8080`)

## Configuration Used by Compose

`docker-compose.yml` sets:

- `ASPNETCORE_ENVIRONMENT=Development`
- `Database__UseInMemory=false`
- `ConnectionStrings__DefaultConnection=Host=postgres;Port=5432;Database=saasdb;Username=saas;Password=saas_dev_password`

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
- API should listen on `http://localhost:5000`.

Quick checks:

```powershell
curl http://localhost:5000/health/live
curl http://localhost:5000/health/ready
curl http://localhost:5000/api/foundation/ping
```

Success looks like:

- `postgres` status is `healthy`
- API endpoints return `200` for health/ping
- API logs show normal startup and request logs

## PostgreSQL Connection from Host

For local DB tools:

- Host: `localhost`
- Port: `5432`
- Database: `saasdb`
- User: `saas`
- Password: `saas_dev_password`

## Common Troubleshooting

- Port `5432` or `5000` already in use:
  - stop conflicting service or change host ports in `docker-compose.yml`
- Database connection errors:
  - confirm `Database__UseInMemory=false`
  - confirm connection string host is `postgres` (not `localhost`) for API container
- Clean reset data:

```powershell
docker compose down -v
docker compose up --build
```

## Intentionally Deferred

- CI/container pipeline integration
- Kubernetes or orchestration beyond compose
- production secret management and hardened runtime settings
- automatic migration execution on container startup
