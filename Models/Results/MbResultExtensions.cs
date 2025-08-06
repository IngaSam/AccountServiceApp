using AccountService.Models.Errors;

namespace AccountService.Models.Results
{
    public static class MbResultExtensions
    {
        /// <summary>
        /// Преобразует результат в новый тип, сохраняя ошибку (если есть)
        /// </summary>
        public static MbResult<TNew> Map<TOrig, TNew>(
            this MbResult<TOrig> result,
            Func<TOrig, TNew> mapper)
        {
            return result.IsSuccess
                ? MbResult<TNew>.Success(mapper(result.Data!))
                : MbResult<TNew>.Fail(result.Error!);
        }

        /// <summary>
        /// Выполняет действие при успешном результате
        /// </summary>
        public static MbResult<T> OnSuccess<T>(
            this MbResult<T> result,
            Action<T> action)
        {
            if (result.IsSuccess)
            {
                action(result.Data!);
            }
            return result;
        }

        /// <summary>
        /// Обрабатывает ошибку (если есть)
        /// </summary>
        public static MbResult<T> OnError<T>(
            this MbResult<T> result,
            Action<MbError> action)
        {
            if (!result.IsSuccess)
            {
                action(result.Error!);
            }
            return result;
        }
    }
}