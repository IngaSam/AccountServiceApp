using AccountService.Features.Accounts.Queries;
using AccountService.Interfaces;
using AccountService.Models;
using MediatR;

namespace AccountService.Features.Accounts.Handlers
{
    public class GetTransactionsByAccountIdHandler(ITransactionRepository repository) :
        IRequestHandler<GetTransactionsByAccountIdQuery, IEnumerable<Transaction>>
    {
        public Task<IEnumerable<Transaction>> Handle(
            GetTransactionsByAccountIdQuery request,
            CancellationToken ct)
            => Task.FromResult(repository.GetByAccountId(request.AccountId));
    }
}
