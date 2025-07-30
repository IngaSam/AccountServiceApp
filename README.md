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

- Хранение данных: In-memory (заглушки)

## 🚀 Запуск проекта
1. Клонировать репозиторий:
```bash
 git clone https://github.com/IngaSam/AccountService.git
  
2. Запустите проект:
```bash
cd AccountService
dotnet runi

3. Откройте Swagger UI:
```
http://localhost:5121/swagger


## 📡 API Endpoints
# 1. Управление счетами (/api/Accounts)
Метод	Endpoint	Описание	Параметры
GET	/api/Accounts	Получить список счетов	currency (фильтр по валюте), type (фильтр по типу), page, pageSize
POST	/api/Accounts	Создать новый счет	OwnerId (GUID), Type (Checking/Deposit/Credit), Currency (ISO 4217)
GET	/api/Accounts/{id}	Получить счет по ID	id (GUID счета)
PUT	/api/Accounts/{id}	Обновить счет (полная замена)	InterestRate, CloseDate
PATCH	/api/Accounts/{id}	Частичное обновление	JSON Patch-документ
DELETE	/api/Accounts/{id}	"Удалить" счет (мягкое удаление)	–
    ```json
    {
  "ownerId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "type": "Deposit",
  "currency": "USD",
  "interestRate": 3.5
}


# 2. Транзакции (/api/Transactions)
Метод	Endpoint	Описание	Параметры
POST	/api/Transactions	Создать транзакцию	AccountId, Amount, Type (Credit/Debit), Description
GET	/api/Transactions/{id}	Получить транзакцию по ID	id (GUID транзакции)
GET	/api/Accounts/{id}/transactions	Список транзакций по счету	fromDate, toDate (фильтр по дате)
    ```json
    {
  "accountId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "amount": 1000.50,
  "type": "Credit",
  "description": "Пополнение через кассу"
}


# 3. Переводы (/api/Transfers)
Метод	Endpoint	Описание	Параметры
POST	/api/Transfers	Перевод между счетами	FromAccountId, ToAccountId, Amount
Коды ответов:

- 200 – Успешный перевод

- 400 – Ошибка валидации/недостаточно средств

- 404 – Счет не найден

# 4. Выписки (/api/Accounts/{id}/statement)
Параметры:

- fromDate – Начало периода (по умолчанию: 30 дней назад)

- toDate – Конец периода (по умолчанию: текущая дата)



## 🗄 Модели данных
```mermaid
classDiagram
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

    Account "1" *-- "0..*" Transaction
    Account --> AccountStatement : Генерация




## ✅ Критерии выполнения задания
Реализованы все API endpoints

Настроен Swagger

Добавлена валидация через FluentValidation

Реализован CQRS через MediatR

Созданы заглушки сервисов

Добавлена документация