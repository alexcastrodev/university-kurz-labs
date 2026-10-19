namespace Accounts;

// No SMS between 22:00 and 07:00 in the local time zone of the provider.
public sealed class QuietHours(TimeProvider time)
{
    private static readonly TimeOnly Start = new(22, 0);
    private static readonly TimeOnly End = new(7, 0);

    // IsBetween handles a range that wraps past midnight; the end is exclusive.
    public bool IsQuietNow() => TimeOnly.FromDateTime(time.GetLocalNow().DateTime).IsBetween(Start, End);
}
