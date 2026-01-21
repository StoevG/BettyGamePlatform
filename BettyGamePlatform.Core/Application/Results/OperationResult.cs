namespace BettyGamePlatform.Core.Application.Results
{
    public sealed class OperationResult
    {
        public bool IsSuccess { get; }
        public OperationCode Code { get; }
        public decimal Balance { get; }
        public decimal? Amount { get; }
        public decimal? WinAmount { get; }
        public string? Error { get; }

        private OperationResult(bool isSuccess, OperationCode code, decimal balance, decimal? amount, decimal? winAmount, string? error)
        {
            IsSuccess = isSuccess;
            Code = code;
            Balance = balance;
            Amount = amount;
            WinAmount = winAmount;
            Error = error;
        }

        public static OperationResult Success(OperationCode code, decimal balance, decimal? amount = null, decimal? winAmount = null)
            => new(true, code, balance, amount, winAmount, null);

        public static OperationResult Failure(OperationCode code, decimal balance, decimal? amount = null, string? error = null)
            => new(false, code, balance, amount, null, error);
    }
}