using System.Diagnostics.CodeAnalysis;
using LanguageExt;
using NetTopologySuite.Geometries;
using template.net10.api.Core.Geometries.DTOs;

namespace template.net10.api.Core.Geometries;

/// <summary>
///     Provides utility methods for creating and converting geometry objects to and from DTOs.
/// </summary>
[SuppressMessage(
    "ReSharper",
    "UnusedType.Global",
    Justification = "General-purpose helper type; usage depends on consumer requirements.")]
[SuppressMessage(
    "ReSharper",
    "UnusedMember.Global",
    Justification = "General-purpose helper methods; not all members are used in every scenario.")]
internal static class GeometryUtils
{
    /// <summary>
    ///     Creates a polygon geometry representing an extent (bounding box) from the given <see cref="CreateExtentDto" />.
    /// </summary>
    /// <param name="extent">The extent DTO containing WGS84 coordinate boundaries.</param>
    /// <returns>A <see cref="Try{A}" /> containing the resulting <see cref="Geometry" /> polygon with SRID 4326.</returns>
    /// <exception cref="ArgumentException">If the ring is not closed, or has too few points</exception>
    internal static Try<Geometry> CreateExtentFromExtentDto(CreateExtentDto extent)
    {
        return () =>
        {
            // Check that the input array has exactly four coordinates
            if (!extent.IsValid())
                return new LanguageExt.Common.Result<Geometry>(
                    new GeometryExtentNotValidException("Extent is not valid"));

            var linearRing = new LinearRing([
                new Coordinate((double)extent.LonMin, (double)extent.LatMin),
                new Coordinate((double)extent.LonMax, (double)extent.LatMin),
                new Coordinate((double)extent.LonMax, (double)extent.LatMax),
                new Coordinate((double)extent.LonMin, (double)extent.LatMax),
                new Coordinate((double)extent.LonMin, (double)extent.LatMin)
            ]);
            return new Polygon(linearRing) { SRID = 4326 };
        };
    }

    /// <summary>
    ///     Creates a point geometry from the given <see cref="CreatePointDto" />.
    /// </summary>
    /// <param name="point">The point DTO containing WGS84 coordinates.</param>
    /// <returns>A <see cref="Try{A}" /> containing the resulting <see cref="Geometry" /> point with SRID 4326.</returns>
    internal static Try<Geometry> CreatePointFromPointDto(CreatePointDto point)
    {
        return () =>
        {
            // Check that the input array has exactly four coordinates
            if (!point.IsValid())
                return new LanguageExt.Common.Result<Geometry>(
                    new GeometryPointNotValidException("Point is not valid"));

            return new Point(
                new Coordinate((double)point.Lon, (double)point.Lat)
            ) { SRID = 4326 };
        };
    }

    /// <summary>
    ///     Extracts extent coordinate boundaries from a polygon <see cref="Geometry" /> and returns them as an
    ///     <see cref="ExtentDto" />.
    /// </summary>
    /// <param name="geometry">The source geometry, expected to be a valid polygon.</param>
    /// <returns>A <see cref="Try{A}" /> containing the resulting <see cref="ExtentDto" />.</returns>
    [SuppressMessage(
        "ReSharper",
        "ExceptionNotDocumentedOptional",
        Justification =
            "Potential exceptions originate from underlying implementation details and are not part of the method contract.")]
    internal static Try<ExtentDto> CreateExtentDtoFromGeometry(Geometry geometry)
    {
        return () =>
        {
            if (geometry?.IsEmpty != false)
                return new LanguageExt.Common.Result<ExtentDto>(
                    new GeometryPointNotValidException("Input geometry is null or empty."));

            // Extract the Envelope from the Geometry
            var envelope = geometry.Envelope;

            Coordinate[] coordinates = envelope.Coordinates;

            // Check if the geometry is a polygon and if it has enough coordinates
            return geometry is Polygon && coordinates.Length >= 4
                ? new ExtentDto
                {
                    LonMin = new decimal(coordinates[0].X),
                    LatMin = new decimal(coordinates[0].Y),
                    LonMax = new decimal(coordinates[2].X),
                    LatMax = new decimal(coordinates[2].Y)
                }
                : new LanguageExt.Common.Result<ExtentDto>(
                    new GeometryExtentNotValidException("Input geometry is not a valid extent"));
        };
    }

    /// <summary>
    ///     Extracts point coordinates from a point <see cref="Geometry" /> and returns them as a <see cref="PointDto" />.
    /// </summary>
    /// <param name="geometry">The source geometry, expected to be a valid point.</param>
    /// <param name="uuid">The unique identifier to assign to the resulting DTO.</param>
    /// <returns>A <see cref="Try{A}" /> containing the resulting <see cref="PointDto" />.</returns>
    [SuppressMessage(
        "ReSharper",
        "ExceptionNotDocumentedOptional",
        Justification =
            "Potential exceptions originate from underlying implementation details and are not part of the method contract.")]
    internal static Try<PointDto> CreatePointDtoFromGeometry(Geometry geometry, Guid uuid)
    {
        return () =>
        {
            if (geometry?.IsEmpty != false)
                return new LanguageExt.Common.Result<PointDto>(
                    new GeometryPointNotValidException("Input geometry is null or empty."));

            // Extract the Envelope from the Geometry
            var envelope = geometry.Envelope;

            Coordinate[] coordinates = envelope.Coordinates;

            // Check if the geometry is a polygon and if it has enough coordinates
            return geometry is Point && coordinates.Length == 1
                ? new PointDto
                {
                    Uuid = uuid,
                    Lon = new decimal(coordinates[0].X),
                    Lat = new decimal(coordinates[0].Y)
                }
                : new LanguageExt.Common.Result<PointDto>(
                    new GeometryPointNotValidException("Input geometry is not a valid point"));
        };
    }
}