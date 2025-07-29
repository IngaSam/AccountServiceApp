using System.ComponentModel;
using System.Text.Json.Serialization;

namespace AccountService.Models.Enums
{

    [JsonConverter(typeof(JsonStringEnumConverter))] 
    public enum AccountType
    {
        [Description("Текущий счёт")]
        Checking, 

        [Description("Депозитный вклад")]
        Deposit, 

        [Description("Кредитный счёт")]
        Credit     
    }
}
