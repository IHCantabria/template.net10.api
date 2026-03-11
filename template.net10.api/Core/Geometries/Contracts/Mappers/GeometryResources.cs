using template.net10.api.Core.Geometries.DTOs;

namespace template.net10.api.Core.Geometries.Contracts;

public sealed partial record CreateExtentResource
{
    /// <summary>
    ///     Implicitly converts a <see cref="CreateExtentResource"/> to a <see cref="CreateExtentDto"/>.
    /// </summary>
    /// <param name="resource">The resource to convert.</param>
    /// <returns>A new <see cref="CreateExtentDto"/> with the mapped coordinate values.</returns>
    public static implicit operator CreateExtentDto(CreateExtentResource resource)
    {
        ArgumentNullException.ThrowIfNull(resource);
        return new CreateExtentDto
        {
            LatMin = resource.LatMin,
            LatMax = resource.LatMax,
            LonMin = resource.LonMin,
            LonMax = resource.LonMax
        };
    }

    /// <summary>
    ///     Explicitly converts a <see cref="CreateExtentResource"/> to a <see cref="CreateExtentDto"/>.
    /// </summary>
    /// <param name="resource">The resource to convert.</param>
    /// <returns>A new <see cref="CreateExtentDto"/> with the mapped coordinate values.</returns>
    public static CreateExtentDto ToCreateExtentDto(
        CreateExtentResource resource)
    {
        return resource;
    }

    /// <summary>
    ///     Converts a collection of <see cref="CreateExtentResource"/> instances to <see cref="CreateExtentDto"/> instances.
    /// </summary>
    /// <param name="resources">The read-only list of resources to convert.</param>
    /// <returns>An enumerable of <see cref="CreateExtentDto"/> instances.</returns>
    internal static IEnumerable<CreateExtentDto> ToCollection(
        IReadOnlyList<CreateExtentResource> resources)
    {
        var dtos = new CreateExtentDto[resources.Count];
        for (var i = 0; i < resources.Count; i++) dtos[i] = resources[i];
        return dtos;
    }
}

public sealed partial record CreatePointResource
{
    /// <summary>
    ///     Implicitly converts a <see cref="CreatePointResource"/> to a <see cref="CreatePointDto"/>.
    /// </summary>
    /// <param name="resource">The resource to convert.</param>
    /// <returns>A new <see cref="CreatePointDto"/> with the mapped coordinate values.</returns>
    public static implicit operator CreatePointDto(CreatePointResource resource)
    {
        ArgumentNullException.ThrowIfNull(resource);
        return new CreatePointDto
        {
            Lat = resource.Lat, Lon = resource.Lon
        };
    }

    /// <summary>
    ///     Explicitly converts a <see cref="CreatePointResource"/> to a <see cref="CreatePointDto"/>.
    /// </summary>
    /// <param name="resource">The resource to convert.</param>
    /// <returns>A new <see cref="CreatePointDto"/> with the mapped coordinate values.</returns>
    public static CreatePointDto ToCreatePointDto(
        CreatePointResource resource)
    {
        return resource;
    }

    /// <summary>
    ///     Converts a collection of <see cref="CreatePointResource"/> instances to <see cref="CreatePointDto"/> instances.
    /// </summary>
    /// <param name="resources">The read-only list of resources to convert.</param>
    /// <returns>An enumerable of <see cref="CreatePointDto"/> instances.</returns>
    internal static IEnumerable<CreatePointDto> ToCollection(
        IReadOnlyList<CreatePointResource> resources)
    {
        var dtos = new CreatePointDto[resources.Count];
        for (var i = 0; i < resources.Count; i++) dtos[i] = resources[i];
        return dtos;
    }
}