using BettyGamePlatform.Core.Application.Results;

namespace BettyGamePlatform.Core.Domain
{
    public sealed class Wallet
    {
        public decimal Balance { get; private set; } = 0m;

        public OperationResult Deposit(decimal amount)
        {
            if (amount <= 0m)
            {
                return OperationResult.Failure(OperationCode.InvalidAmount, Balance, amount, "Amount must be a positive number.");
            }

            Balance = RoundToCents(Balance + amount);
            return OperationResult.Success(OperationCode.DepositSuccess, Balance, amount);
        }

        public OperationResult Withdraw(decimal amount)
        {
            if (amount <= 0m)
            {
                return OperationResult.Failure(OperationCode.InvalidAmount, Balance, amount, "Amount must be a positive number.");
            }

            if (amount > Balance)
            {
                return OperationResult.Failure(OperationCode.InsufficientFunds, Balance, amount);
            }

            Balance = RoundToCents(Balance - amount);
            return OperationResult.Success(OperationCode.WithdrawSuccess, Balance, amount);
        }

        public OperationResult ApplyGameRound(decimal stakeAmount, decimal winAmount)
        {
            if (stakeAmount <= 0m)
            {
                return OperationResult.Failure(OperationCode.InvalidStake, Balance, stakeAmount, "Bet amount must be a positive number.");
            }

            if (stakeAmount > Balance)
            {
                return OperationResult.Failure(OperationCode.InsufficientFunds, Balance, stakeAmount);
            }

            if (winAmount < 0m)
            {
                return OperationResult.Failure(OperationCode.InvalidAmount, Balance, stakeAmount, "Win amount cannot be negative.");
            }

            stakeAmount = RoundToCents(stakeAmount);
            winAmount = RoundToCents(winAmount);

            Balance = RoundToCents(Balance - stakeAmount + winAmount);

            return OperationResult.Success(OperationCode.Success, Balance, stakeAmount, winAmount);
        }

        private static decimal RoundToCents(decimal value)
        {
            return Math.Round(value, 2, MidpointRounding.AwayFromZero);
        }
    }
}
