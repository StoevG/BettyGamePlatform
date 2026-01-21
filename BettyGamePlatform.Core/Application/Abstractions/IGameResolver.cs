using BettyGamePlatform.Core.Domain.Games;

namespace BettyGamePlatform.Core.Application.Abstractions
{
    public interface IGameResolver
    {
        IGame GetDefaultGame();
    }
}