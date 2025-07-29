using AccountService.Models.Enums;

namespace AccountService.Models
{
    public class Account
    {
        public Guid Id { get; set; }                  // Уникальный идентификатор
        public Guid OwnerId { get; set; }            // ID владельца (клиента)
        public AccountType Type { get; set; }         // Тип счёта (Checking|Deposit|Credit)
        public required string Currency { get; set; }          // Валюта (ISO 4217: "RUB", "USD")
        public decimal Balance { get; set; }          // Текущий баланс
        public decimal? InterestRate { get; set; }    // Процентная ставка (для вкладов/кредитов) Nullable, только для Deposit/Credit
        public DateTime OpenDate { get; set; }        // Дата открытия
        public DateTime? CloseDate { get; set; }      // Дата закрытия (если счёт закрыт)
        public List<Transaction> Transactions { get; set; } = []; // Список транзакций

    }
}
