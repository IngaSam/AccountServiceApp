using AccountService.Interfaces;
using AccountService.Models.Dto;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace AccountService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransfersController: ControllerBase
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IValidator<TransferRequest> _validator;

        public TransfersController(
            IAccountRepository accountRepository,
            IValidator<TransferRequest> validator)
        {
            _accountRepository = accountRepository;
            _validator = validator;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails),StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Transfer([FromBody] TransferRequest request)
        {
            try
            {
                var validationResult = await _validator.ValidateAsync(request);
                if (!validationResult.IsValid)
                {
                    return ValidationProblem(new ValidationProblemDetails(validationResult.ToDictionary()));
                }

                var fromAccount = _accountRepository.GetById(request.FromAccountId);
                var toAccount = _accountRepository.GetById(request.ToAccountId);

                if (fromAccount == null || toAccount == null) 
                    return NotFound("One of account not found");

                if (fromAccount.Balance < request.Amount)
                    return BadRequest("Insufficient funds");

                fromAccount.Balance -= request.Amount;
                toAccount.Balance += request.Amount;

                _accountRepository.Update(fromAccount);
                _accountRepository.Update(toAccount);


                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
