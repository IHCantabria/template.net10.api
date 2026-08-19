using System.Numerics;
using template.net10.api.Core.Interfaces;

namespace template.net10.api.Core.DTOs;

/// <summary>
///     Data transfer object containing application information details.
/// </summary>
internal sealed partial record InfoDto : IDto, IEqualityOperators<InfoDto, InfoDto, bool>
{
    /// <summary>
    ///     Gets the HTTP status code indicating the application's current state.
    /// </summary>
    internal required short Status { get; init; }

    /// <summary>
    ///     Gets a human-readable label describing the application's current operational status (e.g., "OK", "Degraded").
    /// </summary>
    internal required string StatusInfo { get; init; }

    /// <summary>
    ///     Gets the application version string, normalized without the leading <c>v</c> prefix.
    /// </summary>
    internal required string Version { get; init; }

    /// <summary>
    ///     Gets the deployment environment name in upper case (e.g., <c>LOCAL</c>, <c>DEV</c>, <c>PRE</c>, <c>PROD</c>).
    /// </summary>
    internal required string Environment { get; init; }
}
