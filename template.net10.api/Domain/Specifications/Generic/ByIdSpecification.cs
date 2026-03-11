using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using template.net10.api.Core.Base;
using template.net10.api.Core.Interfaces;
using template.net10.api.Persistence.Models.Interfaces;

namespace template.net10.api.Domain.Specifications.Generic;

/// <summary>
///     Specification that retrieves a single entity by its primary key identifier.
/// </summary>
/// <typeparam name="TEntity">The entity type that implements <see cref="IEntityWithId{TKey}" />.</typeparam>
/// <typeparam name="TKey">The type of the entity's primary key.</typeparam>
[SuppressMessage(
    "ReSharper",
    "UnusedType.Global",
    Justification =
        "Generic specification type intended for reuse in repository queries; usage may be indirect or consumer-dependent.")]
internal sealed class EntityByIdSpecification<TEntity, TKey> : SpecificationBase<TEntity>
    where TEntity : class, IEntityWithId<TKey>
    where TKey : struct
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="EntityByIdSpecification{TEntity, TKey}" /> class filtering by the
    ///     specified entity identifier.
    /// </summary>
    /// <param name="entityId">The primary key value to filter by.</param>
    /// <param name="trackData">
    ///     When <see langword="true" />, entities are tracked by the context; otherwise no-tracking is
    ///     used.
    /// </param>
    internal EntityByIdSpecification(TKey entityId, bool trackData = false)
    {
        AddFilter(e => e.Id.Equals(entityId));
        AddOrderBy(static e => e.Id, OrderByType.Asc);
        SetQueryTrackStrategy(trackData ? QueryTrackingBehavior.TrackAll : QueryTrackingBehavior.NoTracking);
        SetQuerySplitStrategy(QuerySplittingBehavior.SingleQuery);
    }
}

/// <summary>
///     Specification that retrieves multiple entities by a collection of primary key identifiers.
/// </summary>
/// <typeparam name="TEntity">The entity type that implements <see cref="IEntityWithId{TKey}" />.</typeparam>
/// <typeparam name="TKey">The type of the entity's primary key.</typeparam>
[SuppressMessage(
    "ReSharper",
    "UnusedType.Global",
    Justification =
        "Generic specification type intended for reuse in repository queries; usage may be indirect or consumer-dependent.")]
internal sealed class EntitiesByIdsSpecification<TEntity, TKey> : SpecificationBase<TEntity>
    where TEntity : class, IEntityWithId<TKey>
    where TKey : struct
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="EntitiesByIdsSpecification{TEntity, TKey}" /> class, optionally
    ///     filtering by the specified identifiers.
    /// </summary>
    /// <param name="entityIds">
    ///     An optional collection of primary key values to filter by. When <see langword="null" />, no ID
    ///     filter is applied.
    /// </param>
    /// <param name="trackData">
    ///     When <see langword="true" />, entities are tracked by the context; otherwise no-tracking is
    ///     used.
    /// </param>
    [SuppressMessage(
        "ReSharper",
        "ExceptionNotDocumentedOptional",
        Justification =
            "Potential exceptions originate from underlying implementation details and are not part of the method contract.")]
    internal EntitiesByIdsSpecification(IEnumerable<TKey>? entityIds = null,
        bool trackData = false)
    {
        if (entityIds != null)
        {
            var enumerable = entityIds.ToList();
            AddFilter(e => enumerable.Contains(e.Id));
        }

        AddOrderBy(static e => e.Id, OrderByType.Asc);
        SetQueryTrackStrategy(trackData ? QueryTrackingBehavior.TrackAll : QueryTrackingBehavior.NoTracking);
        SetQuerySplitStrategy(QuerySplittingBehavior.SingleQuery);
    }
}