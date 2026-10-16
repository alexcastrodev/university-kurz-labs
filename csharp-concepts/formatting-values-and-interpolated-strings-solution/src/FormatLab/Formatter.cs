using System.Globalization;
using System.Text;

namespace FormatLab;

public static class Formatter
{
    private static readonly CompositeFormat GreetingTemplate =
        CompositeFormat.Parse("Hello {0}, you have {1:N0} new messages");

    public static string Row(string name, decimal price, CultureInfo culture) =>
        string.Create(culture, $"{name,-10}{price,10:N2}");

    public static string CacheKey(string product, decimal price) =>
        string.Create(CultureInfo.InvariantCulture, $"{product}:{price}");

    public static string Compact(double value) =>
        value.ToString("0.##", CultureInfo.InvariantCulture);

    public static string Greeting(string name, int count, CultureInfo culture) =>
        string.Format(culture, GreetingTemplate, name, count);
}
