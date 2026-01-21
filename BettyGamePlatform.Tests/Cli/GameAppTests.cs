using BettyGamePlatform.Cli;
using BettyGamePlatform.Cli.Console;
using BettyGamePlatform.Cli.Parsing;
using BettyGamePlatform.Core.Application.Abstractions;
using BettyGamePlatform.Core.Application.Results;
using FluentAssertions;
using Moq;
using System.Globalization;

namespace BettyGamePlatform.Tests.Cli
{
    public sealed class GameAppTests
    {
        [Fact]
        public void InvalidInput_PrintsHelpMessage_AndDoesNotCallService()
        {
            var console = new FakeConsole("deposit 20,3242", "exit");
            var service = new Mock<IPlayerWalletService>(MockBehavior.Strict);

            var app = new GameApp(console, service.Object, new CommandParser());
            app.Play();

            console.Output.Should().Contain(ConsoleMessages.InvalidInputMessage);
            console.Output.Should().Contain(ConsoleMessages.ExitMessage);

            service.VerifyNoOtherCalls();
            console.ReadKeyCalled.Should().BeTrue();
        }

        [Fact]
        public void Deposit_PrintsSuccessMessage()
        {
            var console = new FakeConsole("deposit 10", "exit");
            var service = new Mock<IPlayerWalletService>(MockBehavior.Strict);

            service.Setup(s => s.Deposit(10m))
                .Returns(OperationResult.Success(OperationCode.DepositSuccess, balance: 10m, amount: 10m));

            var app = new GameApp(console, service.Object, new CommandParser());
            app.Play();

            var expected = string.Format(CultureInfo.InvariantCulture, ConsoleMessages.DepositSuccessMessage, 10m, 10m);
            console.Output.Should().Contain(expected);

            service.Verify(s => s.Deposit(10m), Times.Once);
            service.VerifyNoOtherCalls();
        }

        [Fact]
        public void Withdraw_InsufficientFunds_PrintsInsufficientFundsMessage()
        {
            var console = new FakeConsole("withdraw 5", "exit");
            var service = new Mock<IPlayerWalletService>(MockBehavior.Strict);

            service.Setup(s => s.Withdraw(5m))
                .Returns(OperationResult.Failure(OperationCode.InsufficientFunds, balance: 0m, amount: 5m));

            var app = new GameApp(console, service.Object, new CommandParser());
            app.Play();

            var expected = string.Format(CultureInfo.InvariantCulture, ConsoleMessages.InsufficientFundsMessage, 0m);
            console.Output.Should().Contain(expected);

            service.Verify(s => s.Withdraw(5m), Times.Once);
            service.VerifyNoOtherCalls();
        }

        [Fact]
        public void BetWin_PrintsWinMessage()
        {
            var console = new FakeConsole("bet 5", "exit");
            var service = new Mock<IPlayerWalletService>(MockBehavior.Strict);

            service.Setup(s => s.Bet(5m))
                .Returns(OperationResult.Success(OperationCode.BetWin, balance: 17m, amount: 5m, winAmount: 12m));

            var app = new GameApp(console, service.Object, new CommandParser());
            app.Play();

            var expected = string.Format(CultureInfo.InvariantCulture, ConsoleMessages.BetWinMessage, 12m, 17m);
            console.Output.Should().Contain(expected);

            service.Verify(s => s.Bet(5m), Times.Once);
            service.VerifyNoOtherCalls();
        }

        [Fact]
        public void BetLose_PrintsLoseMessage()
        {
            var console = new FakeConsole("bet 5", "exit");
            var service = new Mock<IPlayerWalletService>(MockBehavior.Strict);

            service.Setup(s => s.Bet(5m))
                .Returns(OperationResult.Success(OperationCode.BetLose, balance: 5m, amount: 5m, winAmount: 0m));

            var app = new GameApp(console, service.Object, new CommandParser());
            app.Play();

            var expected = string.Format(CultureInfo.InvariantCulture, ConsoleMessages.BetLoseMessage, 5m);
            console.Output.Should().Contain(expected);

            service.Verify(s => s.Bet(5m), Times.Once);
            service.VerifyNoOtherCalls();
        }

        [Fact]
        public void InvalidStake_PrintsSpecificStakeError_WhenProvided()
        {
            var console = new FakeConsole("bet 11", "exit");
            var service = new Mock<IPlayerWalletService>(MockBehavior.Strict);

            service.Setup(s => s.Bet(11m))
                .Returns(OperationResult.Failure(OperationCode.InvalidStake, balance: 100m, amount: 11m, error: "Bet amount must be between 1 and 10."));

            var app = new GameApp(console, service.Object, new CommandParser());
            app.Play();

            console.Output.Should().Contain("Bet amount must be between 1 and 10.");

            service.Verify(s => s.Bet(11m), Times.Once);
            service.VerifyNoOtherCalls();
        }

        [Fact]
        public void Exit_PrintsExitMessage_AndWaitsForKey()
        {
            var console = new FakeConsole("exit");
            var service = new Mock<IPlayerWalletService>(MockBehavior.Strict);

            var app = new GameApp(console, service.Object, new CommandParser());
            app.Play();

            console.Output.Should().Contain(ConsoleMessages.ExitMessage);
            console.Output.Should().Contain(ConsoleMessages.PressAnyKeyToExitMessage);
            console.ReadKeyCalled.Should().BeTrue();
            service.VerifyNoOtherCalls();
        }

        private sealed class FakeConsole : IConsoleWrapper
        {
            private readonly Queue<string?> inputQueue = new();

            public List<string> Output { get; } = new();
            public bool ReadKeyCalled { get; private set; }

            public FakeConsole(params string?[] inputs)
            {
                foreach (var input in inputs)
                {
                    inputQueue.Enqueue(input);
                }
            }

            public string? ReadLine()
            {
                return inputQueue.Count > 0 ? inputQueue.Dequeue() : null;
            }

            public void WriteLine(string message)
            {
                Output.Add(message);
            }

            public void ReadKey()
            {
                ReadKeyCalled = true;
            }
        }
    }
}
