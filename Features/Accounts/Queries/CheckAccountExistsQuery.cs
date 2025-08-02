using MediatR;

namespace AccountService.Features.Accounts.Queries
{
    public record CheckAccountExistsQuery(Guid Id) : IRequest<bool>;
}
