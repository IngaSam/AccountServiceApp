namespace AccountService.Models.Dto
{
    // <summary>
    /// DTO для представления информации о банковском счете
    /// </summary>
    public class AccountDto
    {
        /// <summary>
        /// Уникальный идентификатор счета
        /// </summary>
        /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
        public Guid Id { get; set; }

        /// <summary>
        /// Тип счета (Checking|Deposit|Credit)
        /// </summary>
        /// <example>Deposit</example>
        public string Type { get; set; }  
       
        /// <summary>
        /// Валюта счета (3-х буквенный код ISO)
        /// </summary>
        /// <example>RUB</example>
        public string Currency { get; set; }

        /// <summary>
        /// Текущий баланс
        /// </summary>
        /// <example>15000.50</example>
        public decimal Balance { get; set; }
    }
}
