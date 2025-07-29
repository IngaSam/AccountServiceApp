namespace AccountService.Exceptions
{
    public class InsufficientFundsException : Exception 
    {
         public InsufficientFundsException(decimal amount) 
        : base($"Insufficient funds for transfer: {amount}") { }
    }
}
