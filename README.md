# Account Service API

Микросервис для управления банковскими счетами (текущий счет, депозиты, кредиты) с поддержкой транзакций.

## Текущее состояние проекта (v0.1)

### Реализованные функции
✅ **Управление счетами:**
- Создание нового счета (`POST /api/accounts`)
- Просмотр всех счетов с фильтрацией (`GET /api/accounts`)
- Поддержка типов счетов: `Checking` (текущий), `Deposit` (депозит), `Credit` (кредитный)
- Валидация входящих данных (FluentValidation)

✅ **Технологический стек:**
- .NET 9
- ASP.NET Core Web API
- MediatR (CQRS)
- FluentValidation
- Swagger (OpenAPI 3.0)
- In-memory хранилище (заглушка)

### API Endpoints

| Метод | Путь | Описание |
|-------|------|----------|
| POST | `/api/accounts` | Создание нового счета |
| GET | `/api/accounts` | Получение списка счетов (с фильтрацией) |

## Как запустить проект

1. **Требования:**
   - .NET 9 SDK
   - Visual Studio 2022 (или Rider/VSCode)

2. **Запуск:**
   ```bash
   git clone https://github.com/ваш-логин/AccountServiceApp.git
   cd AccountServiceApp
   dotnet run
