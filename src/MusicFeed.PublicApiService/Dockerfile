# docker pull codefuller/musicfeed-updates-service:latest
# docker-compose -f "tests/MusicFeed.PublicApiService.IntegrationTests/docker-compose.yaml" up --no-build --detach
# docker build --network=musicfeed-test-network -t musicfeed/api-service:latest -f "src/MusicFeed.PublicApiService/Dockerfile" .
# docker-compose -f "tests/MusicFeed.PublicApiService.IntegrationTests/docker-compose.yaml" down

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /build

COPY ["src/MusicFeed.PublicApiService/MusicFeed.PublicApiService.csproj", "src/MusicFeed.PublicApiService/"]
COPY ["tests/MusicFeed.PublicApiService.IntegrationTests/MusicFeed.PublicApiService.IntegrationTests.csproj", "tests/MusicFeed.PublicApiService.IntegrationTests/"]
COPY ["MusicFeed.PublicApiService.Build.sln", "./"]
COPY ["MusicFeed.PublicApiService.src.ruleset", "./"]
COPY ["MusicFeed.PublicApiService.tests.ruleset", "./"]

RUN dotnet restore "MusicFeed.PublicApiService.Build.sln"

COPY . .

RUN dotnet restore "MusicFeed.PublicApiService.Build.sln"

RUN dotnet build "MusicFeed.PublicApiService.Build.sln" --no-restore --configuration Release

FROM build AS test

# We don't let dotnet test command to fail build if some tests are failed.
# This will prevent publish of tests results (trx file).
# If some tests are failed, the task PublishTestResults will fail due to failTaskOnFailedTests parameter.
RUN dotnet test "MusicFeed.PublicApiService.Build.sln" --no-build --configuration Release --logger "trx" --collect:"XPlat Code Coverage"; exit 0

RUN mkdir TestResults
RUN cp -r --target-directory=TestResults --parents tests/*/TestResults

FROM test AS publish
RUN dotnet publish "src/MusicFeed.PublicApiService/MusicFeed.PublicApiService.csproj" --no-build --configuration Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MusicFeed.PublicApiService.dll"]
