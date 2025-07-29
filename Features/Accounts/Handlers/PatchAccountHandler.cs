using AccountService.Features.Accounts.Commands;
using AccountService.Interfaces;
using AccountService.Models;
using AccountService.Models.Dto;
using MediatR;

namespace AccountService.Features.Accounts.Handlers
{
    public class PatchAccountHandler(IAccountRepository repository) : IRequestHandler<PatchAccountCommand, Account?>
    {
        public Task<Account?> Handle(PatchAccountCommand request, CancellationToken ct)
        {
            var account = repository.GetById(request.Id);
            if (account == null) return Task.FromResult<Account?>(null);

            var patchDoc = request.PatchDoc;
            var accountToPatch = new UpdateAccountRequest
            {
                InterestRate = account.InterestRate,
                CloseDate = account.CloseDate
            };

            patchDoc.ApplyTo(accountToPatch);

            // Применяем изменения
            if (accountToPatch.InterestRate.HasValue)
                account.InterestRate = accountToPatch.InterestRate.Value;

            if (accountToPatch.CloseDate.HasValue)
                account.CloseDate = accountToPatch.CloseDate.Value;

            repository.Update(account);
            return Task.FromResult<Account?>(account);
        }
    }
}