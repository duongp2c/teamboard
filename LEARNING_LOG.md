# Learning Log

## Buổi 1: Web API CRUD Board & Task

- Tạo project `TeamBoard.Api` (ASP.NET Core 6), entity `Board` / `TaskItem` (1–n), EF Core + SQL Server LocalDB, migration `InitialCreate`.
- CRUD `/api/boards`, `/api/boards/{id}/tasks`, `/api/tasks/{id}`; Swagger.
- Xoá board → xoá theo task (cascade) vì task thuộc sở hữu board.

## 2026-10-21 — Buổi 2: Docker hoá API

**Đã làm**
- `Dockerfile` 2 stage: build bằng `dotnet/sdk:6.0`, chạy bằng `dotnet/aspnet:6.0`.
- Copy `.csproj` + `dotnet restore` trước khi copy toàn bộ source để cache layer restore.
- `.dockerignore` loại `bin/`, `obj/`, `.git`, `.env`, `.vs`.
- Nâng cao: container chạy bằng user `appuser` (không phải root), nghe port 8080.

**Kích thước image** (`docker images`)

| Image | Size |
|---|---|
| `mcr.microsoft.com/dotnet/sdk:6.0` (build) | 745 MB |
| `mcr.microsoft.com/dotnet/aspnet:6.0` (runtime base) | 208 MB |
| `teamboard-api:latest` (runtime + app) | 226 MB |

→ Image cuối nhỏ hơn image SDK ~3.3 lần (745 → 226 MB); app chỉ thêm ~18 MB lên runtime base.

**Kiểm tra**
- `docker build -t teamboard-api .` thành công.
- `docker run -p 8081:8080 teamboard-api`: app khởi động, `/swagger` trả 200, `/api/boards` trả 500 vì không có DB (bình thường — LocalDB chỉ có trên Windows). (Port 8080 trên máy host đang bị dịch vụ khác chiếm nên map sang 8081.)
- `docker exec ... whoami` → `appuser`.
- Sửa 1 file `.cs` rồi build lại: bước `RUN dotnet restore` hiện `CACHED`, chỉ `dotnet publish` chạy lại.

**Học được**
- Image = bản đóng gói chỉ đọc; container = tiến trình chạy từ image; layer = mỗi lệnh Dockerfile, cache theo nội dung; registry = kho push/pull image.
- Thứ tự lệnh trong Dockerfile quyết định cache: thứ ít đổi (csproj) để trước, thứ hay đổi (source) để sau.
- Image `aspnet:6.0` mặc định nghe port 80 → đặt `ASPNETCORE_URLS=http://+:8080` để chạy được bằng user không phải root.
- `Database.Migrate()` lúc startup làm app crash nếu không có DB → bọc try/catch và log lỗi.
- Docker Desktop báo lỗi tích hợp WSL với `kali-linux` (WSL 1) → tắt WSL integration cho distro đó, không ảnh hưởng tới build.

**Việc tiếp theo**
- Thử image `alpine` / chiseled để so kích thước.

## 2026-10-22 — SQL Server trong container (Docker Compose)

**Đã làm**
- `docker-compose.yml` gồm 2 service: `db` (SQL Server 2022) và `api` (build từ Dockerfile).
- Mật khẩu `sa` đặt trong `.env` (đã thêm vào `.gitignore`), có `.env.example` làm mẫu.
- API nhận connection string qua biến môi trường `ConnectionStrings__DefaultConnection` (dấu `__` = dấu `:` trong cấu hình .NET), ghi đè `appsettings.json`.
- Volume `mssql-data` giữ dữ liệu khi xoá container.

**Kiểm tra**
- `docker compose up -d --build`: `db` healthy rồi `api` mới start; migration chạy thành công.
- `POST /api/boards` → 201, `GET /api/boards` → 200 có dữ liệu.
- `docker compose down` rồi `up` lại: board vẫn còn (nhờ volume).

**Học được**
- Trong mạng của compose, các container gọi nhau bằng **tên service** (`Server=db,1433`), không dùng `localhost` (localhost trong container là chính nó).
- `depends_on` chỉ đợi container *start*; cần `healthcheck` + `condition: service_healthy` để đợi SQL Server *sẵn sàng* nhận kết nối.
- Container tạo hỏng (lần đầu lỗi trùng port) không có port mapping / DNS → `docker compose up -d --force-recreate` để tạo lại.
- Không để mật khẩu trong file commit lên git; dùng `.env` + `.env.example`.
