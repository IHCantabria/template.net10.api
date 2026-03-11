using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using template.net10.api.Core.Interfaces;

namespace template.net10.api.Core.Geometries.DTOs;

/// <summary>
///     Represents an internal extent (bounding box) data transfer object with WGS84 coordinates.
/// </summary>
internal sealed partial record ExtentDto : IDto, IEqualityOperators<ExtentDto, ExtentDto, bool>
{
    /// <summary>
    ///     Longitude Min WGS84
    /// </summary>
    internal required decimal LonMin { get; init; }

    /// <summary>
    ///     Longitude Max WGS84
    /// </summary>
    internal required decimal LonMax { get; init; }

    /// <summary>
    ///     Latitude Min WGS84
    /// </summary>
    internal required decimal LatMin { get; init; }

    /// <summary>
    ///     Latitude Max WGS84
    /// </summary>
    internal required decimal LatMax { get; init; }
}

/// <summary>
///     Represents a data transfer object for creating an extent (bounding box) in WGS84 coordinates.
/// </summary>
[SuppressMessage("Design",
    "CA1515:Consider making public types internal",
    Justification =
        "Public visibility is required because the type is exposed through a public conversion operator.")]
public sealed record CreateExtentDto : IDto, IEqualityOperators<CreateExtentDto, CreateExtentDto, bool>
{
    /// <summary>
    ///     Longitude Min WGS84
    /// </summary>
    public required decimal LonMin { get; init; }

    /// <summary>
    ///     Longitude Max WGS84
    /// </summary>
    public required decimal LonMax { get; init; }

    /// <summary>
    ///     Latitude Min WGS84
    /// </summary>
    public required decimal LatMin { get; init; }

    /// <summary>
    ///     Latitude Max WGS84
    /// </summary>

    public required decimal LatMax { get; init; }

    /// <summary>
    ///     Validates that the extent coordinates are within valid WGS84 ranges and that min values are less than max values.
    /// </summary>
    /// <returns><see langword="true"/> if the extent is valid; otherwise, <see langword="false"/>.</returns>
    internal bool IsValid()
    {
        return LonMin >= -180 && LonMax <= 180 && LatMin >= -90 && LatMax <= 90 && LonMin < LonMax &&
               LatMin < LatMax;
    }
}

/// <summary>
///     Represents an internal point data transfer object with WGS84 coordinates.
/// </summary>
internal sealed partial record PointDto : IDto, IEqualityOperators<PointDto, PointDto, bool>
{
    /// <summary>
    ///     Unique identifier of the point.
    /// </summary>
    internal required Guid Uuid { get; init; }

    /// <summary>
    ///     Longitude WGS84
    /// </summary>
    internal decimal? Lon { get; init; }

    /// <summary>
    ///     Latitude WGS84
    /// </summary>
    internal decimal? Lat { get; init; }
}

/// <summary>
///     Represents a data transfer object for creating a geographic point in WGS84 coordinates.
/// </summary>
[SuppressMessage("Design",
    "CA1515:Consider making public types internal",
    Justification =
        "Public visibility is required because the type is exposed through a public conversion operator.")]
public sealed record CreatePointDto : IDto, IEqualityOperators<CreatePointDto, CreatePointDto, bool>
{
    /// <summary>
    ///     Longitude WGS84
    /// </summary>
    public required decimal Lon { get; init; }

    /// <summary>
    ///     Latitude WGS84
    /// </summary>
    public required decimal Lat { get; init; }

    /// <summary>
    ///     Validates that the point coordinates are within valid WGS84 ranges.
    /// </summary>
    /// <returns><see langword="true"/> if the point is valid; otherwise, <see langword="false"/>.</returns>
    internal bool IsValid()
    {
        return Lon is >= -180 and <= 180 && Lat is >= -90 and <= 90;
    }
}