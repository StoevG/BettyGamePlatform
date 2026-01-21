namespace BettyGamePlatform.Core.Application.Results
{
    public enum OperationCode
    {
        Success,

        DepositSuccess,
        WithdrawSuccess,
        BetWin,
        BetLose,

        InvalidAmount,
        InvalidStake,
        InsufficientFunds
    }
}