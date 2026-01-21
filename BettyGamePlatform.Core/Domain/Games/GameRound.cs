namespace BettyGamePlatform.Core.Domain.Games
{
    public sealed class GameRound
    {
        public decimal StakeAmount { get; }
        public decimal WinAmount { get; }

        public GameRound(decimal stakeAmount, decimal winAmount)
        {
            StakeAmount = stakeAmount;
            WinAmount = winAmount;
        }
    }
}
