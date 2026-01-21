using BettyGamePlatform.Core.Domain.Games;
using FluentAssertions;

namespace BettyGamePlatform.Tests.Core
{
    public sealed class SlotGameRulesTests
    {
        private static SlotGameRules CreateValidRules()
        {
            return new SlotGameRules
            {
                MinStake = 1m,
                MaxStake = 10m,
                LoseProbability = 0.5,
                SmallWinProbability = 0.4,
                SmallWinMinMultiplier = 1m,
                SmallWinMaxMultiplier = 2m,
                BigWinMinMultiplier = 2m,
                BigWinMaxMultiplier = 10m
            };
        }

        [Fact]
        public void Validate_WithValidRules_DoesNotThrow()
        {
            var rules = CreateValidRules();

            rules.Invoking(r => r.Validate()).Should().NotThrow();
        }

        [Fact]
        public void Validate_WhenMinStakeIsZero_Throws()
        {
            var rules = CreateValidRules();
            rules.MinStake = 0m;

            rules.Invoking(r => r.Validate()).Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Validate_WhenMaxStakeIsNegative_Throws()
        {
            var rules = CreateValidRules();
            rules.MaxStake = -1m;

            rules.Invoking(r => r.Validate()).Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Validate_WhenMinStakeIsGreaterThanMaxStake_Throws()
        {
            var rules = CreateValidRules();
            rules.MinStake = 11m;

            rules.Invoking(r => r.Validate()).Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Validate_WhenLoseProbabilityIsNaN_Throws()
        {
            var rules = CreateValidRules();
            rules.LoseProbability = double.NaN;

            rules.Invoking(r => r.Validate()).Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Validate_WhenSmallWinProbabilityIsInfinity_Throws()
        {
            var rules = CreateValidRules();
            rules.SmallWinProbability = double.PositiveInfinity;

            rules.Invoking(r => r.Validate()).Should().Throw<ArgumentException>();
        }

        [Theory]
        [InlineData(-0.01)]
        [InlineData(1.01)]
        public void Validate_WhenProbabilityIsOutOfRange_Throws(double invalidProbability)
        {
            var rules = CreateValidRules();
            rules.LoseProbability = invalidProbability;

            rules.Invoking(r => r.Validate()).Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Validate_WhenProbabilitiesSumToMoreThanOne_Throws()
        {
            var rules = CreateValidRules();
            rules.LoseProbability = 0.8;
            rules.SmallWinProbability = 0.5;

            rules.Invoking(r => r.Validate()).Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Validate_WhenSmallWinMultiplierIsZero_Throws()
        {
            var rules = CreateValidRules();
            rules.SmallWinMinMultiplier = 0m;

            rules.Invoking(r => r.Validate()).Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Validate_WhenBigWinMultiplierIsNegative_Throws()
        {
            var rules = CreateValidRules();
            rules.BigWinMaxMultiplier = -1m;

            rules.Invoking(r => r.Validate()).Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Validate_WhenSmallWinMinMultiplierIsGreaterThanMax_Throws()
        {
            var rules = CreateValidRules();
            rules.SmallWinMinMultiplier = 3m;
            rules.SmallWinMaxMultiplier = 2m;

            rules.Invoking(r => r.Validate()).Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Validate_WhenBigWinMinMultiplierIsGreaterThanMax_Throws()
        {
            var rules = CreateValidRules();
            rules.BigWinMinMultiplier = 10m;
            rules.BigWinMaxMultiplier = 2m;

            rules.Invoking(r => r.Validate()).Should().Throw<ArgumentException>();
        }
    }
}
