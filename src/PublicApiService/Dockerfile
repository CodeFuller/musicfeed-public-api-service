FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build

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
