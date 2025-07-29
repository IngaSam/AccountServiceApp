namespace AccountService.Exceptions
{
    public class CurrencyNotSupportedException : Exception
    {
        public CurrencyNotSupportedException(string currency)
            : base($"Currency '{currency}' is not supported.") { }
    }
}
