# Common Commands

คำสั่งที่ใช้บ่อยสำหรับพัฒนาและตรวจสอบโปรเจกต์นี้ (รันจาก root ของ repository)

## Setup

```powershell
dotnet --info
dotnet restore SaaS.Starter.sln
dotnet build SaaS.Starter.sln
```

## Verification Commands

รันตามลำดับเพื่อยืนยัน baseline:

```powershell
$env:DOTNET_SKIP_FIRST_TIME_EXPERIENCE='1'
$env:DOTNET_CLI_HOME="$PWD\\.dotnet-cli"

dotnet --version
dotnet restore SaaS.Starter.sln -m:1 -p:RestoreDisableParallel=true
dotnet build SaaS.Starter.sln -c Debug -m:1
dotnet test tests/SaaS.UnitTests/SaaS.UnitTests.csproj -c Debug --no-build
dotnet test tests/SaaS.IntegrationTests/SaaS.IntegrationTests.csproj -c Debug --no-build
```

ตรวจเฉพาะโปรเจกต์ API:

```powershell
dotnet build src/SaaS.Api/SaaS.Api.csproj -c Debug -m:1
```

## Project Reference Verification

ใช้ตรวจ dependency graph ตาม Clean Architecture:

```powershell
dotnet build SaaS.Starter.sln

dotnet list src/SaaS.Api/SaaS.Api.csproj reference
dotnet list src/SaaS.Application/SaaS.Application.csproj reference
dotnet list src/SaaS.Infrastructure/SaaS.Infrastructure.csproj reference
dotnet list src/SaaS.Domain/SaaS.Domain.csproj reference
dotnet list src/SaaS.Contracts/SaaS.Contracts.csproj reference
dotnet list tests/SaaS.UnitTests/SaaS.UnitTests.csproj reference
dotnet list tests/SaaS.IntegrationTests/SaaS.IntegrationTests.csproj reference
```

## Run API

```powershell
dotnet run --project src/SaaS.Api/SaaS.Api.csproj
```

## Testing

รันทั้งหมด:

```powershell
dotnet test SaaS.Starter.sln
```

รันแยกตามชุด:

```powershell
dotnet test tests/SaaS.UnitTests/SaaS.UnitTests.csproj
dotnet test tests/SaaS.IntegrationTests/SaaS.IntegrationTests.csproj
```

รันเฉพาะ test เดียว:

```powershell
dotnet test tests/SaaS.IntegrationTests/SaaS.IntegrationTests.csproj --filter "FullyQualifiedName~AuthorizationSliceTests.AssignRole_WithPlatformAdmin_ReturnsOk"
```

## Clean + Rebuild

```powershell
dotnet clean SaaS.Starter.sln
dotnet restore SaaS.Starter.sln
dotnet build SaaS.Starter.sln
```

## Smoke Endpoints

```powershell
curl http://localhost:5207/health/live
curl http://localhost:5207/health/ready
curl http://localhost:5207/api/foundation/ping
curl http://localhost:5207/api/foundation/throw
```

## Docker

```powershell
docker compose up --build
docker compose down
```

## Git (Daily)

```powershell
git status --short
git add -A
git commit -m "type: short message"
git push origin main
```

## Troubleshooting Quick Fixes

```powershell
dotnet nuget list source
dotnet --list-sdks
```

หากเจอ `NU1301`:
- ตรวจสอบ internet/proxy/firewall ให้เข้าถึง `https://api.nuget.org` ได้
- จากนั้นรัน `dotnet restore` ใหม่

หากเจอ `MSB3021` / `MSB3027` (ไฟล์ DLL ถูก lock):

```powershell
Get-Process dotnet | Select-Object Id,ProcessName,MainWindowTitle
Get-Process dotnet | Stop-Process -Force
dotnet build SaaS.Starter.sln -c Debug -m:1
```

หากเจอ `CS1591` (Missing XML comment):
- สาเหตุ: มี `public` type/member ใน `SaaS.Api` ที่ยังไม่มี `///` comment ขณะเปิด `GenerateDocumentationFile`
- ผลกระทบ: เป็น warning เอกสาร ไม่ใช่ runtime error
- แนวทางที่แนะนำ: เพิ่ม XML comments ให้ request/response contracts และ endpoints

ตรวจเฉพาะ warning นี้:

```powershell
dotnet build src/SaaS.Api/SaaS.Api.csproj -c Debug -m:1 -v minimal
```
