FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY SaaS.Starter.sln ./
COPY Directory.Build.props ./
COPY Directory.Packages.props ./
COPY src/SaaS.Api/SaaS.Api.csproj src/SaaS.Api/
COPY src/SaaS.Application/SaaS.Application.csproj src/SaaS.Application/
COPY src/SaaS.Contracts/SaaS.Contracts.csproj src/SaaS.Contracts/
COPY src/SaaS.Domain/SaaS.Domain.csproj src/SaaS.Domain/
COPY src/SaaS.Infrastructure/SaaS.Infrastructure.csproj src/SaaS.Infrastructure/

RUN dotnet restore src/SaaS.Api/SaaS.Api.csproj

COPY . .
RUN dotnet publish src/SaaS.Api/SaaS.Api.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "SaaS.Api.dll"]
