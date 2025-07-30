using AccountService.Interfaces;
using AccountService.Models.Configs;
using Microsoft.Extensions.Options;

namespace AccountService.Services
{
    public class CurrencyService : ICurrencyService
    {
        private readonly CurrencySettings _settings;

        public CurrencyService(IOptions<CurrencySettings> settings)
        {
            _settings = settings.Value;
        }

        public bool IsCurrencySupported(string currencyCode)
        {
            return _settings.SupportedCurrencies.Contains(currencyCode.ToUpper());
        }

        public string GetDefaultCurrency()
        {
            return _settings.DefaultCurrency;
        }

        public List<string> GetSupportedCurrencies()
        {
            return _settings.SupportedCurrencies;
        }
    }
}
