namespace AccountService.Interfaces
{
    public interface ICurrencyService
    {
        bool IsCurrencySupported(string currencyCode);
        string GetDefaultCurrency();
        List<string> GetSupportedCurrencies();
    }
}
