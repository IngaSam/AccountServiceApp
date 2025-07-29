# Account Service (Microservice)

Микросервис для управления банковскими счетами и транзакциями

## 📌 Функционал
- Создание, изменение, удаление счетов
- Пополнение и переводы между счетами
- Получение выписок
- Валидация операций
- Ведение истории транзакций

## 🚀 Запуск проекта
1. Убедитесь, что установлены:
   - .NET 6+ SDK
   - Docker (опционально)

2. Запустите проект:
```bash
dotnet run --project AccountService.Api

3. Откройте Swagger UI:
```http://localhost:5000/swagger

📚 API Endpoints
Метод	Путь	Описание
POST	/api/accounts	Создать новый счет
GET	/api/accounts	Получить все счета
GET	/api/accounts/{id}	Получить счет по ID
PUT	/api/accounts/{id}	Обновить счет
POST	/api/transactions	Создать транзакцию


🛠 Технологии
 - ASP.NET Core 6
 - MediatR (CQRS)
 - FluentValidation
 - Entity Framework Core
 - Swagger/OpenAPI

🗄 Модели данных
classDiagram
    class Account {
        +Guid Id
        +Guid OwnerId
        +AccountType Type
        +string Currency
        +decimal Balance
        +decimal? InterestRate
        +DateTime OpenDate
        +DateTime? CloseDate
        +List<Transaction> Transactions
    }
    
    class Transaction {
        +Guid Id
        +Guid AccountId
        +Guid? CounterpartyId
        +decimal Amount
        +string Currency
        +TransactionType Type
        +string Description
        +DateTime Date
    }


✅ Критерии выполнения задания
Реализованы все API endpoints

Настроен Swagger

Добавлена валидация через FluentValidation

Реализован CQRS через MediatR

Созданы заглушки сервисов

Добавлена документация