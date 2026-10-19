namespace Accounts;

// Allows a fixed number of login attempts per window. A timer created by the TimeProvider resets the
// counter at the end of every window.
public sealed class LoginThrottle : IDisposable
{
    private readonly int _attemptsPerWindow;
    private readonly ITimer _reset;
    private readonly Lock _gate = new();
    private int _used;

    public LoginThrottle(int attemptsPerWindow, TimeSpan window, TimeProvider time)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(attemptsPerWindow);
        _attemptsPerWindow = attemptsPerWindow;
        _reset = time.CreateTimer(_ => Reset(), state: null, dueTime: window, period: window);
    }

    public bool TryAttempt()
    {
        lock (_gate)
        {
            if (_used >= _attemptsPerWindow)
            {
                return false;
            }

            _used++;
            return true;
        }
    }

    private void Reset()
    {
        lock (_gate)
        {
            _used = 0;
        }
    }

    public void Dispose() => _reset.Dispose();
}
