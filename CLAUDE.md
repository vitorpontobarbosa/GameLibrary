# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project overview

ASP.NET Core 8 Web API for managing a personal game collection, backed by SQLite via EF Core. Two projects in the solution: `GameLibrary.Api` (the API) and `GameLibrary.Tests` (xUnit + Moq tests). `gamelibrary-web/` is a separate React + TypeScript SPA (Vite) that consumes the API; it is not part of `GameLibrary.sln`.

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

# Docker (API + frontend together)
docker compose up --build          # api on :8080, web (nginx) on :3000, SQLite in the gamelibrary-data volume
docker build -t gamelibrary-api .  # API image directly
docker run -p 8080:8080 -v gamelibrary-data:/app/data gamelibrary-api

# Frontend (gamelibrary-web/), standalone
cd gamelibrary-web
npm install
npm run dev              # Vite dev server, http://localhost:5173, needs VITE_API_URL (defaults to http://localhost:8080)
npm run build             # tsc -b && vite build -> dist/
npm run lint               # oxlint

# Cypress e2e tests (gamelibrary-web/cypress) — needs api+web already running (e.g. `docker compose up -d`)
cd gamelibrary-web
npm run cypress:open     # interactive runner
npm run e2e                # headless run, CYPRESS_BASE_URL/CYPRESS_API_URL default to :3000/:8080

# k6 load tests: fresh DB up, seed, run, save results, teardown — all in one shot
./scripts/run-k6-tests.sh
# run a single test file instead of everything in GameLibrary.k6/tests/
TEST_FILE=games.js ./scripts/run-k6-tests.sh
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

Games belong to a user via `Game.OwnerId`. `GameService` enforces that only the owner can update/delete/fetch-by-id-as-mine a game (`GetByIdAsync` returns null and `UpdateAsync`/`DeleteAsync` return `false` if the requester isn't the owner, which controllers turn into `NotFound`/`Forbid`). `GetAllAsync` is anonymous and returns every game. Note `GET /api/games/{id}` has no `[Authorize]` attribute but still requires the caller's JWT to match `OwnerId` (unauthenticated calls parse to `userId = 0` and always get `404`) — despite the route looking public, it behaves like an authenticated "fetch my game by id".

### CORS

`Program.cs` registers a `Frontend` CORS policy read from `Cors:AllowedOrigins` in config (`appsettings.json` defaults to `http://localhost:5173` and `http://localhost:3000`, i.e. the Vite dev server and the dockerized frontend). Required because `gamelibrary-web` calls the API directly from the browser on a different origin — without it, every fetch from the SPA fails with a CORS error even though `curl`/server-to-server calls (k6, Swagger) work fine.

### Database

SQLite, connection string from `appsettings.json` (overridden by `ConnectionStrings__DefaultConnection` in Docker, pointing at `/app/data/games.db`). `Program.cs` calls `db.Database.Migrate()` at startup, so the schema is created/updated automatically — migrations no longer need to be applied manually before running. Schema changes go through EF Core migrations in `GameLibrary.Api/Migrations`; when editing `GameContext`/models, regenerate with `dotnet ef migrations add` rather than hand-editing `GameContextModelSnapshot.cs` (a previous hand-edit drift there caused the snapshot to omit the `User` entity even though `InitialCreate` did create the `Users` table).

## Frontend (`gamelibrary-web/`)

React 19 + TypeScript SPA scaffolded with Vite, `react-router-dom` for routing. Structure:

- `src/types.ts` — TS mirrors of the API DTOs (`Game`, `CreateGameRequest`, `UpdateGameRequest`, `LoginRequest`/`RegisterRequest`, `AuthResponse`).
- `src/api/client.ts` — `apiFetch<T>(path, { method, body, token })`: thin `fetch` wrapper, base URL from `VITE_API_URL`, throws `ApiError` (with a message extracted from either a plain-text body or an ASP.NET `ModelState` dict) on non-2xx responses.
- `src/api/auth.ts`, `src/api/games.ts` — one function per endpoint, calling `apiFetch`. Mirrors the endpoint list 1:1 with `GameLibrary.k6/services/*`.
- `src/context/AuthContext.tsx` — holds the JWT in `localStorage` (`gamelibrary.token`) plus React state; decodes the token client-side (no library) to read the email claim for display. `useAuth()` exposes `login`/`register`/`logout`/`isAuthenticated`/`token`/`email`.
- `src/components/ProtectedRoute.tsx` — redirects to `/login` when not authenticated; wraps `/my-games`, `/games/new`, `/games/:id/edit` in `App.tsx`.
- `src/components/GameForm.tsx` — shared create/edit form; always submits the full `CreateGameRequest` shape (the API's partial-update support isn't exercised by the UI, which always has every field populated).
- Routes (`src/App.tsx`): `/` (public games list), `/login`, `/register`, `/my-games`, `/games/new`, `/games/:id/edit`.

`VITE_API_URL` must point somewhere the **browser** can reach (not a Docker service name) — see the CORS note above for why the API needs `Cors:AllowedOrigins` to include whatever origin this ends up serving from.

### Cypress e2e tests (`gamelibrary-web/cypress/`)

Page Object Model: `cypress/pages/*.ts` (one class per page/section — `LoginPage`, `RegisterPage`, `GamesListPage`, `MyGamesPage`, `GameFormPage`, `NavBar`) expose selector/interaction methods only; assertions live in the specs (`cypress/e2e/auth.cy.ts`, `cypress/e2e/games.cy.ts`), not in the page objects. Selectors use `data-cy` attributes added throughout `src/` (e.g. `[data-cy=game-card]`, `[data-cy=login-submit]`) rather than CSS classes or text, so they don't break when styling/copy changes.

Tests don't rely on any seeded state: `cypress/support/commands.ts` adds `cy.apiRegister()` (registers a unique user directly via `POST /api/auth/register`, bypassing the UI) and `cy.loginAs(user)` (writes the JWT to `localStorage` under the same key `AuthContext` uses, then visits `/` so the app picks it up) for fast, isolated test setup; `cy.apiCreateGame(user, overrides)` seeds a game via the API when a test needs one to already exist (edit/delete specs). `cypress.config.ts` reads `CYPRESS_BASE_URL` (default `http://localhost:3000`) and `CYPRESS_API_URL` (default `http://localhost:8080`, exposed as `Cypress.env('apiUrl')`).

## Docker

`Dockerfile` (API) is a multi-stage build (SDK image publishes `GameLibrary.Api`, ASP.NET runtime image serves it) running as a non-root user, listening on `PORT` (default 8080, same variable `Program.cs` already reads). `gamelibrary-web/Dockerfile` is a separate multi-stage build: `npm run build` in a `node` image, then the static `dist/` is served by `nginx` (`gamelibrary-web/nginx.conf` adds an SPA fallback: unknown paths serve `index.html` so client-side routing works on refresh/deep links). `VITE_API_URL` is a **build arg** — baked into the JS bundle at build time, not read at container runtime.

`docker-compose.yml` runs both: `api` on `:8080` (SQLite persisted in the `gamelibrary-data` named volume) and `web` on `:3000` (built with `VITE_API_URL=http://localhost:8080`, i.e. the browser's view of the API — must stay a `localhost`/host-reachable URL, not the Docker-internal service name, since the fetch happens from the user's browser). `.dockerignore` files in both directories exclude `bin/`/`obj/`/`node_modules`/`dist`/local `*.db` from build contexts.

## k6 load tests

`GameLibrary.k6/` follows a service-object pattern, separate from the .NET solution and from `gamelibrary-web` (k6 scripts are plain JS run by the k6 binary, no `.csproj`/`package.json` needed):

- `GameLibrary.k6/config.js` — `BASE_URL` (from `__ENV.BASE_URL`).
- `GameLibrary.k6/helpers/http.js` — `jsonHeaders(token)` builds request headers, injecting `Authorization: Bearer` when a token is passed.
- `GameLibrary.k6/services/authService.js` — `register(payload)`, `login(payload)`.
- `GameLibrary.k6/services/gamesService.js` — `getAll()`, `getById(id, token)`, `getMyGames(token)`, `create(payload, token)`, `update(id, payload, token)`, `remove(id, token)`.
- `GameLibrary.k6/data/usuarios.json`, `GameLibrary.k6/data/games.json` — seed data (games reference their owner by email via a `"owner"` field, matched against a user in `usuarios.json`).
- `GameLibrary.k6/seed.js` — runs once before the test files: registers every user in `usuarios.json`, then creates every game in `games.json` under its owner's token.
- `GameLibrary.k6/tests/*.js` — one file per endpoint (`auth-register.js`, `auth-login.js`, `games-get-all.js`, `games-get-me.js`, `games-get-by-id.js`, `games-create.js`, `games-update.js`, `games-delete.js`). Each uses `thresholds: { checks: ['rate==1.0'] }` so a failed check fails the run's exit code, not just the report.

`docker-compose.k6.yml` spins up the API against a throwaway `k6-db-data` volume (fresh schema via the startup `Database.Migrate()`) and a `k6` container that waits on the API healthcheck, then runs `GameLibrary.k6/run.sh`. That script runs `seed.js`, then every file in `tests/` (or just `$TEST_FILE` if set), writing `<name>-summary.json` and `<name>-raw.json.gz` per file to `GameLibrary.k6/results/` (bind-mounted, gitignored).

`scripts/run-k6-tests.sh` wraps the whole lifecycle — build, up, wait-for-healthy, seed, run, save results, then `down -v` to drop the ephemeral DB volume — in one command; the script's exit code matches the k6 container's.

Note: a failed `check()` in k6 does **not** make the run exit non-zero by itself — only `thresholds` do. Add thresholds to test files if a failing check should fail the script/CI.

## CI (`.github/workflows/master_GameLibrary.yml`)

Runs on every push to `master`, three parallel-ish jobs (there is currently no deploy step — this workflow is the test gate, not a deploy pipeline):

- **`unit-tests`** — `dotnet restore`/`build`/`test` against `GameLibrary.sln`.
- **`k6-tests`** — runs `./scripts/run-k6-tests.sh` (see above), uploads `GameLibrary.k6/results/` as an artifact regardless of outcome.
- **`cypress-tests`** (`needs: unit-tests`) — `docker compose up -d --build` (api + web), polls both with `curl` until ready, then `npm ci` + `npx cypress run` in `gamelibrary-web` against `http://localhost:3000`/`:8080`. Uploads `gamelibrary-web/cypress/screenshots/` on failure; always tears the stack down with `docker compose down -v`.

If a deploy job is added later, gate it with `needs: [unit-tests, k6-tests, cypress-tests]` so it only runs once all three pass.

## Notes

- Some source comments/strings are in Portuguese (validation messages, code comments); keep new user-facing validation messages in Portuguese for consistency.
- `PORT` env var controls the listening port at runtime (`Program.cs`), separate from ASP.NET's usual `ASPNETCORE_URLS`.
