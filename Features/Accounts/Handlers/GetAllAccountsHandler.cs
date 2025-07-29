using AccountService.Features.Accounts.Queries;
using AccountService.Interfaces;
using AccountService.Models;
using MediatR;

namespace AccountService.Features.Accounts.Handlers
{
    public class GetAllAccountsHandler(IAccountRepository repository) :
        IRequestHandler<GetAllAccountsQuery, IEnumerable<Account>>
    {
        public Task<IEnumerable<Account>> Handle(
            GetAllAccountsQuery request,
            CancellationToken cancellationToken)
        {
            var query = repository.GetAll().AsQueryable();

            if (!string.IsNullOrEmpty(request.Currency))
                query = query.Where(a => a.Currency == request.Currency);
            
            if (request.Type.HasValue)
                query = query.Where(a => a.Type == request.Type.Value);

            return Task.FromResult<IEnumerable<Account>>(query
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                //.AsEnumerable();
                .ToList());
        }
    }
}

