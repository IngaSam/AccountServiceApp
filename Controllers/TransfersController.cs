using AccountService.Features.Transfers.Commands;
using AccountService.Models.Dto;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AccountService.Controllers
{
    /// <summary>
    /// Управление переводами между счетами
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class TransfersController: ControllerBase
    {
        private readonly IMediator _mediator;

        public TransfersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Выполняет перевод между счетами
        /// </summary>
        /// <remarks>
        /// Пример запроса:
        /// 
        ///     POST /api/Transfers
        ///     {
        ///         "fromAccountId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        ///         "toAccountId": "3fa85f64-5717-4562-b3fc-2c963f66afa7",
        ///         "amount": 500.00,
        ///         "currency": "RUB",
        ///         "description": "Перевод на депозит"
        ///     }
        /// </remarks>
        /// <response code="200">Перевод выполнен успешно</response>
        /// <response code="400">Ошибка валидации или недостаточно средств</response>
        /// <response code="404">Один из счетов не найден</response>
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
