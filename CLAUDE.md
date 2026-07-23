# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project overview

ASP.NET Core 8 Web API for managing a personal game collection, backed by SQLite via EF Core. Two projects in the solution: `GameLibrary.Api` (the API) and `GameLibrary.Tests` (xUnit + Moq tests).

## Commands

```bash
# Restore & build
dotnet build

# Run the API (listens on PORT env var, defaults to 8080; Swagger UI at /swagger)
dotnet run --project GameLibrary.Api

# Run all tests
dotnet test

# Run a single test class or method
dotnet test --filter "FullyQualifiedName~GameServiceTests"
dotnet test --filter "FullyQualifiedName~AuthServiceTests.Login_ComCredenciaisValidas_DeveRetornarToken"

# Apply/create EF Core migrations (dotnet-ef is a local tool, see .config/dotnet-tools.json)
dotnet tool restore
dotnet ef database update --project GameLibrary.Api
dotnet ef migrations add <Name> --project GameLibrary.Api

# Docker
docker compose up --build          # build & run, SQLite file persisted in the gamelibrary-data volume
docker build -t gamelibrary-api .  # build image directly
docker run -p 8080:8080 -v gamelibrary-data:/app/data gamelibrary-api
```

There is no separate lint step; rely on `dotnet build` warnings.

If you don't have the .NET SDK installed locally, run `dotnet-ef`/`dotnet build`/`dotnet test` inside the SDK container instead, e.g.:
```bash
docker run --rm -v "$PWD":/src -w /src/GameLibrary.Api mcr.microsoft.com/dotnet/sdk:8.0 \
  bash -c "dotnet tool restore && dotnet ef migrations add <Name> --project ."
```

## Architecture

Standard layered structure, one layer per folder in `GameLibrary.Api`:

- **Controllers** (`AuthController`, `GamesController`) — thin, only handle HTTP concerns, model-state validation, and pulling `ClaimTypes.NameIdentifier` out of the JWT to get the current user id. No business logic.
- **Services** (`AuthService`, `GameService`) — contain all business/validation logic (e.g. min-length checks, ownership checks) and throw `ArgumentException` for validation failures, which controllers catch and translate into 400/401 responses. `GameService` depends on `IGameRepository`; `AuthService` talks to `GameContext` directly (no repository layer for users).
- **Repositories** (`IGameRepository` / `GameRepository`) — EF Core data access for `Game` only.
- **Data** (`GameContext`) — `DbContext` with `DbSet<Game>` and `DbSet<User>`.
- **Models** — EF entities (`Game`, `User`).
- **DTOs/Games**, **DTOs/Auth** — request/response contracts, decorated with `DataAnnotations` validation attributes (`[Required]`, `[MinLength]`, `[Range]`, `[Url]`, etc.). Note `GameService` re-validates several of these fields itself, so validation exists at both the DTO and service level — keep both in sync when adding fields.
- **Mappings** (`GameProfile`, `AuthProfile`) — AutoMapper profiles, registered in `Program.cs` via `AddAutoMapper(typeof(Program))`.

### Auth model

JWT bearer auth (`Microsoft.AspNetCore.Authentication.JwtBearer`). Config comes from the `Jwt` section in `appsettings.json` (`Key`/`Issuer`/`Audience`) and is required at startup — missing config throws in `Program.cs`. Tokens carry `ClaimTypes.NameIdentifier` (user id) and `ClaimTypes.Email`, expire after 1 hour. Passwords are stored and compared as plain text in `User`/`AuthService` — there is no hashing.

### Ownership rules

Games belong to a user via `Game.OwnerId`. `GameService` enforces that only the owner can update/delete/fetch-by-id-as-mine a game (`GetByIdAsync` returns null and `UpdateAsync`/`DeleteAsync` return `false` if the requester isn't the owner, which controllers turn into `NotFound`/`Forbid`). `GetAllAsync` is anonymous and returns every game.

### Database

SQLite, connection string from `appsettings.json` (overridden by `ConnectionStrings__DefaultConnection` in Docker, pointing at `/app/data/games.db`). `Program.cs` calls `db.Database.Migrate()` at startup, so the schema is created/updated automatically — migrations no longer need to be applied manually before running. Schema changes go through EF Core migrations in `GameLibrary.Api/Migrations`; when editing `GameContext`/models, regenerate with `dotnet ef migrations add` rather than hand-editing `GameContextModelSnapshot.cs` (a previous hand-edit drift there caused the snapshot to omit the `User` entity even though `InitialCreate` did create the `Users` table).

## Docker

`Dockerfile` is a multi-stage build (SDK image publishes `GameLibrary.Api`, ASP.NET runtime image serves it) running as a non-root user, listening on `PORT` (default 8080, same variable `Program.cs` already reads). `docker-compose.yml` maps port 8080 and persists the SQLite file in a `gamelibrary-data` named volume mounted at `/app/data`. `.dockerignore` excludes `bin/`/`obj/`/local `*.db` files from the build context.

## Deployment

`.github/workflows/master_GameLibrary.yml` builds, tests, publishes, and deploys to an Azure Web App named `GameLibrary` on every push to `master`, using the `AZUREAPPSERVICE_PUBLISHPROFILE` secret. This is a non-containerized (`dotnet publish`) deploy, independent of the Dockerfile.

## Notes

- Some source comments/strings are in Portuguese (validation messages, code comments); keep new user-facing validation messages in Portuguese for consistency.
- `PORT` env var controls the listening port at runtime (`Program.cs`), separate from ASP.NET's usual `ASPNETCORE_URLS`.
