using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using template.net10.api.Core.Base;
using template.net10.api.Core.Interfaces;
using template.net10.api.Persistence.Models.Interfaces;

namespace template.net10.api.Domain.Specifications.Generic;

/// <summary>
///     Specification that retrieves entities ordered ascending by their alias text.
/// </summary>
/// <typeparam name="TEntity">The entity type that implements <see cref="IEntityWithAlias"/>.</typeparam>
[SuppressMessage(
    "ReSharper",
    "UnusedType.Global",
    Justification =
        "Generic specification type intended for reuse in repository queries; usage may be indirect or consumer-dependent.")]
internal sealed class DtosOrderedByAliasSpecifications<TEntity> : SpecificationBase<TEntity>
    where TEntity : class, IEntityWithAlias
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="DtosOrderedByAliasSpecifications{TEntity}"/> class with ascending alias order.
    /// </summary>
    /// <param name="trackData">When <see langword="true"/>, entities are tracked by the context; otherwise no-tracking is used.</param>
    internal DtosOrderedByAliasSpecifications(bool trackData = false)
    {
        AddOrderBy(static e => e.AliasText, OrderByType.Asc);
        SetQueryTrackStrategy(trackData ? QueryTrackingBehavior.TrackAll : QueryTrackingBehavior.NoTracking);
        SetQuerySplitStrategy(QuerySplittingBehavior.SingleQuery);
    }
}