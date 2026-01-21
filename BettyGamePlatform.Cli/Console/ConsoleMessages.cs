namespace BettyGamePlatform.Cli.Console
{
    public static class ConsoleMessages
    {
        public const string PromptMessage = "Please, submit action:";

        public const string InvalidInputMessage =
            "Invalid input.\n" +
            "Commands: deposit <amount> | withdraw <amount> | bet <amount> | exit\n" +
            "Amount: positive, max 2 decimals, decimal separator '.' or ','.\n" +
            "Examples: deposit 10.50 | deposit 10,50 | withdraw 5 | bet 3";

        public const string InvalidStakeMessage = "Invalid bet amount. Bet must be between $1 and $10.";

        public const string DepositSuccessMessage = "Your deposit of ${0:0.00} was successful. Your current balance is: ${1:0.00}";

        public const string WithdrawSuccessMessage = "Your withdrawal of ${0:0.00} was successful. Your current balance is: ${1:0.00}";

        public const string BetWinMessage = "Congrats - you won ${0:0.00}! Your current balance is: ${1:0.00}";

        public const string BetLoseMessage = "No luck this time! Your current balance is: ${0:0.00}";

        public const string InsufficientFundsMessage = "Insufficient funds. Your current balance is: ${0:0.00}";

        public const string ExitMessage = "Thank you for playing! Hope to see you again soon.";

        public const string PressAnyKeyToExitMessage = "Press any key to exit.";
    }
}