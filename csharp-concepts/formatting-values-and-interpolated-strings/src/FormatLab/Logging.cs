using System.Runtime.CompilerServices;

namespace FormatLab;

public sealed class Logger
{
    public bool Enabled { get; set; } = true;

    public List<string> Lines { get; } = [];

    public void Debug([InterpolatedStringHandlerArgument("")] DebugHandler handler)
    {
        if (handler.IsEnabled)
        {
            Lines.Add(handler.ToStringAndClear());
        }
    }
}

[InterpolatedStringHandler]
public ref struct DebugHandler
{
    private DefaultInterpolatedStringHandler _inner;

    public readonly bool IsEnabled;

    public DebugHandler(int literalLength, int formattedCount, Logger logger, out bool shouldAppend)
    {
        // TODO-04: Let the logger decide. When logger.Enabled is false, shouldAppend must be false:
        //   the compiler then skips every AppendLiteral/AppendFormatted call and does not even
        //   evaluate the expressions inside the holes.
        shouldAppend = true;
        IsEnabled = shouldAppend;
        _inner = new DefaultInterpolatedStringHandler(literalLength, formattedCount);
    }

    public void AppendLiteral(string value) => _inner.AppendLiteral(value);

    public void AppendFormatted<T>(T value) => _inner.AppendFormatted(value);

    public void AppendFormatted<T>(T value, string? format) => _inner.AppendFormatted(value, format);

    public void AppendFormatted<T>(T value, int alignment) => _inner.AppendFormatted(value, alignment);

    public void AppendFormatted<T>(T value, int alignment, string? format) =>
        _inner.AppendFormatted(value, alignment, format);

    public string ToStringAndClear() => _inner.ToStringAndClear();
}
