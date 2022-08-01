# docker pull codefuller/musicfeed-updates-service:latest
# docker-compose -f "../../tests/PublicApiService.IntegrationTests/docker-compose.yml" up --no-build --detach
# docker build --network=musicfeed-test-network -t musicfeed/api-service:latest -f "Dockerfile" ../..
# docker-compose -f "../../tests/PublicApiService.IntegrationTests/docker-compose.yml" down

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

COPY ["src/PublicApiService/PublicApiService.csproj", "/project/src/PublicApiService/"]
COPY ["tests/PublicApiService.IntegrationTests/PublicApiService.IntegrationTests.csproj", "/project/tests/PublicApiService.IntegrationTests/"]
COPY ["PublicApiService.sln", "/project/"]
RUN dotnet restore "/project/PublicApiService.sln"

COPY . "/project/"

RUN dotnet build "/project/PublicApiService.sln" --no-restore -c Release

RUN dotnet test "/project/PublicApiService.sln" --no-build -c Release --logger "trx;LogFileName=PublicApiService.trx" 

FROM build AS publish
RUN dotnet publish "/project/src/PublicApiService/PublicApiService.csproj" --no-restore -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish /app

ENTRYPOINT ["dotnet", "PublicApiService.dll"]
