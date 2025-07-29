using AccountService.Exceptions;
using AccountService.Features.Accounts.Queries;
using AccountService.Interfaces;
using AccountService.Models;
using AccountService.Models.Enums;
using MediatR;

public class GetAccountStatementQueryHandler : IRequestHandler<GetAccountStatementQuery, AccountStatement>
{
    private readonly IAccountRepository _accountRepository;
    private readonly ITransactionRepository _transactionRepository;

    public GetAccountStatementQueryHandler(
        IAccountRepository accountRepository,
        ITransactionRepository transactionRepository)
    {
        _accountRepository = accountRepository;
        _transactionRepository = transactionRepository;
    }

    public async Task<AccountStatement> Handle(
        GetAccountStatementQuery request,
        CancellationToken cancellationToken)
    {
        // Используем синхронный GetById вместо GetByIdAsync
        var account = _accountRepository.GetById(request.AccountId)
            ?? throw new AccountNotFoundException(request.AccountId);

        // Устанавливаем период по умолчанию (последние 30 дней)
        var fromDate = request.FromDate ?? DateTime.UtcNow.AddDays(-30);
        var toDate = request.ToDate ?? DateTime.UtcNow;

        if (fromDate > toDate)
        {
            throw new ArgumentException("Дата начала периода не может быть позже даты окончания");
        }

        var transactions = await _transactionRepository
            .GetByAccountIdAndDateRangeAsync(request.AccountId, fromDate, toDate);

        var openingBalance = await CalculateOpeningBalance(request.AccountId, fromDate);
        var closingBalance = openingBalance + CalculateBalanceChange(transactions);

        return new AccountStatement
        {
            AccountId = account.Id,
            PeriodStart = fromDate,
            PeriodEnd = toDate,
            OpeningBalance = openingBalance,
            ClosingBalance = closingBalance,
            Transactions = transactions
        };
    }

    private async Task<decimal> CalculateOpeningBalance(Guid accountId, DateTime date)
    {
        var previousTransactions = await _transactionRepository
            .GetByAccountIdAndDateRangeAsync(accountId, DateTime.MinValue, date.AddDays(-1));

        return previousTransactions.Sum(t =>
            t.Type == TransactionType.Credit ? t.Amount : -t.Amount);
    }

    private decimal CalculateBalanceChange(IEnumerable<Transaction> transactions)
    {
        return transactions.Sum(t =>
            t.Type == TransactionType.Credit ? t.Amount : -t.Amount);
    }
}