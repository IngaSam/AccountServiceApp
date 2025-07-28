using AccountService.Models;
using MediatR;

namespace AccountService.Features.Accounts.Queries
{
    public record GetAccountByIdQuery(Guid Id) : IRequest<Account?>;

}
