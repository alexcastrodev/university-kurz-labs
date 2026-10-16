using Microsoft.Extensions.Logging;

namespace EfLogLab;

public record LogEntry(string Category, LogLevel Level, EventId EventId, string Message);

public sealed class ListLoggerProvider : ILoggerProvider
{
    private readonly List<LogEntry> _entries = [];

    public IReadOnlyList<LogEntry> Entries
    {
        get { lock (_entries) return [.. _entries]; }
    }

    public ILogger CreateLogger(string categoryName) => new ListLogger(categoryName, this);

    public void Dispose() { }

    private void Add(LogEntry entry)
    {
        lock (_entries) _entries.Add(entry);
    }

    private sealed class ListLogger(string category, ListLoggerProvider owner) : ILogger
    {
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            owner.Add(new LogEntry(category, logLevel, eventId, formatter(state, exception)));
        }
    }
}
