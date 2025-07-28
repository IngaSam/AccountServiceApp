using AccountService.Models;
using AccountService.Models.Enums;
using MediatR;

namespace AccountService.Features.Transactions.Commands
{
    public record CreateTransactionCommand(
        Guid AccountId,
        decimal Amount,
        TransactionType Type,
        string Description) : IRequest<Transaction>;
}
