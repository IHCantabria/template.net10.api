using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Text.Json.Serialization;
using template.net10.api.Core.Interfaces;

namespace template.net10.api.Core.Contracts;

/// <summary>
///     Represents an application information resource containing status, version, and environment details.
/// </summary>
[SuppressMessage("Design",
    "CA1515:Consider making public types internal",
    Justification =
        "Contracts must remain public to allow proper discovery and schema generation by OpenAPI.")]
public sealed record InfoResource : IPublicApiContract, IEqualityOperators<InfoResource, InfoResource, bool>
{
    /// <summary>
    ///     Gets the HTTP status code indicating the application's current state.
    /// </summary>
    [JsonRequired]
    public required short Status { get; init; }

    /// <summary>
    ///     Gets a human-readable label describing the application's current operational status (e.g., "OK", "Degraded").
    /// </summary>
    [JsonRequired]
    public required string StatusInfo { get; init; }

    /// <summary>
    ///     Gets the application version string, normalized without the leading <c>v</c> prefix.
    /// </summary>
    [JsonRequired]
    public required string Version { get; init; }

    /// <summary>
    ///     Gets the deployment environment name in upper case (e.g., <c>LOCAL</c>, <c>DEV</c>, <c>PRE</c>, <c>PROD</c>).
    /// </summary>
    [JsonRequired]
    public required string Environment { get; init; }
}
