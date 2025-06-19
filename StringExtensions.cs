namespace Diamonds;

public static class StringExtensions
{
    public static double? ToDouble(this string @string)
    {
        return double.TryParse(@string, out var @double) ? @double : null;
    }
}