using AccountService.Features.Accounts.Commands;
using AccountService.Interfaces;
using AccountService.Models;
using FluentValidation;
using MediatR;

namespace AccountService.Features.Accounts.Handlers
{
    public class CreateAccountHandler : IRequestHandler<CreateAccountCommand, Account>
    {
        private readonly IAccountRepository _repository;
        private  readonly IClientVerificationService _clientVerification;
        private readonly ICurrencyService _currencyService;

        public CreateAccountHandler(
            IAccountRepository repository,
            IClientVerificationService clientVerification,
            ICurrencyService currencyService
        )
        {
            _repository =   repository;
            _clientVerification = clientVerification;
            _currencyService = currencyService;
        }

        public async Task<Account> Handle( CreateAccountCommand request, CancellationToken ct)
        {
            if (!await _clientVerification.ClientExistsAsync(request.OwnerId))
                throw new ValidationException("Client not found");
            if (!_currencyService.IsCurrencySupported(request.Currency))
                throw new ValidationException("Unsupported currency");
             

                
                var account = new Account
                {
                    Id = Guid.NewGuid(),
                    OwnerId = request.OwnerId,
                    Type = request.Type,
                    Currency = request.Currency,
                    InterestRate = request.InterestRate,
                    OpenDate = DateTime.UtcNow,
                    Balance = 0

                };
                _repository.Add(account);

                return account;

        }
    }
    
}
