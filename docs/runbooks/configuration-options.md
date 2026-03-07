# Configuration and Options Foundation

## Scope
Add a strongly typed configuration baseline for `SaaS.Api` with practical validation and clean DI binding.

## Configuration Sections

`appsettings.json` and `appsettings.Development.json` include:

- `ConnectionStrings`
- `Jwt`
- `Email`
- `Logging`

All values use safe placeholders for local/dev usage only. No real secrets are committed.

## Options Classes Added

- `JwtOptions` (`Jwt` section)
  - `Issuer`
  - `Audience`
  - `SigningKey` (min length validation)
  - `AccessTokenMinutes` (range validation)
- `EmailOptions` (`Email` section)
  - provider/from/smtp fields
  - basic required/format/range validation

## DI Binding and Validation

`AddSaaSConfigurationOptions()` binds and validates options:

- `AddOptions<JwtOptions>()`
  - bind section `Jwt`
  - data annotations validation
  - `ValidateOnStart()`
- `AddOptions<EmailOptions>()`
  - bind section `Email`
  - data annotations validation
  - lightweight additional rule for valid port
  - `ValidateOnStart()`

This keeps startup behavior predictable and easy to extend.

## Verification

From repository root:

```powershell
$env:DOTNET_SKIP_FIRST_TIME_EXPERIENCE='1'
$env:DOTNET_CLI_HOME="$PWD\\.dotnet-cli"

dotnet build SaaS.Starter.sln
dotnet run --project src/SaaS.Api/SaaS.Api.csproj
```

How to confirm options binding works:

1. Start API with current settings and confirm host starts successfully.
2. Break one required value (for example set `Jwt:SigningKey` to short text).
3. Restart API and confirm startup fails with options validation error.
4. Restore valid placeholder value and confirm startup succeeds again.
