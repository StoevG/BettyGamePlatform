namespace BettyGamePlatform.Cli.Console
{
    public interface IConsoleWrapper
    {
        void WriteLine(string message);
        string? ReadLine();
        void ReadKey();
    }
}
