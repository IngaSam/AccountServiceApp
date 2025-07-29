using AccountService.Features.Accounts.Queries;
using AccountService.Interfaces;
using AccountService.Models;
using MediatR;

namespace AccountService.Features.Accounts.Handlers
{
    public class GetTransactionsByAccountIdHandler : 
        IRequestHandler<GetTransactionsByAccountIdQuery, IEnumerable<Transaction>>
    {
        private readonly ITransactionRepository _repository;

        public GetTransactionsByAccountIdHandler(ITransactionRepository repository)
            => _repository = repository;

        public Task<IEnumerable<Transaction>> Handle(
            GetTransactionsByAccountIdQuery request,
            CancellationToken ct)
            => Task.FromResult(_repository.GetByAccountId(request.AccountId));
    }
}
