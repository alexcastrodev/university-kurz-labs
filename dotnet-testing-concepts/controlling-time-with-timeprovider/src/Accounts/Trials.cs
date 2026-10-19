namespace Accounts;

public sealed record Trial(Guid CustomerId, DateTimeOffset StartedAt, DateTimeOffset EndsAt);

public enum TrialStatus
{
    Active,
    Expired,
}

// A free trial lasts 14 days, to the tick. Every read of "now" goes through the injected TimeProvider.
public sealed class TrialService(TimeProvider time)
{
    public static readonly TimeSpan Length = TimeSpan.FromDays(14);

    public Trial Start(Guid customerId)
    {
        var now = time.GetUtcNow();
        return new Trial(customerId, now, now + Length);
    }

    public TrialStatus StatusOf(Trial trial) =>
        time.GetUtcNow() < trial.EndsAt ? TrialStatus.Active : TrialStatus.Expired;

    public TimeSpan Remaining(Trial trial)
    {
        var left = trial.EndsAt - time.GetUtcNow();
        return left > TimeSpan.Zero ? left : TimeSpan.Zero;
    }
}
