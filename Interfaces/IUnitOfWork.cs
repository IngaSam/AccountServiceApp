namespace AccountService.Interfaces
{
    public interface IUnitOfWork: IDisposable
    {
        IAccountRepository Accounts
        {
            get;
        }

        int Complete();
    }
}
