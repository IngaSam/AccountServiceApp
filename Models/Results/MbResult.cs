using AccountService.Models.Errors;

namespace AccountService.Models.Results
{
    /// <summary>
    /// Универсальный результат операции
    /// </summary>
    /// <typeparam name="T">Тип возвращаемых данных</typeparam>
    public class MbResult<T>
    {
        public T? Data { get; init; }
        public MbError? Error { get; init; }
        public bool IsSuccess => Error == null;

        // Статические фабричные методы
        public static MbResult<T> Success(T data) => new() { Data = data };
        public static MbResult<T> Fail(MbError error) => new() { Error = error };
        public static MbResult<T> Fail(string code, string message) =>
            new() { Error = new MbError(code, message) };
    }
}