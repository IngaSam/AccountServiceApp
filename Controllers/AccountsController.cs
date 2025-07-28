using AccountService.Interfaces;
using AccountService.Models;
using AccountService.Models.Dto;
using AccountService.Models.Enums;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace AccountService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountsController : ControllerBase
    {
            private readonly IAccountRepository _repository;
            private readonly IValidator<CreateAccountRequest> _validator;

            public AccountsController(
                IAccountRepository repository,
                IValidator<CreateAccountRequest> validator)
            {
                _repository = repository;
                _validator = validator;
            }

            [HttpGet]
            public IActionResult GetAll(
                [FromQuery] string? currency,
                [FromQuery] AccountType? type,
                [FromQuery] int page = 1,
                [FromQuery] int pageSize = 10)
            {
                var query  = _repository.GetAll().AsQueryable();

                //фильтрация(если параметры указаны)
                if (currency != null)
                    query = query.Where(a => a.Currency == currency);
                if (type != null)
                query = query.Where(a => a.Type == type);

                var result = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();
                return Ok(new { data = result, page, pageSize});
            }

            [HttpGet("{id}")]
            [ProducesResponseType(typeof(Account), StatusCodes.Status200OK)]
            [ProducesResponseType(StatusCodes.Status404NotFound)]
            public IActionResult GetById(Guid id)
            {
                var account = _repository.GetById(id);
                return account != null ? Ok(account) : NotFound();
            }

            [HttpPost]
            [ProducesResponseType(typeof(Account), StatusCodes.Status201Created)]
            [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
            public async Task<IActionResult> Create([FromBody] CreateAccountRequest request)
            {
                    try
                    {
                        var validationResult = await _validator.ValidateAsync(request);
                        if (!validationResult.IsValid)
                        {
                            return ValidationProblem(new ValidationProblemDetails(validationResult.ToDictionary()));
                        }

                        // Логика создания счёта
                        var account = new Account
                        {
                            Id = Guid.NewGuid(),
                            OwnerId = request.OwnerId,
                            Type = request.Type,
                            Currency = request.Currency,
                            InterestRate = request.InterestRate,
                            OpenDate = DateTime.UtcNow,
                            Balance = 0

                        };
                        _repository.Add(account);

                        return Created($"/accounts/{account.Id}", account);
                    }
                    catch (Exception ex)
                    {
                        return StatusCode(500, new { error = ex.Message });
                    }
            }

            [HttpPut("{id}")]
            [ProducesResponseType(typeof(Account), StatusCodes.Status200OK)]
            [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
            [ProducesResponseType(StatusCodes.Status404NotFound)]
            public IActionResult Update(Guid id,
                [FromBody] UpdateAccountRequest request)
            {
                var account = _repository.GetById(id);
                if (account == null) return NotFound();

                account.InterestRate = request.InterestRate;
                account.CloseDate = request.CloseDate;

                _repository.Update(account);
                return Ok(account);
            }

            [HttpDelete("{id}")]
            [ProducesResponseType(StatusCodes.Status204NoContent)]
            [ProducesResponseType(StatusCodes.Status404NotFound)]
            public IActionResult Delete(Guid id)
            {
            var account = _repository.GetById(id);
            if (account == null) return NotFound();

            _repository.Delete(id);
            return NoContent();
            }
    }
}
