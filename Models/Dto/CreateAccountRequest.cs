using AccountService.Models.Enums;

namespace AccountService.Models.Dto

{
    public class CreateAccountRequest //запрос на создание счета
    {
        public Guid OwnerId { get; set; }

        public AccountType Type { get; set; }
        public string Currency { get; set; }

        public decimal? InterestRate { get; set;  }
    }
}
