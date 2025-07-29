using AccountService.Models;
using AccountService.Models.Dto;
using MediatR;
using Microsoft.AspNetCore.JsonPatch;

namespace AccountService.Features.Accounts.Commands
{
    public record PatchAccountCommand(
        Guid Id,
        JsonPatchDocument<UpdateAccountRequest> PatchDoc) : IRequest<Account?>;
}