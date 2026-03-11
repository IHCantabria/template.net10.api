using System.ComponentModel.DataAnnotations;
using System.Numerics;
using Microsoft.Extensions.Options;

namespace template.net10.api.Core.OpenTelemetry.Options;

/// <summary>
///     Configuration options for OpenTelemetry log, metric, and trace endpoints with authentication support.
/// </summary>
internal sealed record OpenTelemetryOptions : IEqualityOperators<OpenTelemetryOptions, OpenTelemetryOptions, bool>
{
    /// <summary>
    ///     The configuration section name for OpenTelemetry options binding.
    /// </summary>
    public const string OpenTelemetry = nameof(OpenTelemetry);

    /// <summary>
    ///     Gets a value indicating whether OpenTelemetry log export is active.
    /// </summary>
    [Required]
    public required bool IsLogActive { get; init; }

    /// <summary>
    ///     Gets the service name reported in OpenTelemetry resource attributes.
    /// </summary>
    [Required]
    public required string ServiceName { get; init; }

    /// <summary>
    ///     Gets the HTTP header name used for log endpoint API key authentication.
    /// </summary>
    public required string? LogEndpointApiKeyHeader { get; init; }

    /// <summary>
    ///     Gets the API key value sent in the log endpoint authentication header.
    /// </summary>
    public required string? LogEndpointApiKeyValue { get; init; }

    /// <summary>
    ///     Gets the URI of the OpenTelemetry log export endpoint.
    /// </summary>
    [Required]
    public required Uri LogEndpointUrl { get; init; }

    /// <summary>
    ///     Gets the HTTP header name used for metric endpoint API key authentication.
    /// </summary>
    public required string? MetricEndpointApiKeyHeader { get; init; }

    /// <summary>
    ///     Gets the API key value sent in the metric endpoint authentication header.
    /// </summary>
    public required string? MetricEndpointApiKeyValue { get; init; }

    /// <summary>
    ///     Gets a value indicating whether OpenTelemetry metric export is active.
    /// </summary>
    [Required]
    public required bool IsMetricActive { get; init; }

    /// <summary>
    ///     Gets the URI of the OpenTelemetry metric export endpoint.
    /// </summary>
    public required Uri? MetricEndpointUrl { get; init; }

    /// <summary>
    ///     Gets the HTTP header name used for trace endpoint API key authentication.
    /// </summary>
    public required string? TraceEndpointApiKeyHeader { get; init; }

    /// <summary>
    ///     Gets the API key value sent in the trace endpoint authentication header.
    /// </summary>
    public required string? TraceEndpointApiKeyValue { get; init; }

    /// <summary>
    ///     Gets a value indicating whether OpenTelemetry trace export is active.
    /// </summary>
    [Required]
    public required bool IsTraceActive { get; init; }

    /// <summary>
    ///     Gets the URI of the OpenTelemetry trace export endpoint.
    /// </summary>
    public required Uri? TraceEndpointUrl { get; init; }

    /// <summary>
    ///     Determines whether the log endpoint requires API key header authentication.
    /// </summary>
    /// <returns><see langword="true"/> if both header name and value are provided; otherwise, <see langword="false"/>.</returns>
    internal bool UseLogHeaderApiKey()
    {
        if (string.IsNullOrEmpty(LogEndpointApiKeyHeader))
            return false;

        return !string.IsNullOrEmpty(LogEndpointApiKeyValue);
    }

    /// <summary>
    ///     Determines whether the metric endpoint requires API key header authentication.
    /// </summary>
    /// <returns><see langword="true"/> if both header name and value are provided; otherwise, <see langword="false"/>.</returns>
    internal bool UseMetricHeaderApiKey()
    {
        if (string.IsNullOrEmpty(MetricEndpointApiKeyHeader))
            return false;

        return !string.IsNullOrEmpty(MetricEndpointApiKeyValue);
    }

    /// <summary>
    ///     Determines whether the metric endpoint URL is a valid non-empty URI.
    /// </summary>
    /// <returns><see langword="true"/> if the metric endpoint URL is valid; otherwise, <see langword="false"/>.</returns>
    internal bool IsValidMetricUri()
    {
        return !string.IsNullOrEmpty(MetricEndpointUrl?.ToString());
    }

    /// <summary>
    ///     Determines whether the trace endpoint requires API key header authentication.
    /// </summary>
    /// <returns><see langword="true"/> if both header name and value are provided; otherwise, <see langword="false"/>.</returns>
    internal bool UseTraceHeaderApiKey()
    {
        if (string.IsNullOrEmpty(TraceEndpointApiKeyHeader))
            return false;

        return !string.IsNullOrEmpty(TraceEndpointApiKeyValue);
    }

    /// <summary>
    ///     Determines whether the trace endpoint URL is a valid non-empty URI.
    /// </summary>
    /// <returns><see langword="true"/> if the trace endpoint URL is valid; otherwise, <see langword="false"/>.</returns>
    internal bool IsValidTraceUri()
    {
        return !string.IsNullOrEmpty(TraceEndpointUrl?.ToString());
    }
}

/// <summary>
///     Validates <see cref="OpenTelemetryOptions"/> using data annotation attributes.
/// </summary>
[OptionsValidator]
internal sealed partial class OpenTelemetryOptionsValidator : IValidateOptions<OpenTelemetryOptions>;