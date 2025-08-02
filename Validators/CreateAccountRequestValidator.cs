using FluentValidation;
using AccountService.Models.Dto;
using AccountService.Models.Enums;

namespace AccountService.Validators
{
    public class CreateAccountRequestValidator : AbstractValidator<CreateAccountRequest>
    {
        public CreateAccountRequestValidator()
        {
            RuleFor(x => x.OwnerId).NotEmpty().WithMessage("OwnerId обязателен");
            RuleFor(x => x.Type).IsInEnum().WithMessage("Тип счета должен быть одним из: Checking, Deposit, Credit");
            RuleFor(x => x.Currency).Length(3).Must(BeValidCurrency)
                .WithMessage("Валюта должна быть RUB, USD или EUR");
            RuleFor(x => x.InterestRate).GreaterThanOrEqualTo(0)
                .When(x => x.Type != AccountType.Checking)
                .WithMessage("Процентная ставка должна быть >= 0 для вклада/кредитов");

        }

        private static bool BeValidCurrency(string currency)
        {
            string[] validCurrencies = ["RUB", "USD", "EUR"];
            return validCurrencies.Contains(currency);
        }
    }
}