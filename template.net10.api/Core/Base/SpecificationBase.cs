using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using template.net10.api.Core.Interfaces;
using template.net10.api.Persistence.Models.Interfaces;

namespace template.net10.api.Core.Base;

/// <summary>
///     Base class for entity verification specifications, providing filter and ordering capabilities.
/// </summary>
/// <typeparam name="TEntity">The entity type that this verification targets.</typeparam>
[SuppressMessage(
    "ReSharper",
    "UnusedMember.Global",
    Justification =
        "Base infrastructure class. Members are intended to be used selectively by derived verification implementations.")]
internal class VerificationBase<TEntity> : IVerification<TEntity> where TEntity : class, IEntity
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="VerificationBase{TEntity}" /> class.
    /// </summary>
    protected VerificationBase()
    {
    }

    /// <inheritdoc cref="IVerification{TEntity}.Filters" />
    public ICollection<Expression<Func<TEntity, bool>>> Filters { get; } = [];

    /// <inheritdoc cref="IVerification{TEntity}.OrderBys" />
    public ICollection<Tuple<Expression<Func<TEntity, object>>, OrderByType>> OrderBys { get; } = [];

    /// <summary>
    ///     Adds an ordering expression to the verification's order-by collection.
    /// </summary>
    /// <param name="orderByExpression">The expression that identifies the property to order by.</param>
    /// <param name="orderType">The ordering direction (ascending or descending).</param>
    [SuppressMessage(
        "ReSharper",
        "ExceptionNotDocumentedOptional",
        Justification =
            "Potential exceptions originate from underlying implementation details and are not part of the method contract.")]
    protected void AddOrderBy(Expression<Func<TEntity, object>> orderByExpression, OrderByType orderType)
    {
        OrderBys.Add(new Tuple<Expression<Func<TEntity, object>>, OrderByType>(orderByExpression, orderType));
    }

    /// <summary>
    ///     Adds a filter expression to the verification's filter collection.
    /// </summary>
    /// <param name="filterExpression">The predicate expression used to filter entities.</param>
    [SuppressMessage(
        "ReSharper",
        "ExceptionNotDocumentedOptional",
        Justification =
            "Potential exceptions originate from underlying implementation details and are not part of the method contract.")]
    protected void AddFilter(Expression<Func<TEntity, bool>> filterExpression)
    {
        Filters.Add(filterExpression);
    }
}

/// <summary>
///     Base class for entity query specifications, providing filter, ordering, include, grouping, and pagination
///     capabilities.
/// </summary>
/// <typeparam name="TEntity">The entity type that this specification targets.</typeparam>
[SuppressMessage(
    "ReSharper",
    "UnusedMember.Global",
    Justification =
        "Base infrastructure class. Members are intended to be used selectively by derived verification implementations.")]
internal class SpecificationBase<TEntity> : ISpecification<TEntity> where TEntity : class, IEntity
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="SpecificationBase{TEntity}" /> class.
    /// </summary>
    protected SpecificationBase()
    {
    }

    /// <inheritdoc cref="ISpecification{TEntity}.QuerySplitStrategy" />
    public QuerySplittingBehavior QuerySplitStrategy { get; private set; } = QuerySplittingBehavior.SingleQuery;

    /// <inheritdoc cref="ISpecification{TEntity}.QueryTrackStrategy" />
    public QueryTrackingBehavior QueryTrackStrategy { get; private set; } = QueryTrackingBehavior.TrackAll;

    /// <inheritdoc cref="ISpecification{TEntity}.Includes" />
    public ICollection<Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>>
        Includes { get; } = [];

    /// <inheritdoc cref="IVerification{TEntity}.OrderBys" />
    public ICollection<Tuple<Expression<Func<TEntity, object>>, OrderByType>> OrderBys { get; } = [];

    /// <inheritdoc cref="ISpecification{TEntity}.TakeRows" />
    public int? TakeRows { get; private set; }

    /// <inheritdoc cref="IVerification{TEntity}.Filters" />
    public ICollection<Expression<Func<TEntity, bool>>> Filters { get; } = [];

    /// <inheritdoc cref="ISpecification{TEntity}.GroupBy" />
    public Expression<Func<TEntity, object>>? GroupBy { get; private set; }

    /// <summary>
    ///     Adds a navigation property include expression to the specification for eager loading.
    /// </summary>
    /// <param name="includeExpression">The include expression for eager loading related entities.</param>
    [SuppressMessage(
        "ReSharper",
        "ExceptionNotDocumentedOptional",
        Justification =
            "Potential exceptions originate from underlying implementation details and are not part of the method contract.")]
    protected void AddInclude(Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includeExpression)
    {
        Includes.Add(includeExpression);
    }

    /// <summary>
    ///     Adds an ordering expression to the specification's order-by collection.
    /// </summary>
    /// <param name="orderByExpression">The expression that identifies the property to order by.</param>
    /// <param name="orderType">The ordering direction (ascending or descending).</param>
    [SuppressMessage(
        "ReSharper",
        "ExceptionNotDocumentedOptional",
        Justification =
            "Potential exceptions originate from underlying implementation details and are not part of the method contract.")]
    protected void AddOrderBy(Expression<Func<TEntity, object>> orderByExpression, OrderByType orderType)
    {
        OrderBys.Add(new Tuple<Expression<Func<TEntity, object>>, OrderByType>(orderByExpression, orderType));
    }

    /// <summary>
    ///     Sets the maximum number of rows to return in the query result.
    /// </summary>
    /// <param name="rows">The maximum number of rows to take.</param>
    protected void AddTakeRows(int rows)
    {
        TakeRows = rows;
    }

    /// <summary>
    ///     Adds a filter expression to the specification's filter collection.
    /// </summary>
    /// <param name="filterExpression">The predicate expression used to filter entities.</param>
    [SuppressMessage(
        "ReSharper",
        "ExceptionNotDocumentedOptional",
        Justification =
            "Potential exceptions originate from underlying implementation details and are not part of the method contract.")]
    protected void AddFilter(Expression<Func<TEntity, bool>> filterExpression)
    {
        Filters.Add(filterExpression);
    }

    /// <summary>
    ///     Applies a group-by expression to the specification.
    /// </summary>
    /// <param name="groupByExpression">The expression that defines the grouping key.</param>
    protected void ApplyGroupBy(Expression<Func<TEntity, object>> groupByExpression)
    {
        GroupBy = groupByExpression;
    }

    /// <summary>
    ///     Sets the query splitting behavior for the specification.
    /// </summary>
    /// <param name="querySplitStrategy">The query splitting behavior to use (single query or split query).</param>
    protected void SetQuerySplitStrategy(QuerySplittingBehavior querySplitStrategy)
    {
        QuerySplitStrategy = querySplitStrategy;
    }

    /// <summary>
    ///     Sets the query tracking behavior for the specification.
    /// </summary>
    /// <param name="queryTrackStrategy">The query tracking behavior to use (tracking or no-tracking).</param>
    protected void SetQueryTrackStrategy(QueryTrackingBehavior queryTrackStrategy)
    {
        QueryTrackStrategy = queryTrackStrategy;
    }
}