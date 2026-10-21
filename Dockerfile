# ---------- Stage 1: build ----------
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# Copy csproj + restore truoc de cache layer restore (chi chay lai khi csproj doi)
COPY TeamBoard.Api/TeamBoard.Api.csproj TeamBoard.Api/
RUN dotnet restore TeamBoard.Api/TeamBoard.Api.csproj

# Copy toan bo source roi publish
COPY . .
RUN dotnet publish TeamBoard.Api/TeamBoard.Api.csproj -c Release -o /app/publish --no-restore

# ---------- Stage 2: runtime ----------
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime
WORKDIR /app

# Image .NET 6 mac dinh nghe port 80 (can root) -> doi sang 8080
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

# Chay bang user khong phai root
RUN adduser --disabled-password --gecos "" --uid 1001 appuser
USER appuser

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "TeamBoard.Api.dll"]
