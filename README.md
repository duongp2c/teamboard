# TeamBoard API

ASP.NET Core 6 Web API + EF Core 6 + SQL Server. CRUD for `Board` and `Task` (a board has many tasks).

## Run

```bash
# connection string: TeamBoard.Api/appsettings.json -> ConnectionStrings:DefaultConnection
dotnet run --project TeamBoard.Api
```

Migrations are applied automatically on startup (the `TeamBoard` database is created if missing).
Open Swagger at `http(s)://localhost:<port>/swagger`.

## Endpoints

| Method | Route | Description |
|---|---|---|
| GET | /api/boards | List boards |
| GET | /api/boards/{id} | Get board |
| GET | /api/boards/{id}/tasks | Tasks of a board |
| POST | /api/boards | Create board |
| PUT | /api/boards/{id} | Update board |
| DELETE | /api/boards/{id} | Delete board (and its tasks) |
| GET | /api/tasks?boardId=&status= | List tasks (optional filters) |
| GET | /api/tasks/{id} | Get task |
| POST | /api/tasks | Create task |
| PUT | /api/tasks/{id} | Update task |
| DELETE | /api/tasks/{id} | Delete task |

Task `status`: `Todo` | `InProgress` | `Done`.

## Migrations

```bash
dotnet tool restore
dotnet ef migrations add <Name> -p TeamBoard.Api
```
