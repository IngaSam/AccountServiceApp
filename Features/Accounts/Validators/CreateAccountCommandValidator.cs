using AccountService.Features.Accounts.Commands;
using AccountService.Models.Configs;
using AccountService.Models.Enums;
using FluentValidation;

namespace AccountService.Validators
{
    public class CreateAccountCommandValidator : AbstractValidator<CreateAccountCommand>
    {
        public CreateAccountCommandValidator()
        {
            RuleFor(x => x.OwnerId).NotEmpty();
            RuleFor(x => x.Type).IsInEnum();
            RuleFor(x => x.Currency).Length(3).Must(BeSupportedCurrency);
            RuleFor(x => x.InterestRate)
                .GreaterThanOrEqualTo(0)
                .When(x => x.Type != AccountType.Checking);
        }

        private bool BeSupportedCurrency(string currency)
            => CurrencyConfig.SupportedCurrencies.Contains(currency);
    }
}
