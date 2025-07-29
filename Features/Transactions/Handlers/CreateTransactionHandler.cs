using AccountService.Features.Transactions.Commands;
using AccountService.Interfaces;
using AccountService.Models;
using AccountService.Models.Enums;
using MediatR;

namespace AccountService.Features.Transactions.Handlers
{
    public class CreateTransactionHandler(
        ITransactionRepository transactionRepository,
        IAccountRepository accountRepository,
        ILogger<CreateTransactionHandler> logger)
        : IRequestHandler<CreateTransactionCommand, Transaction>
    {
        private readonly ITransactionRepository _transactionRepository = transactionRepository ??
                                                                         throw new ArgumentNullException(nameof(transactionRepository));
        private readonly IAccountRepository _accountRepository = accountRepository ??
                                                                 throw new ArgumentNullException(nameof(accountRepository));
        private readonly ILogger<CreateTransactionHandler> _logger = logger ??
                                                                     throw new ArgumentNullException(nameof(logger));

        public Task<Transaction> Handle(CreateTransactionCommand request, CancellationToken ct)
        {
            // 1. Получаем счет
            var account = _accountRepository.GetById(request.AccountId);
            if (account == null)
            {
                _logger.LogError($"Account {request.AccountId} not found");
                throw new KeyNotFoundException($"Account {request.AccountId} not found");
            }

            // 2. Создаем транзакцию
            var transaction = new Transaction
            {
                Id = Guid.NewGuid(),
                AccountId = request.AccountId,
                Amount = request.Amount,
                Currency = account.Currency,
                Type = request.Type,
                Description = request.Description,
                DateTime = DateTime.UtcNow
            };

            // 3. Обновляем баланс счета
            account.Balance = request.Type == TransactionType.Credit
                ? account.Balance + request.Amount
                : account.Balance - request.Amount;

            // 4. Сохраняем изменения
            _transactionRepository.Add(transaction);
            _accountRepository.Update(account);

            _logger.LogInformation($"Created transaction {transaction.Id}");
            return Task.FromResult(transaction);
        }
    }
}