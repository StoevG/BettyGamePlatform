namespace BettyGamePlatform.Core.Domain.Games
{
    public sealed class SlotGameRules
    {
        public decimal MinStake { get; set; }
        public decimal MaxStake { get; set; }

        public double LoseProbability { get; set; }
        public double SmallWinProbability { get; set; }

        public decimal SmallWinMinMultiplier { get; set; }
        public decimal SmallWinMaxMultiplier { get; set; }

        public decimal BigWinMinMultiplier { get; set; }
        public decimal BigWinMaxMultiplier { get; set; }

        public void Validate()
        {
            RequirePositive(nameof(MinStake), MinStake);
            RequirePositive(nameof(MaxStake), MaxStake);
            RequireMinMax(nameof(MinStake), MinStake, nameof(MaxStake), MaxStake);

            RequireProbability(nameof(LoseProbability), LoseProbability);
            RequireProbability(nameof(SmallWinProbability), SmallWinProbability);

            if (LoseProbability + SmallWinProbability > 1.0)
            {
                throw new ArgumentException($"SlotGameRules probabilities must sum to 1 or less. Current: LoseProbability={LoseProbability}, SmallWinProbability={SmallWinProbability}");
            }

            RequirePositive(nameof(SmallWinMinMultiplier), SmallWinMinMultiplier);
            RequirePositive(nameof(SmallWinMaxMultiplier), SmallWinMaxMultiplier);
            RequireMinMax(nameof(SmallWinMinMultiplier), SmallWinMinMultiplier, nameof(SmallWinMaxMultiplier), SmallWinMaxMultiplier);

            RequirePositive(nameof(BigWinMinMultiplier), BigWinMinMultiplier);
            RequirePositive(nameof(BigWinMaxMultiplier), BigWinMaxMultiplier);
            RequireMinMax(nameof(BigWinMinMultiplier), BigWinMinMultiplier, nameof(BigWinMaxMultiplier), BigWinMaxMultiplier);
        }

        private static void RequirePositive(string name, decimal value)
        {
            if (value <= 0m)
            {
                throw new ArgumentException($"SlotGameRules.{name} must be greater than 0. Current: {value}");
            }
        }

        private static void RequireMinMax(string minName, decimal min, string maxName, decimal max)
        {
            if (min > max)
            {
                throw new ArgumentException($"SlotGameRules.{minName} must be <= SlotGameRules.{maxName}. Current: {minName}={min}, {maxName}={max}");
            }
        }

        private static void RequireProbability(string name, double value)
        {
            if (double.IsNaN(value) || double.IsInfinity(value))
            {
                throw new ArgumentException($"SlotGameRules.{name} must be a valid number. Current: {value}");
            }

            if (value < 0.0 || value > 1.0)
            {
                throw new ArgumentException($"SlotGameRules.{name} must be between 0 and 1. Current: {value}");
            }
        }
    }
}
