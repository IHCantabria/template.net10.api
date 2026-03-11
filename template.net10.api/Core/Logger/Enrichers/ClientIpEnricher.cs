using System.Diagnostics.CodeAnalysis;
using Serilog.Core;
using Serilog.Events;

namespace template.net10.api.Core.Logger.Enrichers;

/// <summary>
///     Serilog enricher that adds the client IP address to log events from the HTTP connection.
/// </summary>
internal sealed class ClientIpEnricher : ILogEventEnricher
{
    /// <summary>
    ///     The log event property name used to store the client IP address.
    /// </summary>
    private const string IpAddressPropertyName = "client.ip";

    /// <summary>
    ///     The HTTP context item key used to cache the resolved IP address property.
    /// </summary>
    private const string IpAddressItemKey = "Serilog_ClientIp";

    /// <summary>
    ///     The HTTP context accessor used to obtain the current request context.
    /// </summary>
    private readonly IHttpContextAccessor _contextAccessor;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ClientIpEnricher" /> class using a default
    ///     <see cref="HttpContextAccessor" />.
    /// </summary>
    public ClientIpEnricher() : this(new HttpContextAccessor())
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ClientIpEnricher" /> class with the specified context accessor.
    /// </summary>
    /// <param name="contextAccessor">The HTTP context accessor to use.</param>
    private ClientIpEnricher(IHttpContextAccessor contextAccessor)
    {
        _contextAccessor = contextAccessor;
    }

    /// <summary>
    ///     Enriches the log event with the client IP address from the current HTTP connection.
    /// </summary>
    /// <param name="logEvent">The log event to enrich.</param>
    /// <param name="propertyFactory">The factory used to create log event properties.</param>
    /// <exception cref="ArgumentNullException">
    ///     <paramref name="logEvent" /> is <see langword="null" />.
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

        var property = ResolveIpProperty();
        if (property is null)
            return;

        logEvent.AddPropertyIfAbsent(property);
    }

    /// <summary>
    ///     Resolves the client IP address property from the HTTP context, using a cached value when available.
    /// </summary>
    /// <returns>
    ///     A <see cref="LogEventProperty" /> containing the client IP, or <see langword="null" /> if no HTTP context is
    ///     available.
    /// </returns>
    private LogEventProperty? ResolveIpProperty()
    {
        var httpContext = _contextAccessor.HttpContext;
        if (httpContext == null)
            return null;

        var ipAddress =
            httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        if (httpContext.Items.TryGetValue(IpAddressItemKey, out var existing)
            && existing is LogEventProperty property)
        {
            if (!string.Equals(
                    ((ScalarValue)property.Value).Value?.ToString(),
                    ipAddress,
                    StringComparison.Ordinal))
                property = new LogEventProperty(
                    IpAddressPropertyName,
                    new ScalarValue(ipAddress));

            return property;
        }

        var newProperty =
            new LogEventProperty(IpAddressPropertyName, new ScalarValue(ipAddress));

        httpContext.Items[IpAddressItemKey] = newProperty;

        return newProperty;
    }
}