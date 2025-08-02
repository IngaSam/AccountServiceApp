using AccountService.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace AccountService.Models.Dto

{
    /// <summary>
    /// Запрос на создание банковского счета
    /// </summary>
    public class CreateAccountRequest //запрос на создание счета
    {
        /// <summary>
        /// Идентификатор владельца счета (GUID)
        /// </summary>
        /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
        public Guid OwnerId { get; set; }

        /// <summary>
        /// Тип счета (Checking, Deposit, Credit)
        /// </summary>
        /// <example>Deposit</example>
        public AccountType Type { get; set; }

        /// <summary>
        /// Валюта счета (3-х буквенный код ISO)
        /// </summary>
        /// <example>RUB</example>
        public required string Currency { get; set; }

        /// <summary>
        /// Процентная ставка (только для Deposit/Credit, должна быть от 0 до 100)
        /// </summary>
        /// <example>3.5</example>
        [Range(0, 100, ErrorMessage = "Процентная ставка должна быть от 0 до 100")]
        public decimal? InterestRate { get; set; }
    }
}
