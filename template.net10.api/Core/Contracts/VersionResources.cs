using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Text.Json.Serialization;
using template.net10.api.Core.Interfaces;

namespace template.net10.api.Core.Contracts;

/// <summary>
///     Represents an API version resource containing version metadata.
/// </summary>
[SuppressMessage("Design",
    "CA1515:Consider making public types internal",
    Justification =
        "Contracts must remain public to allow proper discovery and schema generation by OpenAPI.")]
public sealed record VersionResource : IPublicApiContract, IEqualityOperators<VersionResource, VersionResource, bool>
{
    /// <summary>
    ///     Gets the numeric identifier of the API version.
    /// </summary>
    [JsonRequired]
    public required short Id { get; init; }

    /// <summary>
    ///     Gets the display name of the API version.
    /// </summary>
    [JsonRequired]
    public required string Name { get; init; }

    /// <summary>
    ///     Gets the version tag label (e.g., "v1.0").
    /// </summary>
    [JsonRequired]
    public required string Tag { get; init; }

    /// <summary>
    ///     Gets the release date of this API version.
    /// </summary>
    [JsonRequired]
    public required DateTime Date { get; init; }
}