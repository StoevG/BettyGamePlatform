using BettyGamePlatform.Core.Application.Abstractions;
using BettyGamePlatform.Core.Application.Results;
using BettyGamePlatform.Core.Application.Services;
using BettyGamePlatform.Core.Domain;
using BettyGamePlatform.Core.Domain.Games;
using FluentAssertions;
using Moq;

namespace BettyGamePlatform.Tests.Core
{
    public sealed class PlayerWalletServiceTests
    {
        [Fact]
        public void Deposit_WhenAmountIsValid_ReturnsDepositSuccessAndUpdatesBalance()
        {
            var wallet = new Wallet();
            var resolver = new Mock<IGameResolver>(MockBehavior.Strict);

            var service = new PlayerWalletService(wallet, resolver.Object);

            var result = service.Deposit(10m);

            result.IsSuccess.Should().BeTrue();
            result.Code.Should().Be(OperationCode.DepositSuccess);
            result.Balance.Should().Be(10m);
            result.Amount.Should().Be(10m);
        }

        [Fact]
        public void Deposit_WhenAmountIsInvalid_ReturnsInvalidAmount()
        {
            var wallet = new Wallet();
            var resolver = new Mock<IGameResolver>(MockBehavior.Strict);

            var service = new PlayerWalletService(wallet, resolver.Object);

            var result = service.Deposit(0m);

            result.IsSuccess.Should().BeFalse();
            result.Code.Should().Be(OperationCode.InvalidAmount);
            result.Balance.Should().Be(0m);
        }

        [Fact]
        public void Withdraw_WhenFundsAreEnough_ReturnsWithdrawSuccessAndUpdatesBalance()
        {
            var wallet = new Wallet();
            wallet.Deposit(20m);

            var resolver = new Mock<IGameResolver>(MockBehavior.Strict);
            var service = new PlayerWalletService(wallet, resolver.Object);

            var result = service.Withdraw(7.5m);

            result.IsSuccess.Should().BeTrue();
            result.Code.Should().Be(OperationCode.WithdrawSuccess);
            result.Balance.Should().Be(12.5m);
            result.Amount.Should().Be(7.5m);
        }

        [Fact]
        public void Withdraw_WhenFundsAreNotEnough_ReturnsInsufficientFunds()
        {
            var wallet = new Wallet();
            wallet.Deposit(5m);

            var resolver = new Mock<IGameResolver>(MockBehavior.Strict);
            var service = new PlayerWalletService(wallet, resolver.Object);

            var result = service.Withdraw(6m);

            result.IsSuccess.Should().BeFalse();
            result.Code.Should().Be(OperationCode.InsufficientFunds);
            result.Balance.Should().Be(5m);
            result.Amount.Should().Be(6m);
        }

        [Fact]
        public void Bet_WhenBalanceIsInsufficient_DoesNotCallGameAndReturnsInsufficientFunds()
        {
            var wallet = new Wallet();
            wallet.Deposit(1m);

            var game = new Mock<IGame>(MockBehavior.Strict); // should not be called
            var resolver = new Mock<IGameResolver>(MockBehavior.Strict);
            resolver.Setup(r => r.GetDefaultGame()).Returns(game.Object);

            var service = new PlayerWalletService(wallet, resolver.Object);

            var result = service.Bet(5m);

            result.IsSuccess.Should().BeFalse();
            result.Code.Should().Be(OperationCode.InsufficientFunds);
            result.Balance.Should().Be(1m);
            result.Amount.Should().Be(5m);

            // important: no RNG / no game call
            game.VerifyNoOtherCalls();
        }

        [Fact]
        public void Bet_WhenStakeIsInvalidAndBalanceIsEnough_ReturnsInvalidStake()
        {
            var wallet = new Wallet();
            wallet.Deposit(100m);

            var game = new Mock<IGame>(MockBehavior.Strict);
            game.Setup(g => g.PlayRound(11m))
                .Returns(OperationResult.Failure(OperationCode.InvalidStake, balance: 0m, amount: 11m, error: "Bet amount must be between 1 and 10."));

            var resolver = new Mock<IGameResolver>(MockBehavior.Strict);
            resolver.Setup(r => r.GetDefaultGame()).Returns(game.Object);

            var service = new PlayerWalletService(wallet, resolver.Object);

            var result = service.Bet(11m);

            result.IsSuccess.Should().BeFalse();
            result.Code.Should().Be(OperationCode.InvalidStake);
            result.Balance.Should().Be(100m);
            result.Amount.Should().Be(11m);
            result.Error.Should().NotBeNullOrWhiteSpace();

            game.VerifyAll();
        }

        [Fact]
        public void Bet_WhenLose_UpdatesBalanceAndReturnsBetLose()
        {
            var wallet = new Wallet();
            wallet.Deposit(10m);

            var game = new Mock<IGame>(MockBehavior.Strict);
            game.Setup(g => g.PlayRound(4m))
                .Returns(OperationResult.Success(OperationCode.BetLose, balance: 0m, amount: 4m, winAmount: 0m));

            var resolver = new Mock<IGameResolver>(MockBehavior.Strict);
            resolver.Setup(r => r.GetDefaultGame()).Returns(game.Object);

            var service = new PlayerWalletService(wallet, resolver.Object);

            var result = service.Bet(4m);

            result.IsSuccess.Should().BeTrue();
            result.Code.Should().Be(OperationCode.BetLose);
            result.Balance.Should().Be(6m);
            result.Amount.Should().Be(4m);
            result.WinAmount.Should().Be(0m);

            game.VerifyAll();
        }

        [Fact]
        public void Bet_WhenWin_UpdatesBalanceAndReturnsBetWin()
        {
            var wallet = new Wallet();
            wallet.Deposit(10m);

            var game = new Mock<IGame>(MockBehavior.Strict);
            game.Setup(g => g.PlayRound(5m))
                .Returns(OperationResult.Success(OperationCode.BetWin, balance: 0m, amount: 5m, winAmount: 8m));

            var resolver = new Mock<IGameResolver>(MockBehavior.Strict);
            resolver.Setup(r => r.GetDefaultGame()).Returns(game.Object);

            var service = new PlayerWalletService(wallet, resolver.Object);

            var result = service.Bet(5m);

            result.IsSuccess.Should().BeTrue();
            result.Code.Should().Be(OperationCode.BetWin);
            result.Balance.Should().Be(13m);
            result.Amount.Should().Be(5m);
            result.WinAmount.Should().Be(8m);

            game.VerifyAll();
        }
    }
}
