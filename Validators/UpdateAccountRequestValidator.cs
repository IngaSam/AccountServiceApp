using AccountService.Models.Dto;
using FluentValidation;

namespace AccountService.Validators
{
    public class UpdateAccountRequestValidator: AbstractValidator<UpdateAccountRequest>
    {
        public UpdateAccountRequestValidator()
        {
            RuleFor(x => x.InterestRate)
                .GreaterThanOrEqualTo(0)
                .When(x => x.InterestRate.HasValue);
        }
    }
}
