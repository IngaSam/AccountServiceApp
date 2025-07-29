using AccountService.Features.Accounts.Commands;
using FluentValidation;

namespace AccountService.Features.Accounts.Validators
{
    public class UpdateAccountCommandValidator : 
        AbstractValidator<UpdateAccountCommand>
    {
        public UpdateAccountCommandValidator()
        {
            RuleFor(x => x.InterestRate)
                .GreaterThanOrEqualTo(0)
                .When(x => x.InterestRate.HasValue)
                .WithMessage("Процентная ставка не может быть отрицательной.");

            RuleFor(x => x.CloseDate)
                .GreaterThanOrEqualTo(DateTime.UtcNow.Date)
                .When(x => x.CloseDate.HasValue)
                .WithMessage("Дата закрытия не может быть в прошлом.");
        }
    }
}
