# BettyGamePlatform

## Overview
BettyGamePlatform is a *.NET 8* console application that simulates a simple slot-style game.
The app maintains a wallet balance and lets the user:
- *deposit* funds
- *withdraw* funds
- *bet* an amount to play a single game round

Game behavior (stake limits, probabilities, and payout multipliers) is configured via appsettings.json.

---

## Project structure
- *BettyGamePlatform.Cli*  
  Console entry point and command handling. Loads configuration and wires dependencies (DI).

- *BettyGamePlatform.Core*  
  Core domain/game logic: wallet operations, game rules, and round execution.

- *BettyGamePlatform.Tests*  
  Tests for the core logic and command handling.

---

## How to run

### From the solution root
- dotnet restore
- dotnet build
- dotnet run --project BettyGamePlatform.Cli

---

## Command format

The app prompts you to submit an action. Use one of the following formats:

- deposit {amount}

- withdraw {amount}

- bet {amount}

- exit

### Amount rules
- Must be *greater than 0*
- Up to *2 decimal places*
- Decimal separator can be . or ,

---

## Configuration: appsettings.json (Slot game rules)

File location:
- BettyGamePlatform.Cli/appsettings.json

Main settings (under SlotGame) include:
- MinStake / MaxStake  
  Allowed range for bet amounts.

- LoseProbability  
  Probability of losing a round (value between 0 and 1).

- SmallWinProbability  
  Probability of a “small win” (value between 0 and 1).

- SmallWinMinMultiplier / SmallWinMaxMultiplier  
  Multiplier range used when the outcome is a small win.

- BigWinMinMultiplier / BigWinMaxMultiplier  
  Multiplier range used when the outcome is a big win.


## Tests
Run all tests from the repository root:
- dotnet test
