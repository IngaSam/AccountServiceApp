using AccountService.Interfaces;
using MediatR;

namespace AccountService.Features.Transfers.Commands
{
    public class TransferCommandHandler : IRequestHandler<TransferCommand, TransferResult>
    {
        private readonly IAccountRepository _repository;
        private readonly ILogger<TransferCommandHandler> _logger;

        public TransferCommandHandler(
            IAccountRepository repository,
            ILogger<TransferCommandHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<TransferResult> Handle(TransferCommand request, CancellationToken ct)
        {
            _logger.LogInformation($"Transfer from {request.FromAccountId} to {request.ToAccountId}");

            var fromAccount = _repository.GetById(request.FromAccountId);
            if (fromAccount == null)
            {
                _logger.LogWarning($"FromAccount not found: {request.FromAccountId}");
                return TransferResult.AccountNotFound;
            }

            var toAccount = _repository.GetById(request.ToAccountId);
            if (toAccount == null)
            {
                _logger.LogWarning($"ToAccount not found: {request.ToAccountId}");
                return TransferResult.AccountNotFound;
            }

            if (fromAccount.Id == toAccount.Id)
            {
                _logger.LogWarning("Cannot transfer to same account");
                return TransferResult.SameAccount;
            }

            if (fromAccount.Currency != toAccount.Currency)
            {
                _logger.LogWarning($"Currency mismatch: {fromAccount.Currency} != {toAccount.Currency}");
                return TransferResult.CurrencyMismatch;
            }

            if (fromAccount.Balance < request.Amount)
            {
                _logger.LogWarning($"Insufficient funds: {fromAccount.Balance} < {request.Amount}");
                return TransferResult.InsufficientFunds;
            }

            // Выполняем перевод
            fromAccount.Balance -= request.Amount;
            toAccount.Balance += request.Amount;

            _repository.Update(fromAccount);
            _repository.Update(toAccount);

            _logger.LogInformation($"Transfer completed: {request.Amount} {fromAccount.Currency}");
            return TransferResult.Success;
        }
    }
}
