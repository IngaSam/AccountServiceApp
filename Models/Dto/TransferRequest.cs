namespace AccountService.Models.Dto
{
    /// <summary>
    /// Запрос на перевод между счетами
    /// </summary>
    public class TransferRequest
    {
        /// <summary>
        /// Идентификатор счета-отправителя
        /// </summary>
        /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
        public Guid FromAccountId { get; set; }

        /// <summary>
        /// Идентификатор счета-получателя
        /// </summary>
        /// <example>4fa85f64-5717-4562-b3fc-2c963f66afa7</example>
        public Guid ToAccountId { get; set; }

        /// <summary>
        /// Сумма перевода (должна быть положительной)
        /// </summary>
        /// <example>500.00</example>
        public decimal Amount { get; set; }
    }
}
