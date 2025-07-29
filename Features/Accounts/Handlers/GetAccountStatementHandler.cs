using AccountService.Features.Accounts.Queries;
using AccountService.Interfaces;
using AccountService.Models;
using MediatR;

namespace AccountService.Features.Accounts.Handlers
{
    public class GetAccountStatementHandler : IRequestHandler<GetAccountStatementQuery, AccountStatement>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly ITransactionRepository _transactionRepository;

        public GetAccountStatementHandler(
            IAccountRepository accountRepository,
            ITransactionRepository transactionRepository)
        {
            _accountRepository = accountRepository;
            _transactionRepository = transactionRepository;
        }

        public async Task<AccountStatement> Handle(GetAccountStatementQuery request, CancellationToken ct)
        {
            var account = _accountRepository.GetById(request.AccountId);
            if (account == null) return null;

            var transactions = _transactionRepository
                .GetByAccountId(request.AccountId)
                .Where(t => request.FromDate == null || t.DateTime >= request.FromDate)
                .ToList();

            return new AccountStatement
            {
                AccountId = account.Id,
                PeriodStart = request.FromDate ?? account.OpenDate,
                PeriodEnd = DateTime.UtcNow,
                OpeningBalance = request.FromDate == null ? 0 : CalculateOpeningBalance(request.AccountId, request.FromDate.Value),
                ClosingBalance = account.Balance,
                Transactions = transactions
            };
        }

        private decimal CalculateOpeningBalance(Guid accountId, DateTime fromDate)
        {
            // Реализуйте логику расчета начального баланса
            // Например: сумма всех транзакций до fromDate
            return 0; // Заглушка
        }
    }
}