using Serilog.Core;
using Serilog.Events;
using ILogger = Serilog.ILogger;

namespace template.net10.api.Core.Logger.Sinks;

/// <summary>
///     ADD DOCUMENTATION
/// </summary>
internal sealed class MemorySink : ILogEventSink
{
    /// <summary>
    ///     ADD DOCUMENTATION
    /// </summary>
    private readonly List<LogEvent> _events = [];

    /// <summary>
    ///     ADD DOCUMENTATION
    /// </summary>
    public void Emit(LogEvent logEvent)
    {
        _events.Add(logEvent);
    }

    /// <summary>
    ///     ADD DOCUMENTATION
    /// </summary>
    public void FlushToLogger(ILogger logger)
    {
        var eventsToFlush = _events.ToList();
        _events.Clear();

        foreach (var logEvent in eventsToFlush) logger.Write(logEvent);
    }
}