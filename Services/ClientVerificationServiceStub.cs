using AccountService.Interfaces;

namespace AccountService.Services
{
    public class ClientVerificationServiceStub : IClientVerificationService
    {
        public Task<bool> ClientExistsAsync(Guid clientId)
        {
            // Заглушка: всегда возвращает true для существующих GUID
            return Task.FromResult(clientId != Guid.Empty);
        }
    }
}
