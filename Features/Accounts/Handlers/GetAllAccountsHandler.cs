using AccountService.Features.Accounts.Queries;
using AccountService.Interfaces;
using AccountService.Models;
using MediatR;

namespace AccountService.Features.Accounts.Handlers
{
    public class GetAllAccountsHandler :
        IRequestHandler<GetAllAccountsQuery, IEnumerable<Account>>
    {
        private readonly IAccountRepository _repository;

        public GetAllAccountsHandler(IAccountRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<Account>> Handle(
            GetAllAccountsQuery request,
            CancellationToken cancellationToken)
        {
            var query = _repository.GetAll().AsQueryable();

            if (!string.IsNullOrEmpty(request.Currency))
                query = query.Where(a => a.Currency == request.Currency);
            
            if (request.Type.HasValue)
                query = query.Where(a => a.Type == request.Type.Value);

            return  query
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                //.AsEnumerable();
                .ToList();
        }
    }
}

