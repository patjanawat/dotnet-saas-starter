# Troubleshooting Runbook

Quick guide for recurring local build/test issues in this repository.

## Golden Rule (NuGet Safety)

- Do not change package versions unless the issue is proven to be package-related.
- Fix environment/cache/restore issues first.
- If package changes are unavoidable:
  - change only one package at a time
  - document why
  - run full restore/build/test before commit

## Fast Triage Order

1. Check SDK and sources.
2. Restore with stable local CLI environment variables.
3. Clean cache/build artifacts.
4. Rebuild a single failing project.
5. Rebuild solution.
6. Only then consider package changes.

## Standard Recovery Commands

Run from repo root:

```powershell
$env:DOTNET_SKIP_FIRST_TIME_EXPERIENCE='1'
$env:DOTNET_CLI_HOME="$PWD\\.dotnet-cli"

dotnet --info
dotnet --list-sdks
dotnet nuget list source

dotnet clean SaaS.Starter.sln
dotnet restore SaaS.Starter.sln -m:1 -p:RestoreDisableParallel=true
dotnet build SaaS.Starter.sln -c Debug -m:1
```

## Common Errors and Fixes

### `NU1301` (cannot reach NuGet)

Cause:
- network/proxy/firewall cannot reach `https://api.nuget.org`

Fix:
- verify internet/proxy settings
- check NuGet source list
- retry restore

### `MSB4067` in `Directory.Packages.props`

Cause:
- malformed XML (often hidden text or bad copy/paste)

Fix:
- open `Directory.Packages.props`
- check near reported line for invalid text between XML elements
- ensure each `<PackageVersion ... />` is on its own valid XML line

### `MSB3021` / `MSB3027` (file lock)

Cause:
- stale `dotnet` process locking DLLs

Fix:

```powershell
Get-Process dotnet | Select-Object Id,ProcessName,MainWindowTitle
Get-Process dotnet | Stop-Process -Force
dotnet build SaaS.Starter.sln -c Debug -m:1
```

### Integration tests fail with `The logger is already frozen`

Cause:
- Serilog bootstrap logger lifecycle conflicts with test host recreation

Fix:
- use `CreateLogger()` for the startup logger path used by tests
- avoid patterns that freeze the same reloadable logger across host lifecycles

### IDE shows many fake compile errors but CLI builds

Cause:
- stale design-time cache

Fix:
1. close IDE
2. delete `.vs/`, project `bin/`, project `obj/`
3. reopen solution
4. run restore/build from CLI first

## Package Change Checklist (Before Editing `Directory.Packages.props`)

All must be true:
- issue reproduces after clean+restore
- issue is not network/cache/SDK related
- proposed version is compatible with target framework (`net10.0`)
- change scope is minimal and intentional
- test impact is verified (`UnitTests` + `IntegrationTests`)

If any item is false, do not modify package versions.
