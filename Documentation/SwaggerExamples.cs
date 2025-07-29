using AccountService.Features.Accounts.Commands;
using AccountService.Models.Enums;
using Swashbuckle.AspNetCore.Filters;

namespace AccountService.Api.Documentation
{
    public class AccountExamples : IMultipleExamplesProvider<CreateAccountCommand>
    {
        public IEnumerable<SwaggerExample<CreateAccountCommand>> GetExamples()
        {
            yield return SwaggerExample.Create(
                "Текущий счет (Checking)",
                new CreateAccountCommand(
                    OwnerId: Guid.NewGuid(),
                    Type: AccountType.Checking,
                    Currency: "RUB",
                    InterestRate: null
                ));

            yield return SwaggerExample.Create(
                "Депозитный счет (Deposit)",
                new CreateAccountCommand(
                    OwnerId: Guid.NewGuid(),
                    Type: AccountType.Deposit,
                    Currency: "USD",
                    InterestRate: 3.5m
                ));
        }
    }
}