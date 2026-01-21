using BettyGamePlatform.Core.Application.Abstractions;
using BettyGamePlatform.Core.Domain.Games;

namespace BettyGamePlatform.Core.Application.Services
{
    // Resolver keeps the app open for extension:
    // new games can be added by registering additional IGame implementations,
    // without changing the wallet/service or CLI code. We pick a default game via DI.
    public sealed class GameResolver : IGameResolver
    {
        private readonly IGame defaultGame;

        public GameResolver(IEnumerable<IGame> games)
        {
            defaultGame = games?.FirstOrDefault()
                ?? throw new InvalidOperationException("No games are registered.");
        }

        public IGame GetDefaultGame()
        {
            return defaultGame;
        }
    }
}