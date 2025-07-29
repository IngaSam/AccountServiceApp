using AccountService.Features.Transfers.Commands;
using FluentValidation;

namespace AccountService.Validators
{
    public class TransferCommandValidator : AbstractValidator<TransferCommand>
    {
        public TransferCommandValidator()
        {
            RuleFor(x => x.FromAccountId).NotEmpty().NotEqual(x => x.ToAccountId);
            RuleFor(x => x.ToAccountId).NotEmpty();
            RuleFor(x => x.Amount).GreaterThan(0);
        }
    }
}
