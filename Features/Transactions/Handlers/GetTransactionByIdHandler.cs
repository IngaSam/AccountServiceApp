using AccountService.Features.Transactions.Queries;
using AccountService.Interfaces;
using AccountService.Models;
using MediatR;

namespace AccountService.Features.Transactions.Handlers
{
    public class GetTransactionByIdHandler(ITransactionRepository repository)
        : IRequestHandler<GetTransactionByIdQuery, Transaction?>
    {
        public Task<Transaction?> Handle(GetTransactionByIdQuery request, CancellationToken ct)
        {
            var transaction = repository.GetById(request.Id);
            return Task.FromResult(transaction);
        }
    }
}