using AccountService.Models.Enums;

namespace AccountService.Models.Dto
{
    public class CreateTransactionRequest
    {
        public Guid AccountId { get; set; }
        public decimal Amount { get; set; }
        public TransactionType Type { get; set; }
        public string Description { get; set; }
    }
}
