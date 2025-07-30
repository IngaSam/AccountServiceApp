namespace AccountService.Models.Configs
{
    public class CurrencySettings
    {
        public List<string> SupportedCurrencies { get; set; } = new();
        public string DefaultCurrency { get; set; } = "RUB";
        public decimal MaxInterestRate { get; set; } = 100;
    }
}
