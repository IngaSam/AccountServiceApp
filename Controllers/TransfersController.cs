using AccountService.Features.Transfers.Commands;
using AccountService.Models.Dto;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AccountService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransfersController: ControllerBase
    {
        private readonly IMediator _mediator;

        public TransfersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Transfer([FromBody] TransferRequest request)
        {
            var command = new TransferCommand(
                request.FromAccountId,
                request.ToAccountId,
                request.Amount);

            var result = await _mediator.Send(command);

            return result switch
            {
                TransferResult.Success => Ok(),
                TransferResult.AccountNotFound => NotFound("One of accounts not found"),
                TransferResult.InsufficientFunds => BadRequest("Insufficient funds"),
                TransferResult.SameAccount => BadRequest("Cannot transfer to same account"),
                TransferResult.CurrencyMismatch => BadRequest("Currency mismatch"),
                _ => StatusCode(500)
            };
        }

    }
}
