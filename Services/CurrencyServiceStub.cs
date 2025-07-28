using AccountService.Interfaces;
using AccountService.Models.Configs;

namespace AccountService.Services
{
    public class CurrencyServiceStub : ICurrencyService
    {
        public bool IsCurrencySupported(string currencyCode)
        {
            // Проверяем по списку поддерживаемых валют
            return CurrencyConfig.SupportedCurrencies.Contains(currencyCode);
        }
    }
}