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
