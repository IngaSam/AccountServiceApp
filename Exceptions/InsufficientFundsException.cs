namespace AccountService.Exceptions
{
    public class InsufficientFundsException(decimal amount) : Exception($"Insufficient funds for transfer: {amount}");
}
