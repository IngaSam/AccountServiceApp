namespace AccountService.Models
{
    /// <summary>
    /// Выписка по счёту за период
    /// </summary>
    public class AccountStatement
    {
        /// <summary>
        /// Идентификатор счёта
        /// </summary>
        public Guid AccountId { get; set; }

        /// <summary>
        /// Начало периода
        /// </summary>
        public DateTime PeriodStart { get; set; }

        /// <summary>
        /// Конец периода
        /// </summary>
        public DateTime PeriodEnd { get; set; }

        /// <summary>
        /// Баланс на начало периода
        /// </summary>
        public decimal OpeningBalance { get; set; }

        /// <summary>
        /// Баланс на конец периода
        /// </summary>
        public decimal ClosingBalance { get; set; }

        /// <summary>
        /// Список транзакций за период
        /// </summary>
        public List<Transaction> Transactions { get; set; } = [];
    }
}
