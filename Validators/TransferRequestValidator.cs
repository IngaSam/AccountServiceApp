using AccountService.Models.Dto;
using FluentValidation;

namespace AccountService.Validators
{
    public class TransferRequestValidator: AbstractValidator<TransferRequest>
    {
        public TransferRequestValidator()
        {
            RuleFor(x => x.FromAccountId).NotEmpty();
            RuleFor(x => x.ToAccountId).NotEmpty();
            RuleFor(x => x.Amount).GreaterThan(0);
            RuleFor(x => x.FromAccountId)
                .NotEqual(x => x.ToAccountId)
                .WithMessage("Cannot transfer to the account");
        }
    }
}
