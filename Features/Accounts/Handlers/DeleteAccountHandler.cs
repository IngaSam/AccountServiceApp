using AccountService.Features.Accounts.Commands;
using AccountService.Interfaces;
using MediatR;

namespace AccountService.Features.Accounts.Handlers
{
    public class DeleteAccountHandler : IRequestHandler<DeleteAccountCommand, bool>
    {
        private readonly IAccountRepository _repository;

        public DeleteAccountHandler(IAccountRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public Task<bool> Handle(DeleteAccountCommand request, CancellationToken cancellationToken)
        {
            var account = _repository.GetById(request.Id);
            if (account == null)
                return Task.FromResult(false);

            account.CloseDate = DateTime.UtcNow; // Мягкое удаление
            _repository.Update(account);
            return Task.FromResult(true);
        }
    }
}