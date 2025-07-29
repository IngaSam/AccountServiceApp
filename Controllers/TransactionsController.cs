using AccountService.Features.Transactions.Commands;
using AccountService.Features.Transactions.Queries;
using AccountService.Models;
using AccountService.Models.Dto;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AccountService.Controllers
{
    /// <summary>
    /// Управление финансовыми транзакциями
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionsController(IMediator mediator) : ControllerBase
    {
        /// <summary>
        /// Создает новую транзакцию
        /// </summary>
        /// <remarks>
        /// Пример запроса:
        /// 
        ///     POST /api/Transactions
        ///     {
        ///         "accountId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        ///         "amount": 1000.50,
        ///         "currency": "RUB",
        ///         "type": "Credit",
        ///         "description": "Пополнение счета"
        ///     }
        /// </remarks>
        /// <response code="201">Транзакция успешно создана</response>
        /// <response code="400">Некорректные данные</response>
        /// <response code="404">Счет не найден</response>
        [HttpPost]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(Transaction), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Create([FromBody] CreateTransactionRequest request)
        {
            try
            {
                var command = new CreateTransactionCommand(
                    request.AccountId,
                    request.Amount,
                    request.Type,
                    request.Description);

                var transaction = await mediator.Send(command);
                return Created($"/api/transactions/{transaction.Id}", transaction);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Получает информацию о транзакции по ID
        /// </summary>
        /// <param name="id">Идентификатор транзакции</param>
        /// <response code="200">Возвращает запрошенную транзакцию</response>
        /// <response code="404">Транзакция не найдена</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Transaction), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var query = new GetTransactionByIdQuery(id);
            var transaction = await mediator.Send(query);
            return transaction != null ? Ok(transaction) : NotFound();
        }
    }
}