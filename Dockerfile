## Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY GameLibrary.sln ./
COPY GameLibrary.Api/GameLibrary.Api.csproj GameLibrary.Api/
COPY GameLibrary.Tests/GameLibrary.Tests.csproj GameLibrary.Tests/
RUN dotnet restore GameLibrary.Api/GameLibrary.Api.csproj

COPY GameLibrary.Api/ GameLibrary.Api/
RUN dotnet publish GameLibrary.Api/GameLibrary.Api.csproj -c Release -o /app/publish --no-restore

## Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

RUN apt-get update \
    && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/* \
    && useradd --uid 1000 --create-home appuser \
    && mkdir -p /app/data \
    && chown -R appuser:appuser /app

COPY --from=build --chown=appuser:appuser /app/publish .

USER appuser

ENV PORT=8080 \
    ConnectionStrings__DefaultConnection="Data Source=/app/data/games.db"

EXPOSE 8080

ENTRYPOINT ["dotnet", "GameLibrary.Api.dll"]
