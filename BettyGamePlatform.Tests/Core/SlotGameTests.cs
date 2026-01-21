using BettyGamePlatform.Core.Application.Results;
using BettyGamePlatform.Core.Domain.Abstractions;
using BettyGamePlatform.Core.Domain.Games;
using Microsoft.Extensions.Options;
using FluentAssertions;

namespace BettyGamePlatform.Tests.Core
{
    public sealed class SlotGameTests
    {
        private sealed class FakeRandomProvider : IRandomProvider
        {
            private readonly double fixedProbability;
            private readonly decimal fixedMultiplier;

            public FakeRandomProvider(double fixedProbability, decimal fixedMultiplier)
            {
                this.fixedProbability = fixedProbability;
                this.fixedMultiplier = fixedMultiplier;
            }

            public double NextProbability() => fixedProbability;

            public decimal NextInRange(decimal minInclusive, decimal maxInclusive) => fixedMultiplier;
        }

        private static IOptions<SlotGameRules> CreateRules()
        {
            return Options.Create(new SlotGameRules
            {
                MinStake = 1m,
                MaxStake = 10m,
                LoseProbability = 0.5,
                SmallWinProbability = 0.4,
                SmallWinMinMultiplier = 1m,
                SmallWinMaxMultiplier = 2m,
                BigWinMinMultiplier = 2m,
                BigWinMaxMultiplier = 10m
            });
        }

        [Fact]
        public void PlayRound_WhenStakeIsBelowMin_Fails()
        {
            var game = new SlotGame(new FakeRandomProvider(fixedProbability: 0.2, fixedMultiplier: 1.5m), CreateRules());

            var result = game.PlayRound(0.99m);

            result.IsSuccess.Should().BeFalse();
            result.Code.Should().Be(OperationCode.InvalidStake);
            result.Error.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void PlayRound_WhenStakeIsAboveMax_Fails()
        {
            var game = new SlotGame(new FakeRandomProvider(fixedProbability: 0.2, fixedMultiplier: 1.5m), CreateRules());

            var result = game.PlayRound(10.01m);

            result.IsSuccess.Should().BeFalse();
            result.Code.Should().Be(OperationCode.InvalidStake);
        }

        [Fact]
        public void PlayRound_WhenChanceIsBelowLoseThreshold_ReturnsLoseWithZeroWin()
        {
            var game = new SlotGame(new FakeRandomProvider(fixedProbability: 0.10, fixedMultiplier: 1.5m), CreateRules());

            var result = game.PlayRound(5m);

            result.IsSuccess.Should().BeTrue();
            result.Code.Should().Be(OperationCode.BetLose);
            result.Amount.Should().Be(5m);
            result.WinAmount.Should().Be(0m);
        }

        [Fact]
        public void PlayRound_WhenChanceIsAtLoseThreshold_GoesToSmallWinBranch()
        {
            var game = new SlotGame(new FakeRandomProvider(fixedProbability: 0.50, fixedMultiplier: 1.5m), CreateRules());

            var result = game.PlayRound(4m);

            result.IsSuccess.Should().BeTrue();
            result.Code.Should().Be(OperationCode.BetWin);
            result.WinAmount.Should().Be(6m); // 4 * 1.5
        }

        [Fact]
        public void PlayRound_WhenChanceIsInSmallWinRange_ReturnsWinUsingSmallMultiplier()
        {
            var game = new SlotGame(new FakeRandomProvider(fixedProbability: 0.60, fixedMultiplier: 2.0m), CreateRules());

            var result = game.PlayRound(3m);

            result.IsSuccess.Should().BeTrue();
            result.Code.Should().Be(OperationCode.BetWin);
            result.WinAmount.Should().Be(6m); // 3 * 2.0
        }

        [Fact]
        public void PlayRound_WhenChanceIsAtSmallWinUpperBoundary_GoesToBigWinBranch()
        {
            var game = new SlotGame(new FakeRandomProvider(fixedProbability: 0.90, fixedMultiplier: 3.0m), CreateRules());

            var result = game.PlayRound(2m);

            result.IsSuccess.Should().BeTrue();
            result.Code.Should().Be(OperationCode.BetWin);
            result.WinAmount.Should().Be(6m); // 2 * 3.0
        }

        [Fact]
        public void PlayRound_WhenChanceIsInBigWinRange_ReturnsWinUsingBigMultiplier()
        {
            var game = new SlotGame(new FakeRandomProvider(fixedProbability: 0.95, fixedMultiplier: 10.0m), CreateRules());

            var result = game.PlayRound(1m);

            result.IsSuccess.Should().BeTrue();
            result.Code.Should().Be(OperationCode.BetWin);
            result.WinAmount.Should().Be(10m); 
        }
    }
}
