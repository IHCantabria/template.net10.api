using System.Diagnostics;
using Serilog.Core;
using Serilog.Events;

namespace template.net10.api.Core.Logger.Enrichers;

/// <summary>
///     Serilog enricher that adds OpenTelemetry activity trace data (trace ID, span ID, parent span ID, and tags) to log
///     events.
/// </summary>
internal sealed class ActivityEnricher : ILogEventEnricher
{
    /// <summary>
    ///     Enriches the log event with trace ID, span ID, parent span ID, and activity tags from the current
    ///     <see cref="Activity" />.
    /// </summary>
    /// <param name="logEvent">The log event to enrich.</param>
    /// <param name="propertyFactory">The factory used to create log event properties.</param>
    /// <exception cref="ArgumentNullException">
    ///     <paramref name="logEvent" /> is <see langword="null" />.
    ///     <paramref name="propertyFactory" /> is <see langword="null" />.
    /// </exception>
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        ArgumentNullException.ThrowIfNull(logEvent);
        ArgumentNullException.ThrowIfNull(propertyFactory);

        var activity = Activity.Current;
        if (activity is null) return;

        logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("trace_id", activity.TraceId.ToString()));
        logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("span_id", activity.SpanId.ToString()));

        if (activity.ParentSpanId != default)
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("parent_span_id",
                activity.ParentSpanId.ToString()));

        foreach (var tag in activity.TagObjects)
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty(tag.Key, tag.Value));
    }
}