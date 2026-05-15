# APIGateway.API/Dockerfile
################################

# --- Build stage ---
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 1. Копируем только csproj, чтобы кешировать restore
COPY APIGateway/APIGateway.API/APIGateway.API.csproj APIGateway.API/

# 2. Восстанавливаем зависимости
RUN dotnet restore APIGateway.API/APIGateway.API.csproj

# 3. Копируем весь исходный код API‑шлюза
COPY APIGateway/APIGateway.API/ APIGateway.API/

# 4. Публикуем приложение
WORKDIR /src/APIGateway.API
RUN dotnet publish -c Release -o /app/publish

# --- Runtime stage ---
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# 5. Копируем опубликованные артефакты
COPY --from=build /app/publish ./

# 6. Точка входа
ENTRYPOINT ["dotnet", "APIGateway.API.dll"]
