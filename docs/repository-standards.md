# Repository Standards

## Scope
Repository-wide engineering defaults for the .NET 10 modular monolith SaaS starter.
This document covers standards only and does not include business/module features.

## Files Covered
- `.editorconfig`
- `.gitignore`
- `Directory.Build.props`
- `Directory.Packages.props`

## Standards

### `.editorconfig`
- Enforces UTF-8, LF line endings, trailing whitespace trim, and final newline.
- Uses space indentation with 2-space default and 4 spaces for C# files.
- Keeps C# style rules practical at `suggestion` level:
  - file-scoped namespaces
  - braces preference
  - simplified `using` statements
  - object/collection initializers
  - collection expressions when type matching is appropriate
- Naming conventions:
  - interfaces use `I` prefix
  - private fields use `_camelCase`
  - async methods use `Async` suffix
- Generated code patterns are marked as generated to reduce analyzer noise.

### `.gitignore`
- Ignores standard .NET build artifacts (`bin/`, `obj/`, `out/`, `artifacts/`, `publish/`).
- Ignores IDE/editor artifacts for Visual Studio, Rider/ReSharper, and VS Code.
- Ignores test and coverage outputs (`TestResults/`, `*.trx`, `*.coverage*`, `coverage/`).
- Ignores local environment and machine-specific files (`.env*`, local appsettings variants, logs, temp/cache files).

### `Directory.Build.props`
- Sets repository defaults:
  - `TargetFramework=net10.0`
  - `Nullable=enable`
  - `ImplicitUsings=enable`
  - analyzers enabled (`EnableNETAnalyzers=true`, `AnalysisLevel=latest-recommended`)
  - `EnforceCodeStyleInBuild=false`
  - `TreatWarningsAsErrors=false` (balanced mode)
  - `LangVersion=latest`
  - deterministic/CI-aware build flags
- Sets `IsPackable=false` for test projects (`IsTestProject == true`).

### `Directory.Packages.props`
- Central package version management is enabled (`ManagePackageVersionsCentrally=true`).
- Transitive pinning is enabled (`CentralPackageTransitivePinningEnabled=true`).
- Package versions are defined centrally to prevent per-project version drift.

## Why These Standards Fit A Balanced Starter
- Enforces consistency and cleanliness across projects and contributors.
- Maintains velocity with suggestion-level style guidance rather than strict failures.
- Supports long-term maintainability and predictable builds without over-engineering.

## Contributor and AI Guidance
- Add or change package versions in `Directory.Packages.props`, not individual project files.
- Keep repository-wide compiler/analyzer behavior in `Directory.Build.props`.
- Follow `.editorconfig` conventions to reduce churn and review noise.
- Update this document when repository standards change.
