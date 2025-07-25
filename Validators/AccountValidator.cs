using FluentValidation;
using AccountService.Models;
using AccountService.Models.Enums;

namespace AccountService.Validators
{
    public class AccountValidator: AbstractValidator<Account>
    {
        public AccountValidator()
        {
            RuleFor(x => x.OwnerId).NotEmpty().WithMessage("OwnerId обезателен");

            RuleFor(x => x.Type).IsInEnum().WithMessage("Недопустимый тип счёта");

            RuleFor(x => x.Currency).Length(3)
                .Must(BeValidCurrency).WithMessage("Валюта должна быть в формате ISO 4217 (RUB, USD)");
            RuleFor(x => x.InterestRate).GreaterThanOrEqualTo(0)
                .When(x => x.Type != AccountType.Checking)
                .WithMessage("Процентная ставка должна быть >= 0 для вклада/кредитов ");

        }

        private bool BeValidCurrency(string currency)
        {
            string[] validCurrencies = { "RUB", "USD", "EUR" };
            return validCurrencies.Contains(currency);
        }
    }
}
