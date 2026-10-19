using System.Collections.Concurrent;
using System.Security.Cryptography;

namespace Accounts;

public enum TokenCheck
{
    Valid,
    Expired,
    Unknown,
}

// Issues opaque API tokens that are valid for 15 minutes.
public sealed class ApiTokenIssuer(TimeProvider time)
{
    public static readonly TimeSpan Lifetime = TimeSpan.FromMinutes(15);

    private readonly ConcurrentDictionary<string, Entry> _issued = new();

    public string Issue(Guid userId)
    {
        var token = Convert.ToHexString(RandomNumberGenerator.GetBytes(16));
        _issued[token] = new Entry(userId, time.GetUtcNow() + Lifetime);
        return token;
    }

    public TokenCheck Check(string token)
    {
        if (!_issued.TryGetValue(token, out var entry))
        {
            return TokenCheck.Unknown;
        }

        return entry.IsExpiredAt(time.GetUtcNow()) ? TokenCheck.Expired : TokenCheck.Valid;
    }

    // The nested type receives "now" from its owner instead of reading a clock of its own.
    private sealed record Entry(Guid UserId, DateTimeOffset ExpiresAt)
    {
        public bool IsExpiredAt(DateTimeOffset now) => now >= ExpiresAt;
    }
}
