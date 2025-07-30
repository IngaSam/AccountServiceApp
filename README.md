# Account Service (Microservice)

Микросервис для управления банковскими счетами с REST API, реализующий:

    ✅ Создание/изменение/удаление счетов

    ✅ Транзакции и переводы между счетами

    ✅ Выписки по счетам

## 📌 Функционал
    - Создание, изменение, удаление счетов
    - Пополнение и переводы между счетами
    - Получение выписок
    - Валидация операций
    - Ведение истории транзакций

## 🛠 Технологии
    Язык: C# (.NET 9)

    Архитектура: Clean Architecture + CQRS

    Библиотеки:

    - MediatR (для CQRS)

    - FluentValidation

    - Swagger (документация API)
    Хранение данных: In-memory (заглушки)

## 🚀 Запуск проекта
1. Клонировать репозиторий:
```bash
 git clone https://github.com/IngaSam/AccountService.git
 ```

2. Запустите проект:
```bash
cd AccountService
dotnet runi
```

3. Откройте Swagger UI:
```
http://localhost:5121/swagger
```

## 📡 API Endpoints

| Метод  | Путь                          | Описание                          |
|--------|-------------------------------|-----------------------------------|
| `GET`  | `/api/Accounts`               | Получить список всех счетов       |
| `POST` | `/api/Accounts`               | Создать новый банковский счёт     |
| `GET`  | `/api/Accounts/{id}`          | Получить счёт по ID               |
| `PUT`  | `/api/Accounts/{id}`          | Обновить счёт (полная замена)     |
| `PATCH`| `/api/Accounts/{id}`          | Частично обновить счёт            |
| `DELETE`| `/api/Accounts/{id}`         | Удалить счёт (мягкое удаление)    |
| `GET`  | `/api/Accounts/{id}/statement`| Получить выписку по счёту         |
| `GET`  | `/api/Accounts/{id}/transactions` | Список транзакций по счёту    |
| `POST` | `/api/Transactions`           | Создать транзакцию                |
| `GET`  | `/api/Transactions/{id}`      | Получить транзакцию по ID         |
| `POST` | `/api/Transfers`              | Выполнить перевод между счетами   |    


```json
    {
  "ownerId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "type": "Deposit",
  "currency": "USD",
  "interestRate": 3.5
}
```

```json
    {
  "accountId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "amount": 1000.50,
  "type": "Credit",
  "description": "Пополнение через кассу"
}
```

Коды ответов:

- 200 – Успешный перевод

- 400 – Ошибка валидации/недостаточно средств

- 404 – Счет не найден



## 🗄 Модели данных


    class Account {
        <<Entity>>
        +Guid Id
        +Guid OwnerId
        +AccountType Type
        +string Currency
        +decimal Balance
        +decimal? InterestRate
        +DateTime OpenDate
        +DateTime? CloseDate
        +List~Transaction~ Transactions
    }
    
    class Transaction {
        <<Entity>>
        +Guid Id
        +Guid AccountId
        +Guid? CounterpartyAccountId
        +decimal Amount
        +string Currency
        +TransactionType Type
        +string Description
        +DateTime DateTime
    }

    class AccountStatement {
        <<DTO>>
        +Guid AccountId
        +DateTime PeriodStart
        +DateTime PeriodEnd
        +decimal OpeningBalance
        +decimal ClosingBalance
        +List~Transaction~ Transactions
    }

    

## ✅ Критерии выполнения задания
    Реализованы все API endpoints

    Настроен Swagger

    Добавлена валидация через FluentValidation

    Реализован CQRS через MediatR

    Созданы заглушки сервисов

    Добавлена документацияgit