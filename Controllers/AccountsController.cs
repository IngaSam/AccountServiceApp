using AccountService.Features.Accounts.Commands;
using AccountService.Features.Accounts.Queries;
using AccountService.Models;
using AccountService.Models.Dto;
using AccountService.Models.Enums;
using AccountService.Models.Results;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace AccountService.Controllers
{
    /// <summary>
    /// Контроллер для управления банковскими счетами
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Требуется аутентификация для всех методов
    [Produces("application/json")]
    public class AccountsController(IMediator mediator, ILogger<AccountsController> logger) : ControllerBase
    {
        /// <summary>
        /// Получает список всех счетов с возможностью фильтрации
        /// </summary>
        /// <param name="currency">Фильтр по валюте (например, "RUB")</param>
        /// <param name="type">Тип счета (Checking, Deposit, Credit)</param>
        /// <param name="page">Номер страницы (по умолчанию 1)</param>
        /// <param name="pageSize">Размер страницы (по умолчанию 10)</param>
        /// <returns>Результат операции со списком счетов</returns>
        /// <response code="200">Успешный запрос</response>
        /// <response code="401">Требуется аутентификация</response>
        [HttpGet]
        [ProducesResponseType(typeof(MbResult<IEnumerable<Account>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MbResult<object>), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetAll(
                    [FromQuery] string? currency,
                    [FromQuery] AccountType? type,
                    [FromQuery] int page = 1,
                    [FromQuery] int pageSize = 10)
        {
                try
                {
                    var query = new GetAllAccountsQuery(currency, type, page, pageSize);
                                    var result = await mediator.Send(query);
                                    return Ok(MbResult<IEnumerable<Account>>.Success(result));
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Ошибка при получении списка счетов");
                    return StatusCode(
                        StatusCodes.Status500InternalServerError,
                        MbResult<object>.Fail("INTERNAL_ERROR", "Внутренняя ошибка сервера"));
                }

        }

        /// <summary>
        /// Получает информацию о конкретном счете по ID
        /// </summary>
        /// <param name="id">Идентификатор счета</param>
        /// <returns>Информация о счете</returns>
        /// <response code="200">Счет найден</response>
        /// <response code="404">Счет не найден</response>
        /// <response code="401">Требуется аутентификация</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(MbResult<Account>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MbResult<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(MbResult<object>), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var query = new GetAccountByIdQuery(id);
                var account = await mediator.Send(query);

                return account != null
                    ? Ok(MbResult<Account>.Success(account))
                    : NotFound(MbResult<object>.Fail("ACCOUNT_NOT_FOUND", "Счет не найден"));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Ошибка при получении счета {AccountId}", id);
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    MbResult<object>.Fail("INTERNAL_ERROR", "Внутренняя ошибка сервера"));
            }
        }


        /// <summary>
        /// Получение выписки по счёту за указанный период
        /// </summary>
        /// <param name="accountId">Идентификатор счета (GUID)</param>
        /// <param name="fromDate">Начальная дата периода (опционально, по умолчанию 30 дней назад)</param>
        /// <param name="toDate">Конечная дата периода (опционально, по умолчанию текущая дата)</param>
        /// <returns>Результат операции с выпиской по счету</returns>
        /// <response code="200">Возвращает выписку по счёту</response>
        /// <response code="400">Некорректные параметры запроса</response>
        /// <response code="404">Счёт не найден</response>
        /// <response code="401">Требуется аутентификация</response>
        [HttpGet("{accountId}/statement")]
        [ProducesResponseType(typeof(MbResult<AccountStatement>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MbResult<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(MbResult<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(MbResult<object>), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetStatement(
            [FromRoute] Guid accountId,
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate)
        {
            try
            {
                if (fromDate > toDate)
                {
                    return BadRequest(MbResult<object>.Fail(
                        "INVALID_DATE_RANGE",
                        "Дата начала периода должна быть раньше даты окончания"));
                }

                var query = new GetAccountStatementQuery(accountId, fromDate, toDate);
                var statement = await mediator.Send(query);

                return statement != null
                    ? Ok(MbResult<AccountStatement>.Success(statement))
                    : NotFound(MbResult<object>.Fail(
                        "ACCOUNT_NOT_FOUND",
                        $"Счет с ID {accountId} не найден"));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Ошибка при получении выписки по счету {AccountId}", accountId);
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    MbResult<object>.Fail("INTERNAL_ERROR", "Внутренняя ошибка сервера"));
            }
        }


        /// <summary>
        /// Получает список транзакций по указанному счёту.
        /// </summary>
        /// <param name="id">Уникальный идентификатор счёта (GUID).</param>
        /// <returns>Результат операции со списком транзакций.</returns>
        /// <response code="200">Успешный запрос. Возвращает список транзакций.</response>
        /// <response code="404">Счёт не найден.</response>
        /// <response code="401">Требуется аутентификация</response>
        [HttpGet("{id}/transactions")]
        [ProducesResponseType(typeof(MbResult<IEnumerable<Transaction>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MbResult<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(MbResult<object>), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetTransactions(Guid id)
        {
            try
            {
                var query = new GetTransactionsByAccountIdQuery(id);
                var transactions = await mediator.Send(query);

                return transactions != null && transactions.Any()
                    ? Ok(MbResult<IEnumerable<Transaction>>.Success(transactions))
                    : NotFound(MbResult<object>.Fail(
                        "NO_TRANSACTIONS",
                        $"Для счета {id} не найдено транзакций"));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Ошибка при получении транзакций по счету {AccountId}", id);
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    MbResult<object>.Fail("INTERNAL_ERROR", "Внутренняя ошибка сервера"));
            }
        }


        /// <summary>
        /// Создает новый банковский счёт.
        /// </summary>
        /// <param name="request">Данные для создания счёта (OwnerId, Type, Currency).</param>
        /// <returns>Созданный счёт.</returns>
        /// <response code="201">Счёт успешно создан.</response>
        /// <response code="400">Неверные параметры запроса.</response>
        /// <response code="401">Требуется аутентификация</response>
        [HttpPost]
        [ProducesResponseType(typeof(MbResult<Account>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(MbResult<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(MbResult<object>), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Create([FromBody] CreateAccountRequest request)
        {
            try
            {
                logger.LogInformation("Создание счета для владельца {OwnerId}", request.OwnerId);

                var command = new CreateAccountCommand(
                    request.OwnerId,
                    request.Type,
                    request.Currency,
                    request.InterestRate);

                var account = await mediator.Send(command);

                logger.LogInformation("Счет {AccountId} успешно создан", account.Id);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = account.Id },
                    MbResult<Account>.Success(account));
            }
            catch (ValidationException ex)
            {
                logger.LogWarning("Ошибка валидации: {ErrorMessage}", ex.Message);
                var firstError = ex.Message;
                return BadRequest(MbResult<object>.Fail(
                    "VALIDATION_ERROR",
                    firstError));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Ошибка при создании счета");
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    MbResult<object>.Fail("INTERNAL_ERROR", "Внутренняя ошибка сервера"));
            }
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
        /// <response code="401">Требуется аутентификация</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(MbResult<Account>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MbResult<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(MbResult<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(MbResult<object>), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Update(
            Guid id,
            [FromBody] UpdateAccountRequest request)
        {
            try
            {
                if (request.InterestRate < 0)
                {
                    return BadRequest(MbResult<object>.Fail(
                        "INVALID_INTEREST_RATE",
                        "Процентная ставка не может быть отрицательной"));
                }

                var command = new UpdateAccountCommand(id, request.InterestRate, request.CloseDate);
                var account = await mediator.Send(command);

                return account != null
                    ? Ok(MbResult<Account>.Success(account))
                    : NotFound(MbResult<object>.Fail(
                        "ACCOUNT_NOT_FOUND",
                        $"Счет с ID {id} не найден"));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Ошибка при обновлении счета {AccountId}", id);
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    MbResult<object>.Fail("INTERNAL_ERROR", "Внутренняя ошибка сервера"));
            }
        }


        /// <summary>
        /// Частично обновляет информацию о счете
        /// </summary>
        /// <param name="id">Идентификатор счета</param>
        /// <param name="patchDoc">JSON Patch документ с изменениями</param>
        /// <returns>Обновленный счет</returns>
        /// <response code="200">Счет успешно обновлен</response>
        /// <response code="400">Неверный формат запроса</response>
        /// <response code="401">Требуется аутентификация</response>
        /// <response code="404">Счет не найден</response>
        [HttpPatch("{id}")]
        [ProducesResponseType(typeof(MbResult<Account>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MbResult<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(MbResult<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(MbResult<object>), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> PatchAccount(
            Guid id,
            [FromBody] JsonPatchDocument<UpdateAccountRequest> patchDoc)
        {
            try
            {
                if (patchDoc == null)
                {
                    return BadRequest(MbResult<object>.Fail(
                        "INVALID_PATCH_DOC",
                        "Необходимо предоставить документ для обновления"));
                }

                var command = new PatchAccountCommand(id, patchDoc);
                var account = await mediator.Send(command);

                return account != null
                    ? Ok(MbResult<Account>.Success(account))
                    : NotFound(MbResult<object>.Fail(
                        "ACCOUNT_NOT_FOUND",
                        $"Счет с ID {id} не найден"));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Ошибка при частичном обновлении счета {AccountId}", id);
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    MbResult<object>.Fail("INTERNAL_ERROR", "Внутренняя ошибка сервера"));
            }
        }



        /// <summary>
        /// Удаляет счет (мягкое удаление - устанавливает дату закрытия)
        /// </summary>
        /// <param name="id">Идентификатор счета</param>
        /// <returns>Статус операции</returns>
        /// <response code="204">Счет успешно "удален"</response>
        /// <response code="404">Счет не найден</response>
        /// <response code="401">Требуется аутентификация</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(MbResult<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(MbResult<object>), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var command = new DeleteAccountCommand(id);
                var result = await mediator.Send(command);

                return result
                    ? NoContent()
                    : NotFound(MbResult<object>.Fail("ACCOUNT_NOT_FOUND", "Счет не найден"));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Ошибка при удалении счета {AccountId}", id);
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    MbResult<object>.Fail("INTERNAL_ERROR", "Внутренняя ошибка сервера"));
            }
        }

        /// <summary>
        /// Проверяет существование счёта по ID
        /// </summary>
        /// <param name="id">Идентификатор счета</param>
        /// <returns>Результат проверки существования счета</returns>
        /// <response code="200">Счет существует</response>
        /// <response code="404">Счет не найден</response>
        /// <response code="401">Требуется аутентификация</response>
        [HttpGet("{id}/exists")]
        [ProducesResponseType(typeof(MbResult<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MbResult<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(MbResult<object>), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Exists(Guid id)
        {
            try
            {
                var exists = await mediator.Send(new CheckAccountExistsQuery(id));
                return exists
                    ? Ok(MbResult<bool>.Success(true))
                    : NotFound(MbResult<object>.Fail(
                        "ACCOUNT_NOT_FOUND",
                        $"Счет с ID {id} не найден"));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Ошибка при проверке существования счета {AccountId}", id);
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    MbResult<object>.Fail("INTERNAL_ERROR", "Внутренняя ошибка сервера"));
            }
        }
    }

}
