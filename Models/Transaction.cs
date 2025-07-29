using AccountService.Models.Enums;

namespace AccountService.Models
{
    public class Transaction
    {
        public Guid Id { get; set; }                  // Уникальный идентификатор
        public Guid AccountId { get; set; }           // ID счёта
        public Guid? CounterpartyAccountId { get; set; } // ID счёта-контрагента (для переводов) Nullable Guid (для переводов)
        public decimal Amount { get; set; }           // Сумма
        public required string Currency { get; set; }          // Валюта операции
        public TransactionType Type { get; set; }     // Тип (Credit|Debit)
        public required string Description { get; set; }       // Описание ("Пополнение", "Перевод")
        public DateTime DateTime { get; set; }        // Дата и время операции
    }
}
