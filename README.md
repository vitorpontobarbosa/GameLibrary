# GameLibrary

Simple ASP.NET Core API for managing a game collection.

## Requirements

- .NET 8 SDK

## Build

Restore dependencies and build the solution:

```bash
dotnet build
```

## Database

The API uses SQLite. The connection string is defined in `GameLibrary.Api/appsettings.json`. When running for the first time a `games.db` file will be created automatically. If you modify the models, apply migrations with:

```bash
dotnet ef database update --project GameLibrary.Api
```

## Running

Restore dependencies and start the API:

```bash
dotnet run --project GameLibrary.Api
```

## Endpoints

- `POST /api/auth/register` – create a user
- `POST /api/auth/login` – get a JWT token
- `GET /api/games` – list all games
- `GET /api/games/{id}` – get a game by id
- `GET /api/games/me` – list your games (requires token)
- `POST /api/games` – create a game (requires token)
- `PUT /api/games/{id}` – update a game (requires token)
- `DELETE /api/games/{id}` – delete a game (requires token)
