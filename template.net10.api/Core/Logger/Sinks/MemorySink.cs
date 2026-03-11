using Serilog.Core;
using Serilog.Events;
using ILogger = Serilog.ILogger;

namespace template.net10.api.Core.Logger.Sinks;

/// <summary>
///     In-memory Serilog sink that buffers log events until they can be flushed to the real logger.
/// </summary>
internal sealed class MemorySink : ILogEventSink
{
    /// <summary>
    ///     The buffered log events awaiting flush.
    /// </summary>
    private readonly List<LogEvent> _events = [];

    /// <summary>
    ///     Records a log event into the in-memory buffer.
    /// </summary>
    /// <param name="logEvent">The log event to buffer.</param>
    public void Emit(LogEvent logEvent)
    {
        _events.Add(logEvent);
    }

    /// <summary>
    ///     Flushes all buffered log events to the specified logger and clears the buffer.
    /// </summary>
    /// <param name="logger">The Serilog logger to write the buffered events to.</param>
    public void FlushToLogger(ILogger logger)
    {
        var eventsToFlush = _events.ToList();
        _events.Clear();

        foreach (var logEvent in eventsToFlush) logger.Write(logEvent);
    }
}