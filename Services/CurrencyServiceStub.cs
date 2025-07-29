using AccountService.Interfaces;
using AccountService.Models.Configs;

namespace AccountService.Services
{
    public class CurrencyServiceStub : ICurrencyService
    {
        public bool IsCurrencySupported(string currencyCode)
        {
            if (string.IsNullOrWhiteSpace(currencyCode))
             return false;

            return CurrencyConfig.SupportedCurrencies.Contains(currencyCode);
        }
    }
}