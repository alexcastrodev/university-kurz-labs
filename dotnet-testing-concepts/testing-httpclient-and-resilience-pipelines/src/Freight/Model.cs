namespace Freight;

public sealed record QuoteRequest(string FromZip, string ToZip, decimal WeightKg);

public sealed record Quote(decimal Price, string Currency, int Days);

public enum FreightOutcome
{
    Ok,
    // The API said no (a 4xx other than 408 and 429). Retrying would not help.
    Rejected,
    // The API could not answer: 5xx, 408 or 429 after the retries, a timeout, an open circuit, the network.
    Unavailable,
}

public sealed record FreightResult(FreightOutcome Outcome, Quote? Quote = null, string? Reason = null)
{
    public static FreightResult Ok(Quote? quote = null) => new(FreightOutcome.Ok, quote);

    public static FreightResult Rejected(string reason) => new(FreightOutcome.Rejected, Reason: reason);

    public static FreightResult Unavailable(string reason) => new(FreightOutcome.Unavailable, Reason: reason);
}
