using System.Globalization;

namespace FormatLab;

public readonly record struct Money(decimal Amount, string Currency) : IFormattable
{
    public string ToString(string? format, IFormatProvider? provider)
    {
        // TODO-03: Support three formats. null, "" and "short" give the amount with at most two
        //   decimals ("0.##") followed by the first letter of the currency: 12.5 EUR -> "12.5E".
        //   "long" gives the amount with "N2" followed by a space and the currency: "12.50 EUR".
        //   Any other format must throw FormatException. Use `provider ?? CultureInfo.CurrentCulture`.
        throw new NotImplementedException("Not implemented yet.");
    }
}
