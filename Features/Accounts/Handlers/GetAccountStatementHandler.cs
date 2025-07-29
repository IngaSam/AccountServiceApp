using AccountService.Features.Accounts.Queries;
using AccountService.Interfaces;
using AccountService.Models;
using MediatR;

namespace AccountService.Features.Accounts.Handlers
{
    public class GetAccountStatementHandler(
        IAccountRepository accountRepository,
        ITransactionRepository transactionRepository)
        : IRequestHandler<GetAccountStatementQuery, AccountStatement>
    {
        public Task<AccountStatement> Handle(GetAccountStatementQuery request, CancellationToken ct)
        {
            var account = accountRepository.GetById(request.AccountId);
            if (account == null) return Task.FromResult<AccountStatement>(null!);

            var transactions = transactionRepository
                .GetByAccountId(request.AccountId)
                .Where(t => request.FromDate == null || t.DateTime >= request.FromDate)
                .ToList();

            return Task.FromResult(new AccountStatement
            {
                AccountId = account.Id,
                PeriodStart = request.FromDate ?? account.OpenDate,
                PeriodEnd = DateTime.UtcNow,
                OpeningBalance = request.FromDate == null ? 0 : CalculateOpeningBalance(),
                ClosingBalance = account.Balance,
                Transactions = transactions
            });
        }

        private decimal CalculateOpeningBalance()
        {
            // Реализуйте логику расчета начального баланса
            // Например: сумма всех транзакций до fromDate
            return 0; // Заглушка
        }
    }
}