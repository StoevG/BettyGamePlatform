namespace BettyGamePlatform.Cli.Console
{
    public sealed class ConsoleWrapper : IConsoleWrapper
    {
        public void WriteLine(string message) => System.Console.WriteLine(message);
        public string? ReadLine() => System.Console.ReadLine();
        public void ReadKey() => System.Console.ReadKey();
    }
}
