# OrderService.API/Dockerfile
################################

# --- Build stage ---
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 1. Копируем только csproj для кеширования restore
COPY OrderService/OrderService.Domain/OrderService.Domain.csproj      OrderService.Domain/
COPY OrderService/OrderService.Application/OrderService.Application.csproj  OrderService.Application/
COPY OrderService/OrderService.Infrastructure/OrderService.Infrastructure.csproj  OrderService.Infrastructure/
COPY OrderService/OrderService.API/OrderService.API.csproj          OrderService.API/

# 2. Восстанавливаем пакеты для API проекта
RUN dotnet restore OrderService.API/OrderService.API.csproj

# 3. Копируем весь исходный код подпроектов
COPY OrderService/OrderService.Domain/      OrderService.Domain/
COPY OrderService/OrderService.Application/ OrderService.Application/
COPY OrderService/OrderService.Infrastructure/ OrderService.Infrastructure/
COPY OrderService/OrderService.API/         OrderService.API/

# 4. Публикуем приложение
WORKDIR /src/OrderService.API
RUN dotnet publish -c Release -o /app/publish

# --- Runtime stage ---
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# 5. Копируем опубликованные артефакты
COPY --from=build /app/publish ./

# 6. Точка входа
ENTRYPOINT ["dotnet", "OrderService.API.dll"]
