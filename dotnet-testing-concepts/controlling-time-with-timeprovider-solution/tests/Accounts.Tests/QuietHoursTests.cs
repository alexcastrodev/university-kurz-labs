namespace Accounts.Tests;

[TestClass]
public sealed class QuietHoursTests
{
    // A custom zone works on every OS and does not depend on the machine running the tests.
    private static readonly TimeZoneInfo UtcMinus3 =
        TimeZoneInfo.CreateCustomTimeZone("Test/UTC-03", TimeSpan.FromHours(-3), "UTC-03", "UTC-03");

    [TestMethod]
    [DataRow(21, 59, false)]
    [DataRow(22, 0, true)]
    [DataRow(6, 59, true)]
    [DataRow(7, 0, false)]
    public void QuietHoursRunFromTenAtNightToSevenInTheMorningLocalTime(int hour, int minute, bool expected)
    {
        var clock = new FakeTimeProvider();
        clock.SetLocalTimeZone(UtcMinus3);
        var local = new DateTimeOffset(2026, 1, 5, hour, minute, 0, TimeSpan.FromHours(-3));
        // FakeTimeProvider keeps the offset it is given, so hand it a UTC value.
        clock.SetUtcNow(local.ToUniversalTime());

        Assert.AreEqual(expected, new QuietHours(clock).IsQuietNow());
    }
}
