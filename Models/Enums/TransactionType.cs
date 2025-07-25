using System.Text.Json.Serialization;

namespace AccountService.Models.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))] //Для реализации в строку
    public enum TransactionType
    {
        Credit,  // Зачисление (увеличение баланса)
        Debit    // Списание (уменьшение баланса)
    }
}
