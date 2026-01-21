using BettyGamePlatform.Core.Application.Results;

namespace BettyGamePlatform.Core.Application.Abstractions
{
    public interface IPlayerWalletService
    {
        OperationResult Deposit(decimal amount);
        OperationResult Withdraw(decimal amount);
        OperationResult Bet(decimal stakeAmount);
    }
}
