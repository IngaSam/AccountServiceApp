using System.Text.Json.Serialization;

namespace AccountService.Models.Errors
{
    public class MbError
    {
        public string Code { get; }
        public string Message { get; }
        public Dictionary<string, string>? Details { get; }

        [JsonConstructor]
        public MbError(string code, string message, Dictionary<string, string>? details = null)
        {
            Code = code ?? throw new ArgumentNullException(nameof(code));
            Message = message ?? throw new ArgumentNullException(nameof(message));
            Details = details;

            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentException("Error code cannot be empty", nameof(code));
        }

        public static MbError Create(string code, string message, Dictionary<string, string>? details = null)
            => new(code, message, details);

        public override string ToString()
            => Details == null
                ? $"Error {Code}: {Message}"
                : $"Error {Code}: {Message}. Details: {string.Join(", ", Details)}";
    }

}
