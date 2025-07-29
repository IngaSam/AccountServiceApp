namespace AccountService.Models.Dto
{
    /// <summary>
    /// Данные для обновления счета
    /// </summary>
    public class UpdateAccountRequest
    {
        /// <summary>
        /// Новая процентная ставка
        /// </summary>
        /// <example>5.0</example>
        public decimal? InterestRate { get; set; }

        /// <summary>
        /// Дата закрытия счета
        /// </summary>
        /// <example>2025-12-31</example>
        public DateTime? CloseDate { get; set; }
    }
}
