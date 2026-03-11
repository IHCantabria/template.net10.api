using System.Diagnostics.CodeAnalysis;
using Serilog.Core;
using Serilog.Events;

namespace template.net10.api.Core.Logger.Enrichers;

/// <summary>
///     Serilog enricher that adds the current thread name to log events.
/// </summary>
internal sealed class ThreadNameEnricher : ILogEventEnricher
{
    /// <summary>
    ///     The cached last created <c>request.thread.name</c> property. Frequently reused to avoid heap allocations.
    /// </summary>
    private LogEventProperty? _lastValue;

    /// <summary>
    ///     Enriches the log event with the current thread name as <c>request.thread.name</c>.
    /// </summary>
    /// <param name="logEvent">The log event to enrich.</param>
    /// <param name="propertyFactory">The factory used to create log event properties.</param>
    /// <exception cref="ArgumentNullException">
    ///     <paramref name="logEvent" /> is <see langword="null" />.
    ///     -or-
    ///     <paramref name="propertyFactory" /> is <see langword="null" />.
    /// </exception>
    [SuppressMessage(
        "ReSharper",
        "ExceptionNotDocumentedOptional",
        Justification =
            "Potential exceptions originate from underlying implementation details and are not part of the method contract.")]
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        ArgumentNullException.ThrowIfNull(logEvent);
        ArgumentNullException.ThrowIfNull(propertyFactory);

        var threadName = Thread.CurrentThread.Name;
        if (threadName is null) return;

        var last = _lastValue;
        if (last?.Value is not ScalarValue { Value: string currentName } ||
            !string.Equals(currentName, threadName, StringComparison.Ordinal))
            _lastValue = last =
                new LogEventProperty("request.thread.name", new ScalarValue(threadName));

        logEvent.AddPropertyIfAbsent(last);
    }
}