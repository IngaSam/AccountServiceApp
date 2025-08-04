namespace AccountService.Models.Errors
{
    public class MbError
    {
        public string Code { get; }
        public string Message { get; }

        public MbError(string code, string message)
        {
            Code = code ?? throw new ArgumentNullException(nameof(code));
            Message = message ?? throw new ArgumentNullException(nameof(message));
        }

        public static MbError Create(string code, string message)
            => new(code, message);

        public override string ToString()
            => $"Error {Code}: {Message}";
    }

}
