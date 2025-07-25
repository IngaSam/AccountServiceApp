namespace AccountService.Exceptions
{
    public class AccountNotFoundException: Exception
    {
        public AccountNotFoundException(Guid accountId)
            : base($"Account with id {accountId} not found")
        {

        }
    }
}
