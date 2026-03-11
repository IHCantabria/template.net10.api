using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Text.Json.Serialization;
using template.net10.api.Core.Interfaces;

namespace template.net10.api.Core.Geometries.Contracts;

/// <summary>
///     Represents the input contract for creating an extent (bounding box) defined by geographic WGS84 coordinates.
/// </summary>
[SuppressMessage("Design",
    "CA1515:Consider making public types internal",
    Justification =
        "Contracts must remain public to allow proper discovery and schema generation by OpenAPI.")]
public sealed partial record CreateExtentResource : IPublicApiContract,
    IEqualityOperators<CreateExtentResource, CreateExtentResource, bool>
{
    /// <summary>
    ///     Minimum longitude boundary of the extent in WGS84 coordinates.
    /// </summary>
    [JsonRequired]
    public required decimal LonMin { get; init; }

    /// <summary>
    ///     Maximum longitude boundary of the extent in WGS84 coordinates.
    /// </summary>
    [JsonRequired]
    public required decimal LonMax { get; init; }

    /// <summary>
    ///     Minimum latitude boundary of the extent in WGS84 coordinates.
    /// </summary>
    [JsonRequired]
    public required decimal LatMin { get; init; }

    /// <summary>
    ///     Maximum latitude boundary of the extent in WGS84 coordinates.
    /// </summary>
    [JsonRequired]
    public required decimal LatMax { get; init; }
}

/// <summary>
///     Represents an extent (bounding box) defined by geographic WGS84 coordinates. Used as a read response contract.
/// </summary>
[SuppressMessage("Design",
    "CA1515:Consider making public types internal",
    Justification =
        "Contracts must remain public to allow proper discovery and schema generation by OpenAPI.")]
public sealed record ExtentResource : IPublicApiContract, IEqualityOperators<ExtentResource, ExtentResource, bool>
{
    /// <summary>
    ///     Minimum longitude boundary of the extent in WGS84 coordinates.
    /// </summary>
    [JsonRequired]
    public required decimal LonMin { get; init; }

    /// <summary>
    ///     Maximum longitude boundary of the extent in WGS84 coordinates.
    /// </summary>
    [JsonRequired]
    public required decimal LonMax { get; init; }

    /// <summary>
    ///     Minimum latitude boundary of the extent in WGS84 coordinates.
    /// </summary>
    [JsonRequired]
    public required decimal LatMin { get; init; }

    /// <summary>
    ///     Maximum latitude boundary of the extent in WGS84 coordinates.
    /// </summary>
    [JsonRequired]
    public required decimal LatMax { get; init; }
}

/// <summary>
///     Represents the input contract for creating a geographic point in WGS84 coordinates.
/// </summary>
[SuppressMessage("Design",
    "CA1515:Consider making public types internal",
    Justification =
        "Contracts must remain public to allow proper discovery and schema generation by OpenAPI.")]
public sealed partial record CreatePointResource : IPublicApiContract,
    IEqualityOperators<CreatePointResource, CreatePointResource, bool>
{
    /// <summary>
    ///     Longitude of the point in WGS84 coordinates.
    /// </summary>
    [JsonRequired]
    public required decimal Lon { get; init; }

    /// <summary>
    ///     Latitude of the point in WGS84 coordinates.
    /// </summary>
    [JsonRequired]
    public required decimal Lat { get; init; }
}

/// <summary>
///     Represents a geographic point in WGS84 coordinates. Used as a read response contract.
/// </summary>
[SuppressMessage("Design",
    "CA1515:Consider making public types internal",
    Justification =
        "Contracts must remain public to allow proper discovery and schema generation by OpenAPI.")]
public sealed record PointResource : IPublicApiContract, IEqualityOperators<PointResource, PointResource, bool>
{
    /// <summary>
    ///     Unique identifier of the point.
    /// </summary>
    [JsonRequired]
    public required Guid Uuid { get; init; }

    /// <summary>
    ///     Longitude of the point in WGS84 coordinates.
    /// </summary>
    [JsonRequired]
    public decimal? Lon { get; init; }

    /// <summary>
    ///     Latitude of the point in WGS84 coordinates.
    /// </summary>
    [JsonRequired]
    public decimal? Lat { get; init; }
}