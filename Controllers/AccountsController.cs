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
using Microsoft.AspNetCore.JsonPatch;

namespace AccountService.Controllers
{
    /// <summary>
    /// Контроллер для управления банковскими счетами
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AccountsController : ControllerBase
    {
            private readonly IMediator _mediator;

            public AccountsController(IMediator mediator)
            {
                _mediator = mediator;
            }

            /// <summary>
            /// Получает список всех счетов с возможностью фильтрации
            /// </summary>
            /// <param name="currency">Фильтр по валюте (например, "RUB")</param>
            /// <param name="type">Тип счета (Checking, Deposit, Credit)</param>
            /// <param name="page">Номер страницы (по умолчанию 1)</param>
            /// <param name="pageSize">Размер страницы (по умолчанию 10)</param>
            /// <returns>Список счетов</returns>
            /// <response code="200">Успешный запрос</response>
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

            /// <summary>
            /// Получает информацию о конкретном счете по ID
            /// </summary>
            /// <param name="id">Идентификатор счета</param>
            /// <returns>Информация о счете</returns>
            /// <response code="200">Счет найден</response>
            /// <response code="404">Счет не найден</response>
        [HttpGet("{id}")]
            [ProducesResponseType(typeof(Account), StatusCodes.Status200OK)]
            [ProducesResponseType(StatusCodes.Status404NotFound)]
            public async Task<IActionResult> GetById(Guid id)
            {
                var query = new GetAccountByIdQuery(id);
                var account = await _mediator.Send(query);
                return account != null ? Ok(account) : NotFound();
            }


            /// <summary>
            /// Получает выписку по счету
            /// </summary>
            /// <param name="id">Идентификатор счета</param>
            /// <param name="fromDate">Дата начала периода (необязательно)</param>
            /// <returns>Выписка по счету</returns>
            /// <response code="200">Успешный запрос</response>
            /// <response code="404">Счет не найден</response>
        [HttpGet("{id}/statement")]
            [ProducesResponseType(typeof(AccountStatement), StatusCodes.Status200OK)]
            [ProducesResponseType(StatusCodes.Status404NotFound)]
            public async Task<IActionResult> GetStatement(Guid id, [FromQuery] DateTime? fromDate)
            {
                var query = new GetAccountStatementQuery(id, fromDate);
                var statement = await _mediator.Send(query);
                return Ok(statement);
            }

        
            /// <summary>
            /// Получает список транзакций по указанному счёту.
            /// </summary>
            /// <param name="id">Уникальный идентификатор счёта (GUID).</param>
            /// <returns>Список транзакций в формате JSON.</returns>
            /// <response code="200">Успешный запрос. Возвращает список транзакций.</response>
            /// <response code="404">Счёт не найден.</response>
           [HttpGet("{id}/transactions")]
            [ProducesResponseType(typeof(IEnumerable<Transaction>), StatusCodes.Status200OK)]
            [ProducesResponseType(StatusCodes.Status404NotFound)]
            public async Task<IActionResult> GetTransactions(Guid id)
                {
                    var query = new GetTransactionsByAccountIdQuery(id);
                    var transactions = await _mediator.Send(query);
                    return Ok(transactions);
                }


            /// <summary>
            /// Создает новый банковский счёт.
            /// </summary>
            /// <param name="request">Данные для создания счёта (OwnerId, Type, Currency).</param>
            /// <returns>Созданный счёт.</returns>
            /// <response code="201">Счёт успешно создан.</response>
            /// <response code="400">Неверные параметры запроса.</response>
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
        
            /// <summary>
            /// Обновляет информацию о счете (полная замена)
            /// </summary>
            /// <param name="id">Идентификатор счета</param>
            /// <param name="request">Новые данные счета</param>
            /// <returns>Обновленный счет</returns>
            /// <response code="200">Счет успешно обновлен</response>
            /// <response code="400">Неверные параметры запроса</response>
            /// <response code="404">Счет не найден</response>
        [HttpPut("{id}")]
            [ProducesResponseType(typeof(Account), StatusCodes.Status200OK)]
            [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
            [ProducesResponseType(StatusCodes.Status404NotFound)]
            public async Task<IActionResult> Update(Guid id,
                [FromBody] UpdateAccountRequest request)
            {
                var command = new UpdateAccountCommand(id, request.InterestRate, request.CloseDate);
                var account = await _mediator.Send(command);
                return account != null ? Ok(account) : NotFound();
            }


            /// <summary>
            /// Частично обновляет информацию о счете
            /// </summary>
            /// <param name="id">Идентификатор счета</param>
            /// <param name="patchDoc">JSON Patch документ с изменениями</param>
            /// <returns>Обновленный счет</returns>
            /// <response code="200">Счет успешно обновлен</response>
            /// <response code="400">Неверный формат запроса</response>
            /// <response code="404">Счет не найден</response>
        [HttpPatch("{id}")]
            [ProducesResponseType(typeof(Account), StatusCodes.Status200OK)]
            [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
            [ProducesResponseType(StatusCodes.Status404NotFound)]
            public async Task<IActionResult> PatchAccount(
                Guid id,
                [FromBody] JsonPatchDocument<UpdateAccountRequest> patchDoc)
            {
                var command = new PatchAccountCommand(id, patchDoc);
                var account = await _mediator.Send(command);
                return account != null ? Ok(account) : NotFound();
            }



            /// <summary>
            /// Удаляет счет (мягкое удаление - устанавливает дату закрытия)
            /// </summary>
            /// <param name="id">Идентификатор счета</param>
            /// <returns>Статус операции</returns>
            /// <response code="204">Счет успешно "удален"</response>
            /// <response code="404">Счет не найден</response>
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
