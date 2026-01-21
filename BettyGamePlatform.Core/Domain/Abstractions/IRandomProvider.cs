namespace BettyGamePlatform.Core.Domain.Abstractions
{
    public interface IRandomProvider
    {
        double NextProbability();

        decimal NextInRange(decimal minInclusive, decimal maxInclusive);
    }
}
