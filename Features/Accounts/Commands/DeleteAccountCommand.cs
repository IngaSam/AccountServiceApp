using MediatR;

namespace AccountService.Features.Accounts.Commands
{
    public record DeleteAccountCommand(Guid Id) : IRequest<bool>;
    
}
