# Configuration and Options Foundation

## Configuration Sections

`src/SaaS.Api/appsettings.json` and `src/SaaS.Api/appsettings.Development.json` provide a clean starter structure with:

- `ConnectionStrings`
- `Jwt`
- `Email`
- `Logging`

All values are placeholder-safe and development-friendly. No real secrets are committed.

## Strongly Typed Options Classes

Options classes are defined in `src/SaaS.Api/Configuration`:

- `JwtOptions`
  - `Issuer`
  - `Audience`
  - `SigningKey`
  - `AccessTokenMinutes`
- `EmailOptions`
  - `Provider`
  - `FromAddress`
  - `FromName`
  - `Host`
  - `Port`
  - `UseSsl`
  - `Username`
  - `Password`

## Binding and Registration

Binding is centralized in:

- `ConfigurationOptionsRegistrationExtensions.AddSaaSConfigurationOptions(...)`

Registration pattern:

- `AddOptions<JwtOptions>()`
  - binds `Jwt` section
  - uses data annotations validation
  - uses `ValidateOnStart()`
- `AddOptions<EmailOptions>()`
  - binds `Email` section
  - uses data annotations validation
  - adds lightweight port validation rule
  - uses `ValidateOnStart()`

This keeps configuration binding maintainable and easy to extend for later modules.

## Validation Summary

- Data annotations enforce required fields and basic ranges/formats.
- Startup-time validation (`ValidateOnStart`) fails fast on invalid configuration.
- Validation remains intentionally lightweight for starter phase maintainability.

## Verification

Run from repository root:

```powershell
dotnet build
dotnet run --project src/SaaS.Api
```

How to confirm startup still works:

1. Start the API with current placeholders and confirm the host starts without options validation failures.

How to confirm binding is wired correctly:

1. Temporarily set an invalid value (for example a short `Jwt:SigningKey`).
2. Restart the API and confirm startup fails with options validation error.
3. Restore the placeholder and confirm startup succeeds again.

How to inspect placeholder sections safely:

1. Review `appsettings.json` and `appsettings.Development.json` values in source control.
2. Confirm values are non-secret placeholders and environment-local defaults only.

## Intentionally Deferred

- JWT authentication implementation
- Email sending implementation
- Advanced configuration framework/custom providers
- Persistence, Docker, and README expansion steps
