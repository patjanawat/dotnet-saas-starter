# Persistence Foundation

## Scope
Establish EF Core + PostgreSQL persistence foundation for the starter solution.
Keep the setup minimal, real, and migration-ready.

## What Is Included

- `AppDbContext` in `SaaS.Infrastructure` as the central EF Core context.
- DbContext registration in DI through `AddSaaSInfrastructure()`.
- PostgreSQL wiring via `ConnectionStrings:DefaultConnection`.
- Migration assembly setup for Npgsql registration.
- Design-time factory for EF tools support.

## DbContext and Registration

- DbContext file:
  - `src/SaaS.Infrastructure/Persistence/AppDbContext.cs`
- DI registration:
  - `src/SaaS.Infrastructure/DependencyInjection.cs`

Behavior:
- `Database:UseInMemory=true` uses in-memory provider for local fast loops.
- Otherwise, PostgreSQL provider is used with `ConnectionStrings:DefaultConnection`.

## Design-Time Factory

- File:
  - `src/SaaS.Infrastructure/Persistence/AppDbContextDesignTimeFactory.cs`

Purpose:
- Enables `dotnet ef` commands without requiring runtime host startup.
- Uses appsettings/environment configuration with a safe fallback connection string.

## Migration Commands

Run from repository root.

Create migration:

```powershell
dotnet ef migrations add InitialFoundation -p src/SaaS.Infrastructure/SaaS.Infrastructure.csproj -s src/SaaS.Api/SaaS.Api.csproj -o Persistence/Migrations
```

Apply migration:

```powershell
dotnet ef database update -p src/SaaS.Infrastructure/SaaS.Infrastructure.csproj -s src/SaaS.Api/SaaS.Api.csproj
```

List migrations:

```powershell
dotnet ef migrations list -p src/SaaS.Infrastructure/SaaS.Infrastructure.csproj -s src/SaaS.Api/SaaS.Api.csproj
```

If `dotnet ef` is missing:

```powershell
dotnet tool install --global dotnet-ef
```

## Verification

```powershell
dotnet build SaaS.Starter.sln
dotnet run --project src/SaaS.Api/SaaS.Api.csproj
```

Success looks like:
- Solution builds successfully.
- API starts and can resolve `AppDbContext`.
- EF migration commands can create/list/update migrations against `SaaS.Infrastructure`.



