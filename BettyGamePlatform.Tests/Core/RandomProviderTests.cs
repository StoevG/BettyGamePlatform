using BettyGamePlatform.Core.Infrastructure.Rng;
using FluentAssertions;

namespace BettyGamePlatform.Tests.Core
{
    public sealed class RandomProviderTests
    {
        [Fact]
        public void NextInRange_WhenMinIsGreaterThanMax_Throws()
        {
            var rng = new RandomProvider();

            rng.Invoking(r => r.NextInRange(2m, 1m))
                .Should()
                .Throw<ArgumentException>();
        }

        [Fact]
        public void NextInRange_WhenMinEqualsMax_ReturnsThatValue()
        {
            var rng = new RandomProvider();

            var value = rng.NextInRange(1.23m, 1.23m);

            value.Should().Be(1.23m);
        }

        [Fact]
        public void NextInRange_AlwaysReturnsValueWithinRange()
        {
            var rng = new RandomProvider();

            for (var i = 0; i < 5_000; i++)
            {
                var value = rng.NextInRange(1m, 2m);
                value.Should().BeGreaterThanOrEqualTo(1m);
                value.Should().BeLessThanOrEqualTo(2m);
            }
        }

        [Fact]
        public void NextInRange_ReturnsOnlyTwoDecimalSteps()
        {
            var rng = new RandomProvider();

            for (var i = 0; i < 2_000; i++)
            {
                var value = rng.NextInRange(1m, 2m);

                // value must have at most 2 decimals
                var cents = value * 100m;
                decimal.Round(cents, 0).Should().Be(cents);
            }
        }

        [Fact]
        public void NextInRange_CanReturnMaxValue_InclusiveUpperBound()
        {
            var rng = new RandomProvider();

            // Small range with many discrete points; we try enough times to very likely hit max.
            // If this fails extremely rarely, bump attempts.
            const decimal min = 1.00m;
            const decimal max = 1.10m;

            var hitMax = false;

            for (var i = 0; i < 50_000; i++)
            {
                if (rng.NextInRange(min, max) == max)
                {
                    hitMax = true;
                    break;
                }
            }

            hitMax.Should().BeTrue("the generator should be inclusive and able to return the upper bound");
        }
    }
}
