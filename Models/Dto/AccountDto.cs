namespace AccountService.Models.Dto
{
    public class AccountDto
    {
        public Guid Id { get; set; }
        public string Type { get; set; }  // "Checking", "Deposit", "Credit"
        public string Currency { get; set; }
        public decimal Balance { get; set; }
    }
}
