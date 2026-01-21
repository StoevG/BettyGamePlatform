using BettyGamePlatform.Cli.Console;
using BettyGamePlatform.Cli.Parsing;
using BettyGamePlatform.Core.Application.Abstractions;
using BettyGamePlatform.Core.Application.Results;
using System.Globalization;

namespace BettyGamePlatform.Cli
{
    public sealed class GameApp
    {
        private readonly IConsoleWrapper console;
        private readonly IPlayerWalletService playerWalletService;
        private readonly CommandParser commandParser;

        public GameApp(IConsoleWrapper console, IPlayerWalletService playerWalletService, CommandParser commandParser)
        {
            this.console = console;
            this.playerWalletService = playerWalletService;
            this.commandParser = commandParser;
        }

        public void Play()
        {
            while (true)
            {
                this.console.WriteLine(ConsoleMessages.PromptMessage);
                var input = this.console.ReadLine();

                if (!this.commandParser.TryParse(input, out var command))
                {
                    this.console.WriteLine(ConsoleMessages.InvalidInputMessage);
                    this.console.WriteLine(string.Empty);
                    continue;
                }

                if (command!.Type == CommandType.Exit)
                {
                    this.console.WriteLine(ConsoleMessages.ExitMessage);
                    this.console.WriteLine(string.Empty);
                    this.console.WriteLine(ConsoleMessages.PressAnyKeyToExitMessage);
                    this.console.ReadKey();
                    return;
                }

                OperationResult result = command.Type switch
                {
                    CommandType.Deposit => playerWalletService.Deposit(command.Amount),
                    CommandType.Withdraw => playerWalletService.Withdraw(command.Amount),
                    CommandType.Bet => playerWalletService.Bet(command.Amount),
                    _ => OperationResult.Failure(OperationCode.InvalidAmount, 0m, error: ConsoleMessages.InvalidInputMessage)
                };

                var message = result.Code switch
                {
                    OperationCode.DepositSuccess => string.Format(CultureInfo.InvariantCulture, ConsoleMessages.DepositSuccessMessage, result.Amount ?? 0m, result.Balance),
                    OperationCode.WithdrawSuccess => string.Format(CultureInfo.InvariantCulture, ConsoleMessages.WithdrawSuccessMessage, result.Amount ?? 0m, result.Balance),
                    OperationCode.BetWin => string.Format(CultureInfo.InvariantCulture, ConsoleMessages.BetWinMessage, result.WinAmount ?? 0m, result.Balance),
                    OperationCode.BetLose => string.Format(CultureInfo.InvariantCulture, ConsoleMessages.BetLoseMessage, result.Balance),
                    OperationCode.InsufficientFunds => string.Format(CultureInfo.InvariantCulture, ConsoleMessages.InsufficientFundsMessage, result.Balance),
                    OperationCode.InvalidStake => result.Error ?? ConsoleMessages.InvalidStakeMessage,
                    OperationCode.InvalidAmount => ConsoleMessages.InvalidInputMessage,
                    _ => result.Error ?? ConsoleMessages.InvalidInputMessage
                };

                this.console.WriteLine(message);
                this.console.WriteLine(string.Empty);
            }
        }
    }
}
