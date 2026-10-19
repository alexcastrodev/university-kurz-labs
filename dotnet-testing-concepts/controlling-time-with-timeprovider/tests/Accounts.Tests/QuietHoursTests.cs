namespace Accounts.Tests;

// TODO-03: Read src/Accounts/QuietHours.cs. Quiet hours are 22:00 to 07:00 in the provider's local zone.
[TestClass]
public sealed class QuietHoursTests
{
    [TestMethod]
    public void QuietHoursRunFromTenAtNightToSevenInTheMorningLocalTime()
    {
        // Turn this into one test with [DataRow] rows for 21:59, 22:00, 06:59 and 07:00 local time.
        // Do not depend on the time zone of the machine: build a zone with TimeZoneInfo.CreateCustomTimeZone
        // (for example UTC-03) and give it to the fake with SetLocalTimeZone. Then set the instant with
        // SetUtcNow. Careful: hand it a UTC value (ToUniversalTime()); the fake keeps the offset it gets.
        Assert.Fail("TODO-03: write this test.");
    }
}
