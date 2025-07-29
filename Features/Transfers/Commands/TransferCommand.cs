using MediatR;

namespace AccountService.Features.Transfers.Commands
{
    public record TransferCommand(
        Guid FromAccountId,
        Guid ToAccountId,
        decimal Amount) : IRequest<TransferResult>;

}
