# PaymentService.API/Dockerfile
################################

# --- Build stage ---
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 1. Копируем только csproj-ы, чтобы кешировать restore
COPY PaymentService/PaymentService.Domain/PaymentService.Domain.csproj      PaymentService.Domain/
COPY PaymentService/PaymentService.Application/PaymentService.Application.csproj  PaymentService.Application/
COPY PaymentService/PaymentService.Infrastructure/PaymentService.Infrastructure.csproj  PaymentService.Infrastructure/
COPY PaymentService/PaymentService.API/PaymentService.API.csproj          PaymentService.API/

# 2. Restore
RUN dotnet restore PaymentService.API/PaymentService.API.csproj

# 3. Копируем весь код
COPY PaymentService/PaymentService.Domain/      PaymentService.Domain/
COPY PaymentService/PaymentService.Application/ PaymentService.Application/
COPY PaymentService/PaymentService.Infrastructure/ PaymentService.Infrastructure/
COPY PaymentService/PaymentService.API/         PaymentService.API/

# 4. Публикуем
WORKDIR /src/PaymentService.API
RUN dotnet publish -c Release -o /app/publish

# --- Runtime stage ---
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# 5. Копируем артефакты из build
COPY --from=build /app/publish ./

# 6. Точка входа
ENTRYPOINT ["dotnet", "PaymentService.API.dll"]
