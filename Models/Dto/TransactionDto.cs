namespace AccountService.Models.Dto
{
    public class TransactionDto
    {
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string Type { get; set; } // "Credit", "Debit"
        public string Description { get; set; }

    }
}
