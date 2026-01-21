using BettyGamePlatform.Cli.Parsing;
using FluentAssertions;

namespace BettyGamePlatform.Tests.Cli
{
    public sealed class MoneyInputParserTests
    {
        [Theory]
        [InlineData("10", 10)]
        [InlineData("10.5", 10.5)]
        [InlineData("10.50", 10.50)]
        [InlineData("10,5", 10.5)]
        [InlineData("10,50", 10.50)]
        public void TryParsePositiveAmount_ValidInputs_ShouldParse(string input, decimal expected)
        {
            var ok = MoneyInputParser.TryParsePositiveAmount(input, out var amount);

            ok.Should().BeTrue();
            amount.Should().Be(expected);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("0")]
        [InlineData("-1")]
        [InlineData("abc")]
        [InlineData("10.")]
        [InlineData("10,")]
        [InlineData("20,3242")]
        [InlineData("1,000.00")]
        [InlineData("1 000.00")]
        [InlineData("10,0.1")]
        public void TryParsePositiveAmount_InvalidInputs_ShouldFail(string input)
        {
            var ok = MoneyInputParser.TryParsePositiveAmount(input, out var amount);

            ok.Should().BeFalse();
            amount.Should().Be(0m);
        }
    }
}
