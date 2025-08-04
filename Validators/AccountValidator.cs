using FluentValidation;
using AccountService.Models;
using AccountService.Models.Enums;
using AccountService.Models.Errors;

namespace AccountService.Validators
{
    public class AccountValidator: AbstractValidator<Account>
    {
        public AccountValidator()
        {
            RuleFor(x => x.OwnerId)
                .NotEmpty()
                .WithMessage("OwnerId обезателен")
                .WithState(_ => MbError.Create("EMPTY_OWNER_ID", "Не указан владелец счёта"));

            RuleFor(x => x.Type)
                .IsInEnum()
                .WithMessage("Недопустимый тип счёта")
                .WithState(x => MbError.Create(
                    "INVALID_ACCOUNT_TYPE",
                    $"Допустимые типы: {string.Join(", ", Enum.GetNames(typeof(AccountType)))}"));

            RuleFor(x => x.Currency)
                .Length(3)
                .WithMessage("Код валюты должен содержать 3 символа")
                .WithState(_ => MbError.Create("INVALID_CURRENCY_LENGTH", "Требуется 3-символьный код валюты"))
                .Must(BeValidCurrency)
                .WithMessage("Валюта должна быть в формате ISO 4217 (RUB, USD)")
                .WithState(x => MbError.Create(
                    "UNSUPPORTED_CURRENCY",
                    $"Валюта {x.Currency} не поддерживается. Доступные: RUB, USD, EUR"));


            RuleFor(x => x.InterestRate)
                .GreaterThanOrEqualTo(0)
                .When(x => x.Type != AccountType.Checking)
                .WithMessage("Процентная ставка должна быть >= 0 для вклада/кредитов ")
                .WithState(_ => MbError.Create(
                    "INVALID_INTEREST_RATE",
                    "Процентная ставка должна быть ≥ 0 для депозитов и кредитов"))
                .LessThanOrEqualTo(100)
                .When(x => x.Type != AccountType.Checking)
                .WithMessage("Процентная ставка не может превышать 100%")
                .WithState(_ => MbError.Create(
                    "INTEREST_RATE_TOO_HIGH",
                    "Максимальная процентная ставка - 100%"));
            
            RuleFor(x => x.CloseDate)
                .GreaterThanOrEqualTo(x => x.OpenDate)
                .When(x => x.CloseDate.HasValue)
                .WithMessage("Дата закрытия не может быть раньше даты открытия")
                .WithState(_ => MbError.Create(
                    "INVALID_CLOSE_DATE",
                    "Дата закрытия должна быть после даты открытия"));
        }

        private bool BeValidCurrency(string currency)
        {
            string[] validCurrencies = ["RUB", "USD", "EUR"];
            return validCurrencies.Contains(currency);
        }
    }
}
