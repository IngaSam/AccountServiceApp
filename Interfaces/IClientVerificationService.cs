namespace AccountService.Interfaces
{
    public interface IClientVerificationService
    {
        Task<bool> ClientExistsAsync(Guid clientId);
    }
}
