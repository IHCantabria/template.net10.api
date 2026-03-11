using HotChocolate.Data.Sorting;
using JetBrains.Annotations;
using NetTopologySuite.Geometries;

namespace template.net10.api.GraphQL.Types;

/// <summary>
///     HotChocolate sort input type for <see cref="Point"/> geometry, excluding non-sortable navigation properties.
/// </summary>
[UsedImplicitly]
internal class PointSortInputType : SortInputType<Point>
{
    /// <summary>
    ///     Configures the sort input type by ignoring non-sortable properties of <see cref="Point"/>.
    /// </summary>
    /// <param name="descriptor">The sort input type descriptor to configure.</param>
    /// <exception cref="ArgumentNullException"><paramref name="descriptor" /> is <see langword="null" />.</exception>
    protected override void Configure(ISortInputTypeDescriptor<Point> descriptor)
    {
        ArgumentNullException.ThrowIfNull(descriptor);
        descriptor.Ignore(static f => f.Boundary);
        descriptor.Ignore(static f => f.Envelope);
        descriptor.Ignore(static f => f.Factory);
    }
}

/// <summary>
///     HotChocolate sort input type for <see cref="Polygon"/> geometry, excluding non-sortable navigation properties.
/// </summary>
[UsedImplicitly]
internal class PolygonSortInputType : SortInputType<Polygon>
{
    /// <summary>
    ///     Configures the sort input type by ignoring non-sortable properties of <see cref="Polygon"/>.
    /// </summary>
    /// <param name="descriptor">The sort input type descriptor to configure.</param>
    /// <exception cref="ArgumentNullException"><paramref name="descriptor" /> is <see langword="null" />.</exception>
    protected override void Configure(ISortInputTypeDescriptor<Polygon> descriptor)
    {
        ArgumentNullException.ThrowIfNull(descriptor);
        descriptor.Ignore(static f => f.Boundary);
        descriptor.Ignore(static f => f.Envelope);
        descriptor.Ignore(static f => f.ExteriorRing);
        descriptor.Ignore(static f => f.Factory);
        descriptor.Ignore(static f => f.Shell);
    }
}