using System.ComponentModel;
using System.Text.Json.Serialization;

namespace AccountService.Models.Enums
{

    [JsonConverter(typeof(JsonStringEnumConverter))] 
    public enum AccountType
    {
        Unknown = 0,

        [Description("Текущий счёт")]
        Checking  = 1, 

        [Description("Депозитный вклад")]
        Deposit = 2, 

        [Description("Кредитный счёт")]
        Credit = 3     
    }
}
