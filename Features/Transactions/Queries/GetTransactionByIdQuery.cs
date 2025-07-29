using MediatR;
using AccountService.Models;

namespace AccountService.Features.Transactions.Queries
{
    public record GetTransactionByIdQuery(Guid Id) : IRequest<Transaction?>;
}