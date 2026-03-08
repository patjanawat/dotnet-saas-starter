# Docker Migrations Runbook

## Purpose
Run EF Core migration operations in a Docker-based local workflow without changing application code.

## Scope
- Create/list/apply migrations for `SaaS.Infrastructure`
- Use existing solution layout and startup project
- Keep commands copy-paste ready

## Procedure
### Prerequisites

```bash
docker compose up -d postgres
dotnet tool restore
```

If `dotnet-ef` is not available:

```bash
dotnet tool install --global dotnet-ef
```

### Ensure DB target is available
Current local settings commonly use host port `5433` and DB `saasdb`.
Confirm container is up:

```bash
docker compose ps
```

### Create migration

```bash
dotnet ef migrations add InitialFoundation -p src/SaaS.Infrastructure/SaaS.Infrastructure.csproj -s src/SaaS.Api/SaaS.Api.csproj -o Persistence/Migrations
```

### List migrations

```bash
dotnet ef migrations list -p src/SaaS.Infrastructure/SaaS.Infrastructure.csproj -s src/SaaS.Api/SaaS.Api.csproj
```

### Apply migration

```bash
dotnet ef database update -p src/SaaS.Infrastructure/SaaS.Infrastructure.csproj -s src/SaaS.Api/SaaS.Api.csproj
```

## Verification

```bash
docker compose logs --tail=200 postgres
dotnet ef migrations list -p src/SaaS.Infrastructure/SaaS.Infrastructure.csproj -s src/SaaS.Api/SaaS.Api.csproj
curl http://localhost:5000/health/ready
```

Expected:
- migration command executes without design-time DbContext errors
- readiness endpoint returns `200` when DB is reachable

## Recovery
If migration command cannot create DbContext:
1. verify startup project path `-s src/SaaS.Api/SaaS.Api.csproj`
2. verify infrastructure project path `-p src/SaaS.Infrastructure/SaaS.Infrastructure.csproj`
3. check `AppDbContextDesignTimeFactory` and appsettings connection string

If DB connection fails:
1. run `docker compose ps`
2. check API/DB logs
3. verify compose DB host/port usage (`Host=postgres` inside container network)

If schema/data is inconsistent:
- follow `docker-data-reset-runbook.md` reset flow, then re-run migration commands.
