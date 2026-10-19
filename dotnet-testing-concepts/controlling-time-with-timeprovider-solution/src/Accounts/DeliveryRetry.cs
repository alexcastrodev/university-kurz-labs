namespace Accounts;

// Retries a delivery: waits 1 s, 2 s and 4 s between attempts (four attempts in all), and abandons the
// whole thing when 10 s have passed, even in the middle of an attempt that hangs.
public sealed class DeliveryRetry(TimeProvider time)
{
    public static readonly TimeSpan[] Backoff = [TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(4)];

    public static readonly TimeSpan Deadline = TimeSpan.FromSeconds(10);

    public async Task<bool> RunAsync(Func<CancellationToken, Task<bool>> attempt, CancellationToken ct)
    {
        using var deadline = new CancellationTokenSource(Deadline, time);
        using var linked = CancellationTokenSource.CreateLinkedTokenSource(ct, deadline.Token);

        try
        {
            foreach (var wait in Backoff)
            {
                if (await attempt(linked.Token))
                {
                    return true;
                }

                await Task.Delay(wait, time, linked.Token);
            }

            return await attempt(linked.Token);
        }
        catch (OperationCanceledException) when (deadline.IsCancellationRequested && !ct.IsCancellationRequested)
        {
            return false;
        }
    }
}
