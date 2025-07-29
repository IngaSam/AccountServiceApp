namespace AccountService.Models.Dto
{
    /// <summary>
    /// DTO для представления информации о транзакции
    /// </summary>
    public class TransactionDto
    {
        /// <summary>
        /// Сумма транзакции (всегда положительная)
        /// </summary>
        /// <example>1000.50</example>
        public decimal Amount { get; set; }

        /// <summary>
        /// Валюта транзакции (ISO 4217)
        /// </summary>
        /// <example>RUB</example>
        public required string Currency { get; set; }

        /// <summary>
        /// Тип транзакции: Credit (зачисление) или Debit (списание)
        /// </summary>
        /// <example>Credit</example>
        /// 
        public required string Type { get; set; }

        /// <summary>
        /// Описание транзакции
        /// </summary>
        /// <example>Пополнение через мобильное приложение</example>
        public required string Description { get; set; }

    }
}
