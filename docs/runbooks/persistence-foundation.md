# Persistence Foundation

## What Persistence Pieces Exist

The EF Core persistence foundation is implemented in `SaaS.Infrastructure` with PostgreSQL support:

- `AppDbContext` in `src/SaaS.Infrastructure/Persistence/AppDbContext.cs`
- DbContext registration in `src/SaaS.Infrastructure/DependencyInjection.cs`
- PostgreSQL provider registration via `UseNpgsql(...)`
- In-memory provider toggle for local/dev loops (`Database:UseInMemory`)
- Design-time factory for EF tooling:
  - `src/SaaS.Infrastructure/Persistence/AppDbContextDesignTimeFactory.cs`

## DbContext

`AppDbContext` is the central EF Core context for the starter and is configured in Infrastructure.

- Base type: `IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>`
- Includes starter DbSets and Fluent API mappings in Infrastructure
- Keeps EF details out of API/Application/Domain orchestration layers

## Registration and Connection String

DbContext registration is centralized in `AddSaaSInfrastructure(...)`.

Connection configuration:

- Primary connection: `ConnectionStrings:DefaultConnection`
- Provider: PostgreSQL (`Npgsql.EntityFrameworkCore.PostgreSQL`)
- Migration assembly: `SaaS.Infrastructure` assembly

Configuration source:

- `src/SaaS.Api/appsettings.json`
- `src/SaaS.Api/appsettings.Development.json`

## Migration Readiness

The solution is prepared for EF Core migrations through the design-time factory and Npgsql setup.

Example command to add first migration:

```powershell
dotnet ef migrations add InitialFoundation -p src/SaaS.Infrastructure/SaaS.Infrastructure.csproj -s src/SaaS.Api/SaaS.Api.csproj -o Persistence/Migrations
```

Example command to apply migration:

```powershell
dotnet ef database update -p src/SaaS.Infrastructure/SaaS.Infrastructure.csproj -s src/SaaS.Api/SaaS.Api.csproj
```

## Verification

```powershell
dotnet build
dotnet run --project src/SaaS.Api
```

How to confirm DbContext wiring is correct:

1. API starts without DI errors for `AppDbContext`.
2. Health/readiness endpoint paths that resolve DbContext can execute.
3. `dotnet ef migrations add ...` can discover `AppDbContext` successfully.

Persistence foundation readiness indicators:

- DbContext can be resolved by the host
- EF tooling can create migrations
- PostgreSQL provider is configured from `ConnectionStrings:DefaultConnection`

## Intentionally Deferred

- Repository abstractions for business modules
- Full Auth/User/Tenant modeling decisions for future module phases
- Advanced data access patterns and performance tuning
- Docker/database orchestration workflow documentation
