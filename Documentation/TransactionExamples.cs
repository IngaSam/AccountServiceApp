using AccountService.Features.Transactions.Commands;
using AccountService.Models.Enums;
using Swashbuckle.AspNetCore.Filters;

namespace AccountService.Documentation
{
    public class TransactionExamples : IMultipleExamplesProvider<CreateTransactionCommand>
    {
        public IEnumerable<SwaggerExample<CreateTransactionCommand>> GetExamples()
        {
            yield return SwaggerExample.Create(
                "Пополнение счета (Credit)",
                new CreateTransactionCommand(
                    AccountId: Guid.NewGuid(),
                    Amount: 1000.50m,
                    Type: TransactionType.Credit,
                    Description: "Пополнение через кассу"
                ));

            yield return SwaggerExample.Create(
                "Списание со счета (Debit)",
                new CreateTransactionCommand(
                    AccountId: Guid.NewGuid(),
                    Amount: 200.00m,
                    Type: TransactionType.Debit,
                    Description: "Оплата услуг"
                ));
        }
    }
}