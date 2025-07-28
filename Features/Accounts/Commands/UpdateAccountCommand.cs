using AccountService.Models;
using MediatR;

namespace AccountService.Features.Accounts.Commands
{
    public record UpdateAccountCommand(
        Guid Id,
        decimal? InterestRate,
        DateTime? CloseDate) : IRequest<Account?>;
}
