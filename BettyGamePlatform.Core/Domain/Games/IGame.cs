using BettyGamePlatform.Core.Application.Results;

namespace BettyGamePlatform.Core.Domain.Games
{
    public interface IGame
    {
        OperationResult PlayRound(decimal stakeAmount);
    }
}