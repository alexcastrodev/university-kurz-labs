using System.Globalization;

namespace FormatLab;

public static class Formatter
{
    public static string Row(string name, decimal price, CultureInfo culture)
    {
        // TODO-00: Build "name" left-aligned in 10 characters followed by the price right-aligned in
        //   10 characters with two decimals and group separators (hole syntax {value,width:format}).
        //   Use string.Create(culture, $"...") so the result follows the culture you were given.
        throw new NotImplementedException("Not implemented yet.");
    }

    public static string CacheKey(string product, decimal price)
    {
        // TODO-01: Return "product:price", for example "tea:1.5". A cache key is data, not text for a
        //   person: it must not change when the machine culture is pt-BR or de-DE.
        throw new NotImplementedException("Not implemented yet.");
    }

    public static string Compact(double value)
    {
        // TODO-02: Show at most two decimals without trailing zeros: 0.5 -> "0.5", 1234.567 -> "1234.57",
        //   2 -> "2". Zero must be "0", and the result must not depend on the current culture.
        //   Careful: the custom format "#.##" gives an empty string for 0.
        throw new NotImplementedException("Not implemented yet.");
    }
}
