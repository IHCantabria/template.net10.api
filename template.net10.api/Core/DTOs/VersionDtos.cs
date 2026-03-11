using System.Numerics;
using template.net10.api.Core.Interfaces;

namespace template.net10.api.Core.DTOs;

/// <summary>
///     Data transfer object containing API version metadata.
/// </summary>
internal sealed partial record VersionDto : IDto, IEqualityOperators<VersionDto, VersionDto, bool>
{
    /// <summary>
    ///     Gets the numeric identifier of the API version.
    /// </summary>
    internal required short Id { get; init; }

    /// <summary>
    ///     Gets the display name of the API version.
    /// </summary>
    internal required string Name { get; init; }

    /// <summary>
    ///     Gets the version tag label (e.g., "v1.0").
    /// </summary>
    internal required string Tag { get; init; }

    /// <summary>
    ///     Gets the release date and time of this API version.
    /// </summary>
    internal required DateTimeOffset Date { get; init; }
}