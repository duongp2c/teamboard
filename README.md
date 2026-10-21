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

## Docker

```bash
docker build -t teamboard-api .
docker run --rm -p 8080:8080 teamboard-api
```

Swagger: http://localhost:8080/swagger. Inside the container `(localdb)` does not exist (LocalDB is Windows-only), so DB endpoints fail unless you pass a reachable SQL Server:

```bash
docker run --rm -p 8080:8080 -e ConnectionStrings__DefaultConnection="Server=host.docker.internal,1433;Database=TeamBoard;User Id=sa;Password=<pwd>;TrustServerCertificate=True" teamboard-api
```

### Docker Compose: API + SQL Server

```bash
cp .env.example .env        # roi doi SA_PASSWORD
docker compose up -d --build
```

- `db`: SQL Server 2022 (`mcr.microsoft.com/mssql/server:2022-latest`), port 1433, data in volume `mssql-data`.
- `api`: built from `Dockerfile`, Swagger at http://localhost:8081/swagger. It waits for `db` to be healthy, then applies migrations on startup.
- Connect from SSMS / Azure Data Studio: `localhost,1433`, user `sa`, password from `.env`.
- `docker compose down` keeps the data; `docker compose down -v` deletes the volume too.

### Khái niệm (giải thích bằng lời của mình)
- **Image**: "bản đóng gói" chỉ đọc gồm hệ điều hành tối thiểu + runtime + app đã build. Giống một cái khuôn/snapshot.
- **Container**: một tiến trình đang chạy từ image, có filesystem ghi được riêng, cô lập với máy host. Một image chạy được nhiều container.
- **Layer**: mỗi lệnh `FROM/COPY/RUN` trong Dockerfile tạo một lớp. Lớp được cache theo nội dung; lớp nào không đổi thì dùng lại (`CACHED`), nên copy `.csproj` + `restore` trước để sửa code không phải tải lại package.
- **Registry**: kho chứa image để push/pull (Docker Hub, `mcr.microsoft.com`, GHCR...). `mcr.microsoft.com/dotnet/sdk` là image lấy từ registry của Microsoft.

### Multi-stage
- Stage `build` dùng `dotnet/sdk:6.0` (có compiler, NuGet) để restore + publish.
- Stage `runtime` dùng `dotnet/aspnet:6.0` (chỉ runtime), chỉ copy thư mục publish sang → image nhỏ hơn nhiều, không chứa source/SDK.
- Container chạy bằng user `appuser` (không phải root), nghe port 8080.

## Migrations

```bash
dotnet tool restore
dotnet ef migrations add <Name> -p TeamBoard.Api
```
