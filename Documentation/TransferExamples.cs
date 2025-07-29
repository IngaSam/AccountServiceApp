using AccountService.Features.Transfers.Commands;
using Swashbuckle.AspNetCore.Filters;

namespace AccountService.Documentation
{
    public class TransferExamples : IMultipleExamplesProvider<TransferCommand>
    {
        public IEnumerable<SwaggerExample<TransferCommand>> GetExamples()
        {
            yield return SwaggerExample.Create(
                "Стандартный перевод",
                new TransferCommand(
                    FromAccountId: Guid.NewGuid(),
                    ToAccountId: Guid.NewGuid(),
                    Amount: 500.00m
                ));

            yield return SwaggerExample.Create(
                "Международный перевод",
                new TransferCommand(
                    FromAccountId: Guid.NewGuid(),
                    ToAccountId: Guid.NewGuid(),
                    Amount: 100.00m
                ));
        }
    }
}