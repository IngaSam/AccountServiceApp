using AccountService.Interfaces;
using MediatR;

namespace AccountService.Features.Transfers.Commands
{
    public class TransferCommandHandler(
        IAccountRepository repository,
        ILogger<TransferCommandHandler> logger)
        : IRequestHandler<TransferCommand, TransferResult>
    {
        public Task<TransferResult> Handle(TransferCommand request, CancellationToken ct)
        {
            logger.LogInformation($"Transfer from {request.FromAccountId} to {request.ToAccountId}");

            var fromAccount = repository.GetById(request.FromAccountId);
            if (fromAccount == null)
            {
                logger.LogWarning($"FromAccount not found: {request.FromAccountId}");
                return Task.FromResult(TransferResult.AccountNotFound);
            }

            var toAccount = repository.GetById(request.ToAccountId);
            if (toAccount == null)
            {
                logger.LogWarning($"ToAccount not found: {request.ToAccountId}");
                return Task.FromResult(TransferResult.AccountNotFound);
            }

            if (fromAccount.Id == toAccount.Id)
            {
                logger.LogWarning("Cannot transfer to same account");
                return Task.FromResult(TransferResult.SameAccount);
            }

            if (fromAccount.Currency != toAccount.Currency)
            {
                logger.LogWarning($"Currency mismatch: {fromAccount.Currency} != {toAccount.Currency}");
                return Task.FromResult(TransferResult.CurrencyMismatch);
            }

            if (fromAccount.Balance < request.Amount)
            {
                logger.LogWarning($"Insufficient funds: {fromAccount.Balance} < {request.Amount}");
                return Task.FromResult(TransferResult.InsufficientFunds);
            }

            // Выполняем перевод
            fromAccount.Balance -= request.Amount;
            toAccount.Balance += request.Amount;

            repository.Update(fromAccount);
            repository.Update(toAccount);

            logger.LogInformation($"Transfer completed: {request.Amount} {fromAccount.Currency}");
            return Task.FromResult(TransferResult.Success);
        }
    }
}
