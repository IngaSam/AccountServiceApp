using AccountService.Models;
using AccountService.Models.Enums;
using MediatR;

namespace AccountService.Features.Accounts.Commands
{
    public record CreateAccountCommand
    (
        Guid OwnerId, 
        AccountType Type,
        string Currency,
        decimal? InterestRate): IRequest<Account>;

    
}
