using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using template.net10.api.Persistence.Models.Interfaces;

namespace template.net10.api.Core.Interfaces;

/// <summary>
///     Specifies the direction of an ordering operation.
/// </summary>
[SuppressMessage("Design",
    "CA1515:Consider making public types internal",
    Justification =
        "The enum is part of the public API contract and must remain publicly accessible.")]
public enum OrderByType
{
    /// <summary>
    ///     Ascending Order
    /// </summary>
    Asc = 0,

    /// <summary>
    ///     Descending Order
    /// </summary>
    Desc = 1
}

/// <summary>
///     Defines filter and ordering criteria for querying entities.
/// </summary>
/// <typeparam name="TEntity">The entity type the verification applies to.</typeparam>
[SuppressMessage("Design",
    "CA1515:Consider making public types internal",
    Justification =
        "The interface is part of the public API contract and must remain publicly accessible.")]
internal interface IVerification<TEntity> where TEntity : class, IEntity
{
    /// <summary>
    ///     Gets the collection of filter expressions to apply to the query.
    /// </summary>
    ICollection<Expression<Func<TEntity, bool>>> Filters { get; }

    /// <summary>
    ///     Gets the collection of ordering expressions with their sort direction.
    /// </summary>
    ICollection<Tuple<Expression<Func<TEntity, object>>, OrderByType>> OrderBys { get; }
}

/// <summary>
///     Extends <see cref="IVerification{TEntity}" /> with include, grouping, paging, and query behavior options for
///     building complex entity queries.
/// </summary>
/// <typeparam name="TEntity">The entity type the specification applies to.</typeparam>
[SuppressMessage("Design",
    "CA1515:Consider making public types internal",
    Justification =
        "The interface is part of the public API contract and must remain publicly accessible.")]
internal interface ISpecification<TEntity> : IVerification<TEntity> where TEntity : class, IEntity
{
    /// <summary>
    ///     Gets the collection of include expressions for eager loading related entities.
    /// </summary>
    ICollection<Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>> Includes { get; }

    /// <summary>
    ///     Gets the expression used to group query results, or <see langword="null" /> if no grouping is applied.
    /// </summary>
    Expression<Func<TEntity, object>>? GroupBy { get; }

    /// <summary>
    ///     Gets the maximum number of rows to return, or <see langword="null" /> for no limit.
    /// </summary>
    int? TakeRows { get; }

    /// <summary>
    ///     Gets the query splitting behavior strategy for the query.
    /// </summary>
    QuerySplittingBehavior QuerySplitStrategy { get; }

    /// <summary>
    ///     Gets the change tracking behavior strategy for the query.
    /// </summary>
    QueryTrackingBehavior QueryTrackStrategy { get; }
}