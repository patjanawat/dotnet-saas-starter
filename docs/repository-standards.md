# Phase 2 Step 2.15: Repository Standards

## Scope
This step standardizes repository-wide engineering defaults for the .NET 10 SaaS starter.
It does not add business logic or module-specific behavior.

## Standards Added

### `.editorconfig`
- Kept formatting defaults simple and consistent across file types.
- Added balanced C# style suggestions (`var` usage, braces, file-scoped namespaces, accessibility modifiers).
- Added naming rules for interfaces and private fields.
- Marked generated C# files as generated code.
- Set analyzer posture to suggestion-level defaults to support AI-assisted development without noisy hard failures.

### `.gitignore`
- Expanded to cover common .NET build outputs (`bin`, `obj`, `artifacts`, `publish`, NuGet package outputs).
- Included IDE/workspace artifacts for Visual Studio, Rider, and VS Code.
- Included test and coverage artifacts (`TestResults`, `*.trx`, coverage files, BenchmarkDotNet artifacts).
- Kept local secrets and machine-local environment files ignored.

### `Directory.Build.props`
- Set shared solution defaults:
  - `TargetFramework=net10.0`
  - `Nullable=enable`
  - `ImplicitUsings=enable`
  - `EnableNETAnalyzers=true`
  - `AnalysisLevel=latest-recommended`
  - `EnforceCodeStyleInBuild=false` (balanced mode)
  - `TreatWarningsAsErrors=false` (balanced mode)
  - `LangVersion=latest`
  - `Deterministic=true`
  - CI-aware deterministic build flag (`ContinuousIntegrationBuild` when `CI=true`)

### `Directory.Packages.props`
- Confirmed central package management is enabled (`ManagePackageVersionsCentrally=true`).
- Enabled transitive package pinning (`CentralPackageTransitivePinningEnabled=true`) for more consistent dependency resolution.

## Why This Matters
- Creates consistent defaults across all projects in the modular monolith.
- Supports production-minded development while keeping friction moderate for iterative AI-assisted workflows.
- Prevents environment-specific artifacts from polluting commits.
- Improves reproducibility and maintainability without over-constraining day-to-day development.
