using AccountService.Interfaces;
using AccountService.Models;

namespace AccountService.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly List<Transaction> _transactions = new();
        private readonly object _lock = new();

        public IEnumerable<Transaction> GetByAccountId(Guid accountId)
            => _transactions.Where(t => t.AccountId == accountId).ToList();

        public void Add(Transaction transaction)
        {
            transaction.Id = Guid.NewGuid();
            transaction.DateTime = DateTime.UtcNow;
            _transactions.Add(transaction);
        }
        public Transaction? GetById(Guid id)
        {
            lock (_lock)
            {
                return _transactions.FirstOrDefault(t => t.Id == id);
            }
        }
    }
}
