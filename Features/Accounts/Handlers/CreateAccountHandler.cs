using AccountService.Features.Accounts.Commands;
using AccountService.Interfaces;
using AccountService.Models;
using AccountService.Models.Enums;
using FluentValidation;
using MediatR;

namespace AccountService.Features.Accounts.Handlers
{
    public class CreateAccountHandler : IRequestHandler<CreateAccountCommand, Account>
    {
        private readonly IAccountRepository _repository;
        private  readonly IClientVerificationService _clientVerification;
        private readonly ICurrencyService _currencyService;
        private readonly IValidator<CreateAccountCommand> _validator;

        public CreateAccountHandler(
            IAccountRepository repository,
            IClientVerificationService clientVerification,
            ICurrencyService currencyService,
            IValidator<CreateAccountCommand> validator
        )
        {
            _repository =   repository;
            _clientVerification = clientVerification;
            _currencyService = currencyService;
            _validator = validator;
        }

        public async Task<Account> Handle( CreateAccountCommand request, CancellationToken cancellationToken)
        {
            // Валидация команды
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            // Проверка бизнес-правил
            if (!await _clientVerification.ClientExistsAsync(request.OwnerId))
                throw new ValidationException($"Client with id {request.OwnerId} not found");
            if (!_currencyService.IsCurrencySupported(request.Currency))
                throw new ValidationException($"Currency {request.Currency} is not supported currency");


            // Создание счета
            var account = new Account
                {
                    Id = Guid.NewGuid(),
                    OwnerId = request.OwnerId,
                    Type = request.Type,
                    Currency = request.Currency,
                     InterestRate = request.Type != AccountType.Checking
                        ? request.InterestRate
                        : null,
                OpenDate = DateTime.UtcNow,
                    Balance = 0

                };
                _repository.Add(account);

                return account;

        }
    }
    
}
