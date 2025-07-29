using AccountService.Features.Transactions.Queries;
using AccountService.Interfaces;
using AccountService.Models;
using MediatR;

namespace AccountService.Features.Transactions.Handlers
{
    public class GetTransactionByIdHandler : IRequestHandler<GetTransactionByIdQuery, Transaction?>
    {
        private readonly ITransactionRepository _repository;

        public GetTransactionByIdHandler(ITransactionRepository repository)
        {
            _repository = repository;
        }

        public Task<Transaction?> Handle(GetTransactionByIdQuery request, CancellationToken ct)
        {
            var transaction = _repository.GetById(request.Id);
            return Task.FromResult(transaction);
        }
    }
}