using Serilog.Core;
using Serilog.Events;

namespace template.net10.api.Core.Logger.Enrichers;

/// <summary>
///     Serilog enricher that adds a correlation ID to log events, resolved from HTTP headers or generated automatically.
/// </summary>
internal sealed class CorrelationIdEnricher : ILogEventEnricher
{
    /// <summary>
    ///     The HTTP context item key used to cache the correlation ID log event property.
    /// </summary>
    private const string CorrelationIdItemKey = "Serilog_CorrelationId";

    /// <summary>
    ///     The log event property name for the correlation ID.
    /// </summary>
    private const string PropertyName = "correlation_id";

    /// <summary>
    ///     The HTTP context item key used to cache the raw correlation ID string value.
    /// </summary>
    private const string CorrelationIdValueKey = "Serilog_CorrelationId_Value";

    /// <summary>
    ///     Indicates whether to generate a new correlation ID when the header is absent.
    /// </summary>
    private readonly bool _addValueIfHeaderAbsence;

    /// <summary>
    ///     The HTTP context accessor used to obtain the current request context.
    /// </summary>
    private readonly IHttpContextAccessor _contextAccessor;

    /// <summary>
    ///     The HTTP header key used to look up the correlation ID.
    /// </summary>
    private readonly string _headerKey;

    /// <summary>
    ///     Initializes a new instance of the <see cref="CorrelationIdEnricher" /> class with the specified header key and
    ///     auto-generation behavior.
    /// </summary>
    /// <param name="headerKey">The HTTP header key to look up the correlation ID.</param>
    /// <param name="addValueIfHeaderAbsence">Whether to generate a new GUID when the header is missing.</param>
    public CorrelationIdEnricher(string headerKey, bool addValueIfHeaderAbsence)
        : this(headerKey, addValueIfHeaderAbsence, new HttpContextAccessor())
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="CorrelationIdEnricher" /> class with the specified header key,
    ///     auto-generation behavior, and context accessor.
    /// </summary>
    /// <param name="headerKey">The HTTP header key to look up the correlation ID.</param>
    /// <param name="addValueIfHeaderAbsence">Whether to generate a new GUID when the header is missing.</param>
    /// <param name="contextAccessor">The HTTP context accessor to use.</param>
    private CorrelationIdEnricher(string headerKey, bool addValueIfHeaderAbsence, IHttpContextAccessor contextAccessor)
    {
        _headerKey = headerKey;
        _addValueIfHeaderAbsence = addValueIfHeaderAbsence;
        _contextAccessor = contextAccessor;
    }

    /// <summary>
    ///     Enriches the log event with a correlation ID resolved from HTTP headers or the cache.
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

        var httpContext = _contextAccessor.HttpContext;
        if (httpContext is null)
            return;

        if (TryEnrichFromCache(httpContext, logEvent))
            return;

        var correlationId = ResolveCorrelationId(httpContext);
        var correlationProperty = propertyFactory.CreateProperty(PropertyName, correlationId);

        logEvent.AddOrUpdateProperty(correlationProperty);
        CacheCorrelation(httpContext, correlationProperty, correlationId);
    }

    /// <summary>
    ///     Attempts to enrich the log event from a previously cached correlation ID in the HTTP context.
    /// </summary>
    /// <param name="httpContext">The current HTTP context.</param>
    /// <param name="logEvent">The log event to enrich.</param>
    /// <returns><see langword="true" /> if the log event was enriched from cache; otherwise, <see langword="false" />.</returns>
    private static bool TryEnrichFromCache(HttpContext httpContext, LogEvent logEvent)
    {
        if (!httpContext.Items.TryGetValue(CorrelationIdItemKey, out var existingValue) ||
            existingValue is not LogEventProperty existingProperty)
            return false;

        logEvent.AddPropertyIfAbsent(existingProperty);

        // Ensure the raw string value is stored for quick access later
        httpContext.Items.TryAdd(
            CorrelationIdValueKey,
            (existingProperty.Value as ScalarValue)?.Value as string);

        return true;
    }

    /// <summary>
    ///     Resolves the correlation ID from request or response headers, or generates a new one if configured.
    /// </summary>
    /// <param name="httpContext">The current HTTP context.</param>
    /// <returns>The resolved correlation ID string, or <see langword="null" /> if not found and auto-generation is disabled.</returns>
    private string? ResolveCorrelationId(HttpContext httpContext)
    {
        var headerValue = httpContext.Request.Headers[_headerKey].FirstOrDefault()
                          ?? httpContext.Response.Headers[_headerKey].FirstOrDefault();

        if (!string.IsNullOrWhiteSpace(headerValue))
            return headerValue;

        return _addValueIfHeaderAbsence ? Guid.NewGuid().ToString() : null;
    }

    /// <summary>
    ///     Caches the correlation ID property and its raw string value in the HTTP context for subsequent lookups.
    /// </summary>
    /// <param name="httpContext">The current HTTP context.</param>
    /// <param name="correlationProperty">The log event property to cache.</param>
    /// <param name="correlationId">The raw correlation ID string value.</param>
    private static void CacheCorrelation(
        HttpContext httpContext,
        LogEventProperty correlationProperty,
        string? correlationId)
    {
        httpContext.Items[CorrelationIdItemKey] = correlationProperty;
        httpContext.Items[CorrelationIdValueKey] = correlationId;
    }
}