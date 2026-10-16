namespace BindingLab;

public record Caller(string Name)
{
    // Header first, query string as fallback. Return null (400) instead of throwing (500).
    public static ValueTask<Caller?> BindAsync(HttpContext ctx)
    {
        string? name = ctx.Request.Headers["X-Caller"].FirstOrDefault() ?? ctx.Request.Query["caller"].FirstOrDefault();
        return ValueTask.FromResult(string.IsNullOrWhiteSpace(name) ? null : new Caller(name));
    }
}
