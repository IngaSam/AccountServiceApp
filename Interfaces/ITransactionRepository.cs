using AccountService.Models;

namespace AccountService.Interfaces
{
    public interface ITransactionRepository
    {
        Transaction? GetById(Guid id);
        IEnumerable<Transaction> GetByAccountId(Guid accountId);
        void Add(Transaction transaction);
    }
}
