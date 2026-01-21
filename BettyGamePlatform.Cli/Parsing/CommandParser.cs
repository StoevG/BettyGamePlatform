namespace BettyGamePlatform.Cli.Parsing
{
    public sealed class CommandParser
    {
        public bool TryParse(string? input, out ParsedCommand? command)
        {
            command = null;

            if (string.IsNullOrWhiteSpace(input))
            {
                return false;
            }

            var parts = input.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length == 1 && parts[0].Equals("exit", StringComparison.OrdinalIgnoreCase))
            {
                command = new ParsedCommand(CommandType.Exit, 0m);
                return true;
            }

            if (parts.Length != 2)
            {
                return false;
            }

            var action = parts[0].Trim();

            var type = MapCommandType(action);
            if (type is null)
            {
                return false;
            }

            if (!MoneyInputParser.TryParsePositiveAmount(parts[1], out var amount))
            {
                return false;
            }

            command = new ParsedCommand(type.Value, amount);
            return true;
        }

        private static CommandType? MapCommandType(string action)
        {
            if (action.Equals("deposit", StringComparison.OrdinalIgnoreCase)) { return CommandType.Deposit; }

            if (action.Equals("withdraw", StringComparison.OrdinalIgnoreCase)) { return CommandType.Withdraw; }

            if (action.Equals("bet", StringComparison.OrdinalIgnoreCase)) { return CommandType.Bet; }

            return null;
        }
    }
}
