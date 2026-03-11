using template.net10.api.Core.Geometries.Contracts;

namespace template.net10.api.Core.Geometries.DTOs;

internal sealed partial record ExtentDto
{
    /// <summary>
    ///     Implicitly converts an <see cref="ExtentDto"/> to an <see cref="ExtentResource"/>.
    /// </summary>
    /// <param name="dto">The DTO to convert.</param>
    /// <returns>A new <see cref="ExtentResource"/> with the mapped coordinate values.</returns>
    public static implicit operator ExtentResource(ExtentDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        return new ExtentResource
        {
            LonMax = dto.LonMax,
            LatMax = dto.LatMax,
            LonMin = dto.LonMin,
            LatMin = dto.LatMin
        };
    }
}

internal sealed partial record PointDto
{
    /// <summary>
    ///     Implicitly converts a <see cref="PointDto"/> to a <see cref="PointResource"/>.
    /// </summary>
    /// <param name="dto">The DTO to convert.</param>
    /// <returns>A new <see cref="PointResource"/> with the mapped values.</returns>
    public static implicit operator PointResource(PointDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        return new PointResource
        {
            Uuid = dto.Uuid,
            Lon = dto.Lon,
            Lat = dto.Lat
        };
    }
}