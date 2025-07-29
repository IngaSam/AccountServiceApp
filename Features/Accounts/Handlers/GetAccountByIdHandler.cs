using AccountService.Features.Accounts.Queries;
using AccountService.Interfaces;
using AccountService.Models;
using MediatR;

namespace AccountService.Features.Accounts.Handlers
{
    public class GetAccountByIdHandler : IRequestHandler<GetAccountByIdQuery, Account?>
    {
        private readonly IAccountRepository _repository;

        public GetAccountByIdHandler(IAccountRepository repository)
        {
            _repository = repository;
        }

        public Task<Account?> Handle(GetAccountByIdQuery request, CancellationToken ct)
        {
            var account = _repository.GetById(request.Id);
            return Task.FromResult(account);
        }
    }
}
