using AccountService.Features.Accounts.Commands;
using AccountService.Models.Configs;
using AccountService.Models.Enums;
using FluentValidation;
using Microsoft.Extensions.Options;
using System;

namespace AccountService.Features.Accounts.Validators
{
    public class CreateAccountCommandValidator : AbstractValidator<CreateAccountCommand>
    {
        public CreateAccountCommandValidator(IOptions<CurrencySettings> currencySettings)
        {
            var currencyConfig = currencySettings.Value;

            ClassLevelCascadeMode = CascadeMode.Stop;
            RuleLevelCascadeMode = CascadeMode.Stop;

            // Правильная валидация OwnerId
            RuleFor(x => x.OwnerId)
                .NotEmpty().WithMessage("Идентификатор владельца обязателен");

            // Валидация Type
            RuleFor(x => x.Type)
                .IsInEnum().WithMessage("Допустимые типы счетов: Checking, Deposit, Credit")
                .NotEqual(AccountType.Unknown).WithMessage("Тип счета не может быть Unknown");

            // Валидация Currency
            RuleFor(x => x.Currency)
                .NotEmpty().WithMessage("Валюта обязательна")
                .Length(3).WithMessage("Код валюты должен содержать ровно 3 символа")
                .Must(currency => currencyConfig.SupportedCurrencies.Contains(currency.ToUpper()))
                .WithMessage($"Поддерживаются только: {string.Join(", ", currencyConfig.SupportedCurrencies)}");

            // Условная валидация InterestRate
            When(x => x.Type == AccountType.Deposit || x.Type == AccountType.Credit, () =>
            {
                RuleFor(x => x.InterestRate)
                    .NotNull().WithMessage("Процентная ставка обязательна для счетов типа Deposit или Credit")
                    .GreaterThanOrEqualTo(0).WithMessage("Процентная ставка не может быть отрицательной")
                    .LessThanOrEqualTo(currencyConfig.MaxInterestRate)
                    .WithMessage($"Процентная ставка не может превышать {currencyConfig.MaxInterestRate}%");
            });

            When(x => x.Type == AccountType.Checking, () =>
            {
                RuleFor(x => x.InterestRate)
                    .Null().WithMessage("Процентная ставка не должна указываться для счетов типа Checking");
            });
        }
     

    }
}