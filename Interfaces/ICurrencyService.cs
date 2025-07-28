namespace AccountService.Interfaces
{
    public interface ICurrencyService
    {
        bool IsCurrencySupported(string  currencyCode);
    }
}
