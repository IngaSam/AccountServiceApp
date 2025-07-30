using AccountService.Features.Accounts.Commands;
using AccountService.Models.Configs;
using AccountService.Models.Enums;
using FluentValidation;
using Microsoft.Extensions.Options;

namespace AccountService.Features.Accounts.Validators
{
    public class CreateAccountCommandValidator : AbstractValidator<CreateAccountCommand>
    {
        public CreateAccountCommandValidator(IOptions<CurrencySettings> currencySettings)
        {
            var currencyConfig = currencySettings.Value;

            RuleFor(x => x.OwnerId)
                .NotEmpty()
                .WithMessage("Идентификатор владельца обязателен");

            RuleFor(x => x.Type)
                .IsInEnum()
                .WithMessage("Указан недопустимый тип счета");

            RuleFor(x => x.Currency)
                .NotEmpty()
                .WithMessage("Валюта обязательна")
                .Length(3)
                .WithMessage("Код валюты должен содержать ровно 3 символа")
                .Must(currency => currencyConfig.SupportedCurrencies.Contains(currency.ToUpper()))
                .WithMessage($"Поддерживаются только: {string.Join(", ", currencyConfig.SupportedCurrencies)}")
                ;

            RuleFor(x => x.InterestRate)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Процентная ставка не может быть отрицательной")
                .When(x => x.Type != AccountType.Checking)
                .LessThanOrEqualTo(currencyConfig.MaxInterestRate)
                .WithMessage($"Процентная ставка не может превышать {currencyConfig.MaxInterestRate}%")
                .When(x => x.Type == AccountType.Deposit);
        }
    }
}