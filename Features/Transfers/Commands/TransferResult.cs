namespace AccountService.Features.Transfers.Commands
{
    public enum TransferResult
    {
        Success,
        AccountNotFound,
        InsufficientFunds,
        SameAccount,
        CurrencyMismatch
    }
}
