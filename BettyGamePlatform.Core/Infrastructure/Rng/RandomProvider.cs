using BettyGamePlatform.Core.Domain.Abstractions;

namespace BettyGamePlatform.Core.Infrastructure.Rng
{
    public sealed class RandomProvider : IRandomProvider
    {
        private const decimal MaxCentsConvertible = long.MaxValue / 100m;
        private const decimal MinCentsConvertible = long.MinValue / 100m;

        public double NextProbability()
        {
            return Random.Shared.NextDouble();
        }

        public decimal NextInRange(decimal minInclusive, decimal maxInclusive)
        {
            if (minInclusive > maxInclusive)
            {
                throw new ArgumentException($"minInclusive must be <= maxInclusive. minInclusive={minInclusive}, maxInclusive={maxInclusive}");
            }

            if (minInclusive == maxInclusive)
            {
                return minInclusive;
            }

            var min = ToCents(minInclusive);
            var max = ToCents(maxInclusive);

            if (min == max)
            {
                return min / 100m;
            }

            if (max == long.MaxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(maxInclusive), $"Range is too large. maxInclusive={maxInclusive}");
            }

            var value = Random.Shared.NextInt64(min, max + 1);
            return value / 100m;
        }

        private static long ToCents(decimal value)
        {
            if (value > MaxCentsConvertible || value < MinCentsConvertible)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(value),
                    $"Value is too large to be converted to cents. value={value}");
            }

            var scaled = Math.Round(value * 100m, 0, MidpointRounding.AwayFromZero);
            return (long)scaled;
        }
    }
}
