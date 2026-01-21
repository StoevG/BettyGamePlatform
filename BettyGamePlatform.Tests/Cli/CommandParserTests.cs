using BettyGamePlatform.Cli.Parsing;
using FluentAssertions;

namespace BettyGamePlatform.Tests.Cli
{
    public sealed class CommandParserTests
    {
        private readonly CommandParser parser = new();

        [Fact]
        public void TryParse_Exit_ShouldReturnExitCommand()
        {
            var ok = parser.TryParse("exit", out var cmd);

            ok.Should().BeTrue();
            cmd.Should().NotBeNull();
            cmd!.Type.Should().Be(CommandType.Exit);
            cmd.Amount.Should().Be(0m);
        }

        [Theory]
        [InlineData("deposit 10", CommandType.Deposit, 10)]
        [InlineData("withdraw 5.50", CommandType.Withdraw, 5.50)]
        [InlineData("bet 3,25", CommandType.Bet, 3.25)]
        public void TryParse_ValidCommands_ShouldParse(string input, CommandType type, decimal amount)
        {
            var ok = parser.TryParse(input, out var cmd);

            ok.Should().BeTrue();
            cmd.Should().NotBeNull();
            cmd!.Type.Should().Be(type);
            cmd.Amount.Should().Be(amount);
        }

        [Theory]
        [InlineData("deposit")]
        [InlineData("deposit 10 20")]
        [InlineData("unknown 10")]
        [InlineData("deposit -1")]
        [InlineData("deposit 20,3242")]
        [InlineData("deposit 1,000.00")]
        [InlineData("exit 10")]
        public void TryParse_Invalid_ShouldFail(string input)
        {
            var ok = parser.TryParse(input, out var cmd);

            ok.Should().BeFalse();
            cmd.Should().BeNull();
        }
    }
}
