using AccountService.Models;
using AccountService.Models.Enums;
using MediatR;

namespace AccountService.Features.Accounts.Queries
{
    public record GetAllAccountsQuery(
        string? Currency,
        AccountType? Type,
        int Page = 1,
        int PageSize = 10) : IRequest<IEnumerable<Account>>;
    
}
