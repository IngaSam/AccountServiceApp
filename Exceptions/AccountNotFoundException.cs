namespace AccountService.Exceptions
{
    public class AccountNotFoundException(Guid accountId) : Exception($"Account with id {accountId} not found");
}
