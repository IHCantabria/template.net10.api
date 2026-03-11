using Serilog.Core;
using Serilog.Events;

namespace template.net10.api.Core.Logger.Enrichers;

/// <summary>
///     Serilog enricher that adds the current request identifier to log events.
/// </summary>
internal sealed class RequestIdentifierEnricher : ILogEventEnricher
{
    /// <summary>
    ///     Enriches the log event with the current request ID obtained from <see cref="RequestIdProvider" />.
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

        var requestId = RequestIdProvider.GetCurrentRequestId();

        if (string.IsNullOrEmpty(requestId)) return;

        logEvent.RemovePropertyIfPresent("RequestId");
        logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty("request.id", requestId));
    }
}