using AccountService.Features.Transactions.Commands;
using AccountService.Features.Transactions.Queries;
using AccountService.Interfaces;
using AccountService.Models;
using AccountService.Models.Dto;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AccountService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TransactionsController(IMediator mediator)
        {
            _mediator = mediator;
        }

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

                var transaction = await _mediator.Send(command);
                return Created($"/api/transactions/{transaction.Id}", transaction);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Transaction), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var query = new GetTransactionByIdQuery(id);
            var transaction = await _mediator.Send(query);
            return transaction != null ? Ok(transaction) : NotFound();
        }
    }
}