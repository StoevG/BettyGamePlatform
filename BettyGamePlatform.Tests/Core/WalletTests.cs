using BettyGamePlatform.Core.Application.Results;
using BettyGamePlatform.Core.Domain;
using FluentAssertions;

namespace BettyGamePlatform.Tests.Core
{
    public sealed class WalletTests
    {
        [Fact]
        public void NewWallet_StartsWithZeroBalance()
        {
            var wallet = new Wallet();

            wallet.Balance.Should().Be(0m);
        }

        [Fact]
        public void Deposit_AddsMoneyToBalance()
        {
            var wallet = new Wallet();

            var result = wallet.Deposit(10.25m);

            result.IsSuccess.Should().BeTrue();
            result.Code.Should().Be(OperationCode.DepositSuccess);
            wallet.Balance.Should().Be(10.25m);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-10.50)]
        public void Deposit_WithNonPositiveAmount_Fails(decimal amount)
        {
            var wallet = new Wallet();

            var result = wallet.Deposit(amount);

            result.IsSuccess.Should().BeFalse();
            result.Code.Should().Be(OperationCode.InvalidAmount);
            wallet.Balance.Should().Be(0m);
        }

        [Fact]
        public void Deposit_RoundsToTwoDecimals_AwayFromZero()
        {
            var wallet = new Wallet();

            wallet.Deposit(1.005m);

            wallet.Balance.Should().Be(1.01m);
        }

        [Fact]
        public void Withdraw_SubtractsMoneyFromBalance()
        {
            var wallet = new Wallet();
            wallet.Deposit(20m);

            var result = wallet.Withdraw(7.50m);

            result.IsSuccess.Should().BeTrue();
            result.Code.Should().Be(OperationCode.WithdrawSuccess);
            wallet.Balance.Should().Be(12.50m);
        }

        [Fact]
        public void Withdraw_WhenAmountExceedsBalance_FailsWithInsufficientFunds()
        {
            var wallet = new Wallet();
            wallet.Deposit(5m);

            var result = wallet.Withdraw(6m);

            result.IsSuccess.Should().BeFalse();
            result.Code.Should().Be(OperationCode.InsufficientFunds);
            wallet.Balance.Should().Be(5m);
        }

        [Fact]
        public void ApplyGameRound_UpdatesBalanceUsingFormula()
        {
            var wallet = new Wallet();
            wallet.Deposit(10m);

            // new = old - stake + win = 10 - 4 + 7 = 13
            var result = wallet.ApplyGameRound(stakeAmount: 4m, winAmount: 7m);

            result.IsSuccess.Should().BeTrue();
            wallet.Balance.Should().Be(13m);
        }

        [Fact]
        public void ApplyGameRound_WhenStakeExceedsBalance_FailsWithInsufficientFunds()
        {
            var wallet = new Wallet();
            wallet.Deposit(3m);

            var result = wallet.ApplyGameRound(stakeAmount: 4m, winAmount: 0m);

            result.IsSuccess.Should().BeFalse();
            result.Code.Should().Be(OperationCode.InsufficientFunds);
            wallet.Balance.Should().Be(3m);
        }

        [Fact]
        public void ApplyGameRound_WhenWinAmountIsNegative_Fails()
        {
            var wallet = new Wallet();
            wallet.Deposit(10m);

            var result = wallet.ApplyGameRound(stakeAmount: 1m, winAmount: -0.01m);

            result.IsSuccess.Should().BeFalse();
            result.Code.Should().Be(OperationCode.InvalidAmount);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void ApplyGameRound_WithNonPositiveStake_Fails(decimal stake)
        {
            var wallet = new Wallet();
            wallet.Deposit(10m);

            var result = wallet.ApplyGameRound(stakeAmount: stake, winAmount: 0m);

            result.IsSuccess.Should().BeFalse();
            result.Code.Should().Be(OperationCode.InvalidStake);
        }

        [Fact]
        public void ApplyGameRound_RoundsWinAmountToCents()
        {
            var wallet = new Wallet();
            wallet.Deposit(10m);

            // win rounds away from zero: 1.005 -> 1.01
            wallet.ApplyGameRound(stakeAmount: 1m, winAmount: 1.005m);

            wallet.Balance.Should().Be(10.01m); // 10 - 1 + 1.01
        }
    }
}
