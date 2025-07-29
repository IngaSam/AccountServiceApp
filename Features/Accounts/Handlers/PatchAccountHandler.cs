using AccountService.Features.Accounts.Commands;
using AccountService.Interfaces;
using AccountService.Models;
using AccountService.Models.Dto;
using MediatR;
using Microsoft.AspNetCore.JsonPatch;

namespace AccountService.Features.Accounts.Handlers
{
    public class PatchAccountHandler : IRequestHandler<PatchAccountCommand, Account?>
    {
        private readonly IAccountRepository _repository;

        public PatchAccountHandler(IAccountRepository repository)
        {
            _repository = repository;
        }

        public Task<Account?> Handle(PatchAccountCommand request, CancellationToken ct)
        {
            var account = _repository.GetById(request.Id);
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

            _repository.Update(account);
            return Task.FromResult<Account?>(account);
        }
    }
}