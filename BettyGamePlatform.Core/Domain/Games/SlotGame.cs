using BettyGamePlatform.Core.Application.Results;
using BettyGamePlatform.Core.Domain.Abstractions;
using Microsoft.Extensions.Options;

namespace BettyGamePlatform.Core.Domain.Games
{
    public sealed class SlotGame : IGame
    {
        private readonly IRandomProvider randomProvider;
        private readonly SlotGameRules rules;

        public SlotGame(IRandomProvider randomProvider, IOptions<SlotGameRules> rulesOptions)
        {
            this.randomProvider = randomProvider;
            rules = rulesOptions.Value;
            rules.Validate();
        }

        public OperationResult PlayRound(decimal stakeAmount)
        {
            if (stakeAmount < rules.MinStake || stakeAmount > rules.MaxStake)
            {
                return OperationResult.Failure(
                    OperationCode.InvalidStake,
                    balance: 0m,
                    amount: stakeAmount,
                    error: $"Bet amount must be between {rules.MinStake:0.##} and {rules.MaxStake:0.##}.");
            }

            var chance = randomProvider.NextProbability();
            var loseThreshold = rules.LoseProbability;
            var smallWinThreshold = rules.LoseProbability + rules.SmallWinProbability;

            decimal winAmount;

            if (chance < loseThreshold)
            {
                winAmount = 0m;
            }
            else if (chance < smallWinThreshold)
            {
                var multiplier = randomProvider.NextInRange(rules.SmallWinMinMultiplier, rules.SmallWinMaxMultiplier);
                winAmount = stakeAmount * multiplier;
            }
            else
            {
                var multiplier = randomProvider.NextInRange(rules.BigWinMinMultiplier, rules.BigWinMaxMultiplier);
                winAmount = stakeAmount * multiplier;
            }

            var code = winAmount > 0m ? OperationCode.BetWin : OperationCode.BetLose;

            return OperationResult.Success(code, balance: 0m, amount: stakeAmount, winAmount: winAmount);
        }
    }
}
