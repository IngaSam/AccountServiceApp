using FluentValidation;
using AccountService.Models;

namespace AccountService.Validators
{
    public class TransactionValidator: AbstractValidator<Transaction>
    {
        public TransactionValidator()
        {
            RuleFor(x => x.AccountId).NotEmpty().WithMessage("AccountId обязателен ");

            RuleFor(x => x.Amount).GreaterThan(0).WithMessage("Сумма должна быть положительной ");

            RuleFor(x => x.Currency).Length(3).
                Must(BeValidCurrency).WithMessage("Неверный код валюты ");
            RuleFor(x => x.Description).MaximumLength(500).WithMessage("Описание не должно превышать 500 символов");
        }

        private bool BeValidCurrency(string currency)
        {
            string[] validCurrencies = ["RUB", "USD", "EUR"];
            return validCurrencies.Contains(currency);
        }
    }
}
