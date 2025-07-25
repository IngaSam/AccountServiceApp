using AccountService.Interfaces;
using AccountService.Models;
using AccountService.Models.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace AccountService.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly List<Account> _accounts = new();
        private readonly object _lock = new();

        public IEnumerable<Account> GetAll()
        {
            lock (_lock)
            {
                return _accounts.ToList();
            }
        }

        public Account? GetById(Guid id)
        {
            lock (_lock)
            {
                return _accounts.FirstOrDefault(a => a.Id == id);
            }
        }

        public void Add(Account account)
        {
            if (account == null)
                throw new ArgumentNullException(nameof(account));
            lock (_lock)
            {
                _accounts.Add(account);
            }
        }

        public void Update(Account account)
        {
            if (account == null)
                throw new ArgumentNullException(nameof(account));
            lock (_lock)
            {
                var index = _accounts.FindIndex(a => a.Id == account.Id);
                if (index >= 0)
                    _accounts[index] = account;

            }
        }

        public void Delete(Guid id)
        {
            lock (_lock)
            {
                var account = _accounts.FirstOrDefault(a => a.Id == id);
                if (account != null)
                {
                    account.CloseDate = DateTime.UtcNow; //Мягкое удаление 
                }
            }
        }

        public bool Exists(Guid id)
        {
            lock (_lock)
            {
                return _accounts.Any(a => a.Id == id);
            }
        }

        public IEnumerable<Account> GetByOwnerId(Guid ownerId)
        {
            lock (_lock)
            {
                return _accounts.Where(a => a.OwnerId == ownerId).ToList();
            }
        }

        public IEnumerable<Account> GetByCurrency(string currency)
        {
            if (string.IsNullOrWhiteSpace(currency))
                throw new ArgumentException("Currency cannot be empty ", nameof(currency));
            lock (_lock)
            {
                return _accounts.Where(a => a.Currency.Equals(currency, StringComparison.OrdinalIgnoreCase)).ToList();
            }
        }

        public IEnumerable<Account> GetByType(AccountType type)
        {
            lock (_lock)
            {
                return _accounts.Where(a => a.Type == type).ToList();
            }
        }
    }
}
