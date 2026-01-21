using BettyGamePlatform.Cli.Composition;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BettyGamePlatform.Cli;

public static class Program
{
    public static void Main(string[] args)
    {
        var configuration = BuildConfiguration();

        var services = new ServiceCollection()
            .AddSingleton<IConfiguration>(configuration)
            .AddBettyGamePlatform(configuration);

        using var provider = services.BuildServiceProvider();
        provider.GetRequiredService<GameApp>().Play();
    }

    private static IConfiguration BuildConfiguration()
    {
        return new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .Build();
    }
}