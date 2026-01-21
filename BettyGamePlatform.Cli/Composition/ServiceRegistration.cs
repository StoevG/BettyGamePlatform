using BettyGamePlatform.Cli.Console;
using BettyGamePlatform.Cli.Parsing;
using BettyGamePlatform.Core.Application.Abstractions;
using BettyGamePlatform.Core.Application.Services;
using BettyGamePlatform.Core.Domain;
using BettyGamePlatform.Core.Domain.Abstractions;
using BettyGamePlatform.Core.Domain.Games;
using BettyGamePlatform.Core.Infrastructure.Rng;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BettyGamePlatform.Cli.Composition
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddBettyGamePlatform(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<SlotGameRules>(configuration.GetSection("SlotGame"));

            services.AddSingleton<IConsoleWrapper, ConsoleWrapper>();
            services.AddSingleton<CommandParser>();


            services.AddSingleton<IRandomProvider, RandomProvider>();

            services.AddSingleton<IGame, SlotGame>();
            services.AddSingleton<Wallet>();

            services.AddSingleton<IGameResolver, GameResolver>();
            services.AddSingleton<IPlayerWalletService, PlayerWalletService>();

            services.AddSingleton<GameApp>();

            return services;
        }
    }
}