using AccountService.Features.Transactions.Commands;
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
    public class TransactionsController: ControllerBase
    {
        //private readonly IAccountRepository _accountRepository;
        //private readonly IValidator<CreateTransactionRequest> _validator;

        private readonly IMediator _mediator;

        public TransactionsController(IMediator _mediator)
            => _mediator = _mediator;
        
        [HttpPost]
        [ProducesResponseType(typeof(Transaction), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Create([FromBody] CreateTransactionRequest request)
        {
                var command = new CreateTransactionCommand(
                    request.AccountId,
                    request.Amount,
                    request.Type,
                    request.Description
                    );

                    var transaction = await _mediator.Send(command);
                return Created($"/transactions/{transaction.Id}", transaction);
            
           
        }
    }
}
