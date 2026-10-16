namespace BindingLab;

public record Caller(string Name)
{
    public static ValueTask<Caller?> BindAsync(HttpContext ctx)
    {
        // TODO-03: Read the name from the X-Caller header, falling back to the "caller"
        // query string key. Return null (a 400) when neither has a value. Do not throw:
        // an exception here becomes a 500.
        return ValueTask.FromResult<Caller?>(null);
    }
}
