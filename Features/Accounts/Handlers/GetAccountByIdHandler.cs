using AccountService.Features.Accounts.Queries;
using AccountService.Interfaces;
using AccountService.Models;
using MediatR;

namespace AccountService.Features.Accounts.Handlers
{
    public class GetAccountByIdHandler(IAccountRepository repository) : IRequestHandler<GetAccountByIdQuery, Account?>
    {
        public Task<Account?> Handle(GetAccountByIdQuery request, CancellationToken ct)
        {
            var account = repository.GetById(request.Id);
            return Task.FromResult(account);
        }
    }
}
