using AccountService.Features.Accounts.Commands;
using AccountService.Interfaces;
using AccountService.Models;
using AccountService.Models.Enums;
using FluentValidation;
using MediatR;

namespace AccountService.Features.Accounts.Handlers
{
    public class CreateAccountHandler(
        IAccountRepository repository,
        IClientVerificationService clientVerification,
        ICurrencyService currencyService,
        IValidator<CreateAccountCommand> validator)
        : IRequestHandler<CreateAccountCommand, Account>
    {
        public async Task<Account> Handle( CreateAccountCommand request, CancellationToken cancellationToken)
        {
            // Валидация команды
            await validator.ValidateAndThrowAsync(request, cancellationToken);

            // Проверка бизнес-правил
            if (!await clientVerification.ClientExistsAsync(request.OwnerId))
                throw new ValidationException($"Client with id {request.OwnerId} not found");
            if (!currencyService.IsCurrencySupported(request.Currency))
                throw new ValidationException($"Currency {request.Currency} is not supported currency");


            // Создание счета
            var account = new Account
                {
                    Id = Guid.NewGuid(),
                    OwnerId = request.OwnerId,
                    Type = request.Type,
                    Currency = request.Currency.ToUpper(),
                     InterestRate = request.Type != AccountType.Checking
                        ? request.InterestRate
                        : null,
                OpenDate = DateTime.UtcNow,
                    Balance = 0

                };
                repository.Add(account);

                return account;

        }
    }
    
}
