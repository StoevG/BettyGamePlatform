using BettyGamePlatform.Core.Application.Abstractions;
using BettyGamePlatform.Core.Application.Results;
using BettyGamePlatform.Core.Domain;

namespace BettyGamePlatform.Core.Application.Services
{
    public sealed class PlayerWalletService : IPlayerWalletService
    {
        private readonly Wallet wallet;
        private readonly IGameResolver gameResolver;

        public PlayerWalletService(Wallet wallet, IGameResolver gameResolver)
        {
            this.wallet = wallet;
            this.gameResolver = gameResolver;
        }

        public OperationResult Deposit(decimal amount)
        {
            return wallet.Deposit(amount);
        }

        public OperationResult Withdraw(decimal amount)
        {
            return wallet.Withdraw(amount);
        }

        public OperationResult Bet(decimal stakeAmount)
        {
            if (wallet.Balance < stakeAmount)
            {
                return OperationResult.Failure(OperationCode.InsufficientFunds, wallet.Balance, stakeAmount);
            }

            var game = gameResolver.GetDefaultGame();
            var round = game.PlayRound(stakeAmount);

            if (!round.IsSuccess)
            {
                return OperationResult.Failure(OperationCode.InvalidStake, wallet.Balance, stakeAmount, round.Error);
            }

            var apply = wallet.ApplyGameRound(stakeAmount, round.WinAmount ?? 0m);
            if (!apply.IsSuccess)
            {
                return OperationResult.Failure(apply.Code, apply.Balance, stakeAmount, apply.Error);
            }

            return OperationResult.Success(round.Code, wallet.Balance, stakeAmount, round.WinAmount ?? 0m);
        }
    }
}
