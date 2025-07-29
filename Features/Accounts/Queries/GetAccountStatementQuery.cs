using AccountService.Models;
using MediatR;

namespace AccountService.Features.Accounts.Queries
{
    public record GetAccountStatementQuery(
        Guid AccountId,
        DateTime? FromDate
    ) : IRequest<AccountStatement>;
}
