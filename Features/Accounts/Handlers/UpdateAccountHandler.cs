﻿using AccountService.Features.Accounts.Commands;
using AccountService.Interfaces;
using AccountService.Models;
using MediatR;

namespace AccountService.Features.Accounts.Handlers
{
    public class UpdateAccountHandler(IAccountRepository repository) : IRequestHandler<UpdateAccountCommand, Account?>
    {
        public Task<Account?> Handle(UpdateAccountCommand request, CancellationToken ct)
        {
            var account = repository.GetById(request.Id);
            if (account == null) return Task.FromResult<Account?>(null);

            // Обновляем только разрешенные поля
            if (request.InterestRate.HasValue)
                account.InterestRate = request.InterestRate.Value;

            if (request.CloseDate.HasValue)
                account.CloseDate = request.CloseDate.Value;

            repository.Update(account);
            return Task.FromResult<Account?>(account);
        }
    }
}