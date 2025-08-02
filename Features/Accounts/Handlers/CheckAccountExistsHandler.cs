using AccountService.Features.Accounts.Queries;
using AccountService.Interfaces;
using MediatR;

namespace AccountService.Features.Accounts.Handlers
{
    public class CheckAccountExistsHandler : IRequestHandler<CheckAccountExistsQuery, bool>
    {
        private readonly IAccountRepository _repository;
        public CheckAccountExistsHandler(IAccountRepository repository)
            => _repository = repository;

        public Task<bool> Handle(CheckAccountExistsQuery request, CancellationToken ct)
            => Task.FromResult(_repository.Exists(request.Id));
    }
}
