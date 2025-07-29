using AccountService.Models;
using AccountService.Models.Enums;

namespace AccountService.Interfaces
{
    public interface IAccountRepository
    {
        //Основные CRUD-операции
        IEnumerable<Account> GetAll();
        Account? GetById(Guid id);
        //Task<Account?> GetByIdAsync(Guid id);
        void Add(Account account);
        void Update(Account account);
        void Delete(Guid id);

        //Специфичные методы 
        bool Exists(Guid id);
        IEnumerable<Account> GetByOwnerId(Guid ownerId);
        IEnumerable<Account> GetByCurrency(string currency);
        IEnumerable<Account> GetByType(AccountType type);
       
    }
}
