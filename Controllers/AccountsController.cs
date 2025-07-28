using AccountService.Interfaces;
using AccountService.Models;
using AccountService.Models.Dto;
using AccountService.Models.Enums;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using AccountService.Features.Accounts.Commands;
using AccountService.Features.Accounts.Queries;
using MediatR;

namespace AccountService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountsController : ControllerBase
    {
            private readonly IMediator _mediator;

            public AccountsController(IMediator mediator)
            {
                _mediator = mediator;
            }

            [HttpGet]
            [ProducesResponseType(typeof(IEnumerable<Account>), StatusCodes.Status200OK)]
            public async Task<IActionResult> GetAll(
                    [FromQuery] string? currency,
                    [FromQuery] AccountType? type,
                    [FromQuery] int page = 1,
                    [FromQuery] int pageSize = 10)
            {
                var query = new GetAllAccountsQuery(currency, type, page, pageSize);
                var result = await _mediator.Send(query);
                return Ok(result);
            }

            [HttpGet("{id}")]
            [ProducesResponseType(typeof(Account), StatusCodes.Status200OK)]
            [ProducesResponseType(StatusCodes.Status404NotFound)]
            public async Task<IActionResult> GetById(Guid id)
            {
                var query = new GetAccountByIdQuery(id);
                var account = await _mediator.Send(query);
                return account != null ? Ok(account) : NotFound();
            }

            [HttpPost]
            [ProducesResponseType(typeof(Account), StatusCodes.Status201Created)]
            [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
            public async Task<IActionResult> Create([FromBody] CreateAccountRequest request)
            {
                    // Логика создания счёта
                    var command = new CreateAccountCommand(
                        request.OwnerId,
                        request.Type,
                        request.Currency,
                        request.InterestRate
                    );
                    var account = await _mediator.Send(command);
                    return Created($"/accounts/{account.Id}", account);
                    
            }

            [HttpPut("{id}")]
            [ProducesResponseType(typeof(Account), StatusCodes.Status200OK)]
           // [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
            [ProducesResponseType(StatusCodes.Status404NotFound)]
            public async Task<IActionResult> Update(Guid id,
                [FromBody] UpdateAccountRequest request)
            {
                var command = new UpdateAccountCommand(id, request.InterestRate, request.CloseDate);
                var account = await _mediator.Send(command);
                return account != null ? Ok(account) : NotFound();
            }

            [HttpDelete("{id}")]
            [ProducesResponseType(StatusCodes.Status204NoContent)]
            [ProducesResponseType(StatusCodes.Status404NotFound)]
            public async Task<IActionResult> Delete(Guid id)
            {
            var command = new DeleteAccountCommand(id);
            var result = await _mediator.Send(command);
            return result ? NoContent() : NotFound();
            }
    }
}
