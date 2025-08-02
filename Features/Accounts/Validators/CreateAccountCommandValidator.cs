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
                .WithMessage("Допустимые типы счетов: Checking, Deposit, Credit");

            RuleFor(x => x.Currency)
                .NotEmpty()
                .WithMessage("Валюта обязательна")
                .Length(3)
                .WithMessage("Код валюты должен содержать ровно 3 символа")
                .Must(currency => currencyConfig.SupportedCurrencies.Contains(currency.ToUpper()))
                .WithMessage($"Поддерживаются только: {string.Join(", ", currencyConfig.SupportedCurrencies)}")
                ;

            RuleFor(x => x.InterestRate)
                .NotNull()
                .When(x => x.Type == AccountType.Deposit || x.Type == AccountType.Credit)
                .WithMessage("Процентная ставка обязательна для счетов типа Deposit или Credit")
                .GreaterThanOrEqualTo(0)
                .When(x => x.Type != AccountType.Checking)
                .WithMessage("Процентная ставка не может быть отрицательной")
                .LessThanOrEqualTo(currencyConfig.MaxInterestRate)
                .When(x => x.Type == AccountType.Deposit)
                .WithMessage($"Процентная ставка не может превышать {currencyConfig.MaxInterestRate}%")
                .Null()
                .When(x => x.Type == AccountType.Checking)
                .WithMessage("Процентная ставка не должна указываться для счетов типа Checking");
        }
    }
}