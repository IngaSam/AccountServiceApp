using AccountService.Models.Enums;

namespace AccountService.Models.Dto
{
    /// <summary>
    /// Запрос на создание транзакции
    /// </summary>
    public class CreateTransactionRequest
    {
        /// <summary>
        /// Идентификатор счета
        /// </summary>
        /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
        public Guid AccountId { get; set; }

        /// <summary>
        /// Сумма транзакции (должна быть положительной)
        /// </summary>
        /// <example>1000.50</example>
        public decimal Amount { get; set; }

        /// <summary>
        /// Тип транзакции (Credit - зачисление, Debit - списание)
        /// </summary>
        /// <example>Credit</example>
        public TransactionType Type { get; set; }

        /// <summary>
        /// Описание транзакции (макс. 500 символов)
        /// </summary>
        /// <example>Пополнение через кассу</example>
        public required string Description { get; set; }
    }
}
