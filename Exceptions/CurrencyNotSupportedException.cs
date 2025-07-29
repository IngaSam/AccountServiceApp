namespace AccountService.Exceptions
{
    public class CurrencyNotSupportedException(string currency) : Exception($"Currency '{currency}' is not supported.");
}
