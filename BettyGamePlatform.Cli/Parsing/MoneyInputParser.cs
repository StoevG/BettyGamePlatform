using System.Globalization;
using System.Text.RegularExpressions;

namespace BettyGamePlatform.Cli.Parsing
{
    public static class MoneyInputParser
    {
        // Accepts:
        //  - 10
        //  - 10.5 / 10.50
        //  - 10,5 / 10,50
        //
        // Rejects:
        //  - 1,000.00
        //  - 20,3242
        //  - 10. / 10,
        //  - mixed separators "1,234.56"
        private static readonly Regex MoneyRegex = new(@"^\d+([.,]\d{1,2})?$", RegexOptions.Compiled);

        public static bool TryParsePositiveAmount(string input, out decimal amount)
        {
            amount = 0m;

            if (string.IsNullOrWhiteSpace(input))
            {
                return false;
            }

            input = input.Trim();

            if (input.Contains(' '))
            {
                return false;
            }

            if (input.Contains(',') && input.Contains('.'))
            {
                return false;
            }

            if (!MoneyRegex.IsMatch(input))
            {
                return false;
            }

            var normalized = input.Replace(',', '.');

            if (!decimal.TryParse(normalized, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out amount))
            {
                return false;
            }

            return amount > 0m;
        }
    }
}