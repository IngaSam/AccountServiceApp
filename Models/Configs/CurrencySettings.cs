namespace AccountService.Models.Configs
{
    public class CurrencySettings
    {
        public List<string> SupportedCurrencies { get; set; } = new()
        {
            "RUB", "USD", "EUR", "GBP", "CNY"
        };

        public string DefaultCurrency { get; set; } = "RUB";
        public decimal MaxInterestRate { get; set; } = 100;
        public int Precision { get; set; } = 2; // Добавьте, если используется
    }
}
