# Микросервисная система управления заказами и оплатами
**Автор:** Яковлев Эрик Денисович (БПИ239)  
**Работа:** КР №3

---

## Оглавление
1. [Архитектура приложения](#архитектура-приложения)
2. [Запуск приложения](#запуск-приложения)
3. [Ключевые пользовательские сценарии](#ключевые-пользовательские-сценарии)
4. [API Endpoints](#api-endpoints)
5. [Паттерны и гарантии доставки](#паттерны-и-гарантии-доставки)
6. [Тестирование](#тестирование)
7. [Структура проекта](#структура-проекта)

---

## Архитектура приложения

1. **API Gateway**
    - Маршрутизация запросов к внутренним сервисам
    - Единая точка входа для клиентов

2. **Order Service**
    - Управление жизненным циклом заказов
    - Отслеживание статусов заказов
    - Интеграция с платежной системой через события

3. **Payment Service**
    - Управление счетами пользователей
    - Пополнение баланса и списания средств
    - Обработка платежных операций

4. **Frontend**
    - Веб-интерфейс на React.js
    - Взаимодействие с бэкендом через API Gateway


---

## Запуск приложения
### Требования:
- Docker 24.0+
- Docker Compose 2.20+

### Инструкция:
1. Переход в папку проекта:
   ```
   cd src
   ```

2. Восстановление зависимостей:
   ```
   dotnet restore
   ```
3. Поднятие проекта через docker-compose
   ```
   docker-compose up --build
   ```

**Доступные сервисы после запуска:**
- Фронтенд: http://localhost
- API Gateway Swagger: http://localhost:8080/swagger
- Order Service Swagger: http://localhost:8081/swagger
- Payment Service Swagger: http://localhost:8082/swagger

---

## Ключевые пользовательские сценарии

### 1. Создание заказа
![Editor _ Mermaid Chart-2025-06-19-110741.png](readme_images/Editor%20_%20Mermaid%20Chart-2025-06-19-110741.png)

### 2. Пополнение счета
![Editor _ Mermaid Chart-2025-06-19-110850.png](readme_images/Editor%20_%20Mermaid%20Chart-2025-06-19-110850.png)
---

## API Endpoints
### Order Service
| Метод | Путь               | Описание                  |
|-------|--------------------|---------------------------|
| POST  | /orders            | Создать новый заказ       |
| GET   | /orders            | Получить список заказов   |
| GET   | /orders/{orderId}  | Получить детали заказа    |

### Payment Service
| Метод | Путь                         | Описание                     |
|-------|------------------------------|------------------------------|
| POST  | /accounts                    | Создать счет пользователя    |
| POST  | /accounts/{userId}/deposit   | Пополнить баланс             |
| GET   | /accounts/{userId}/balance   | Получить баланс счета        |

---

## Паттерны и гарантии доставки
### Реализованные паттерны:
1. **Transactional Outbox (Order Service)**
    - Гарантирует доставку событий о создании заказа
    - Предотвращает потерю событий при сбоях

2. **Transactional Inbox + Outbox (Payment Service)**
    - Обеспечивает exactly once семантику при обработке платежей

3. **Idempotent Consumer**
    - Защита от повторной обработки событий
    - Проверка уникальности OrderId в Payment Service

### Гарантии доставки:
- At-least-once доставка событий через Kafka
- Exactly-once семантика для финансовых операций

---

## Тестирование
1. Переход в папку проекта
   ```
   cd src
   ```
2. Прогон тестов
   ```
   dotnet test /p:CollectCoverage=true /p:CoverletOutput=./coverage/ /p:CoverletOutputFormat=opencover
   ```
3. Создание отчета
   ```
   reportgenerator \ -reports:Tests/Tests/coverage/coverage.opencover.xml \ -targetdir:coverage/report \ -reporttypes:Html
   ```
4. Отчет можно посмотреть по пути
   ```
   src/coverage/report/index.html
   ```
Результаты тестов: 17% покрытия строк

---

## Структура проекта
```
└── src
    ├── APIGateway
    │    └── APIGateway.API
    ├── Frontend
    │    ├── public
    │    └── src
    ├── OrderService
    │    ├── OrderService.API
    │    ├── OrderService.Application
    │    ├── OrderService.Domain
    │    └── OrderService.Infrastructure
    ├── PaymentService
    │    ├── PaymentService.API
    │    ├── PaymentService.Application
    │    ├── PaymentService.Domain
    │    └── PaymentService.Infrastructure
    ├── Tests
    │    └── Tests
    └── docker-compose.yml
```