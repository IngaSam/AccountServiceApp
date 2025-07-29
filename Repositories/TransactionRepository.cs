using AccountService.Interfaces;
using AccountService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AccountService.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly List<Transaction> _transactions = [];
        private readonly object _lock = new();

        public IEnumerable<Transaction> GetByAccountId(Guid accountId)
        {
            lock (_lock)
            {
                return _transactions
                    .Where(t => t.AccountId == accountId)
                    .ToList();
            }
        }

        public void Add(Transaction transaction)
        {
            lock (_lock)
            {
                transaction.Id = Guid.NewGuid();
                transaction.DateTime = DateTime.UtcNow;
                _transactions.Add(transaction);
            }
        }

        public Transaction? GetById(Guid id)
        {
            lock (_lock)
            {
                return _transactions.FirstOrDefault(t => t.Id == id);
            }
        }

        public Task<List<Transaction>> GetByAccountIdAndDateRangeAsync(
            Guid accountId,
            DateTime fromDate,
            DateTime toDate)
        {
            lock (_lock)
            {
                var result = _transactions
                    .Where(t => t.AccountId == accountId &&
                                t.DateTime >= fromDate &&
                                t.DateTime <= toDate)
                    .OrderBy(t => t.DateTime)
                    .ToList();

                return Task.FromResult(result);
            }
        }
    }
}