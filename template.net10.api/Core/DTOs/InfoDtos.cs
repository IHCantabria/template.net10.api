using System.Numerics;
using template.net10.api.Core.Interfaces;

namespace template.net10.api.Core.DTOs;

/// <summary>
///     Data transfer object containing application information details.
/// </summary>
internal sealed partial record InfoDto : IDto, IEqualityOperators<InfoDto, InfoDto, bool>
{
    /// <summary>
    ///     Gets the application version string.
    /// </summary>
    internal required string Version { get; init; }

    /// <summary>
    ///     Gets a human-readable label describing the application's current operational status (e.g., "Healthy", "Degraded").
    /// </summary>
    internal required string StatusInfo { get; init; }

    /// <summary>
    ///     Gets the HTTP status code indicating the application's current state.
    /// </summary>
    internal required short StatusCode { get; init; }
}