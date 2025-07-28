using AccountService.Interfaces;
using AccountService.Models;
using AccountService.Models.Dto;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace AccountService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionsController: ControllerBase
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IValidator<CreateTransactionRequest> _validator;

        public TransactionsController(
            IAccountRepository accountRepository,
            IValidator<CreateTransactionRequest> validator)
        {
            _accountRepository = accountRepository;
            _validator = validator;
        }

        [HttpPost]
        [ProducesResponseType(typeof(Transaction), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Create([FromBody] CreateTransactionRequest request)
        {
            try
            {
                var validationResult = await _validator.ValidateAsync(request);
                if (!validationResult.IsValid)
                {
                    return ValidationProblem(new ValidationProblemDetails(validationResult.ToDictionary()));
                }

                var account = _accountRepository.GetById(request.AccountId);
                if (account == null) return NotFound("Account not found");
               

                var transaction= new Transaction
                {
                    Id = Guid.NewGuid(),
                    Amount = request.Amount,
                    Type = request.Type,
                    Description = request.Description,
                    DateTime = DateTime.UtcNow,
                    

                };
                account.Transactions.Add(transaction);
                _accountRepository.Update(account);
                return Created($"/transactions/{transaction.Id}", transaction);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
