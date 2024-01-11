using System.Globalization;
using System.Text.RegularExpressions;

namespace FurnitureStore.Server.Utils
{
    public class VariableHelpers
    {
        public static bool IsNull<T>(IEnumerable<T>? valueToCheck)
        {
            return valueToCheck == null || !valueToCheck.Any();
        }

        public static bool IsNull<T>(T? valueToCheck)
        {
            return valueToCheck == null;
        }

        public static bool IsNull(string? valueToCheck)
        {
            return string.IsNullOrEmpty(valueToCheck);
        }

        public static bool BeAValidDate(DateTime? date)
        {
            return date != null && date != default(DateTime);
        }

        public static int RoundToThoudsand(double value)
        {
            return (int)(Math.Floor(value / 1000) * 1000);
        }

        public static string ToCamelCase(string str)
        {
            return char.ToUpper(str[0]) + str.Substring(1);
        }
    }
}
