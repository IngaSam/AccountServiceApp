using AccountService.Models;
using MediatR;

namespace AccountService.Features.Accounts.Queries
{
    public record GetTransactionsByAccountIdQuery(Guid AccountId) : 
        IRequest<IEnumerable<Transaction>>;
}
