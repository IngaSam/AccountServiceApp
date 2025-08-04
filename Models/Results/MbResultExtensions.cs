using AccountService.Models.Errors;

namespace AccountService.Models.Results
{
    public static class MbResultExtensions
    {
        /// <summary>
        /// Создает неуспешный результат с ошибкой
        /// </summary>
        public static MbResult<T> Fail<T>(this MbResult<T> _, MbError error)
        {
            return new MbResult<T>
            {
                Error = error
            };
        }

        /// <summary>
        /// Создает неуспешный результат с кодом и сообщением
        /// </summary>
        public static MbResult<T> Fail<T>(this MbResult<T> _, string code, string message)
        {
            return new MbResult<T>
            {
                Error = new MbError(code, message)
            };
        }

        /// <summary>
        /// Создает успешный результат с данными
        /// </summary>
        public static MbResult<T> Success<T>(this MbResult<T> _, T data)
        {
            return new MbResult<T>
            {
                Data = data
            };
        }
    }
}