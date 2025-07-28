namespace AccountService.Models.Dto
{
    public class UpdateAccountRequest
    {
        public decimal? InterestRate { get; set; }
        public DateTime? CloseDate { get; set; }
    }
}
