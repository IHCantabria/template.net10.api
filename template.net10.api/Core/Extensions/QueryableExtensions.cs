using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using HotChocolate.Resolvers;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using template.net10.api.Core.Interfaces;
using template.net10.api.Persistence.Models.Interfaces;

namespace template.net10.api.Core.Extensions;

/// <summary>
///     Provides extension methods for <see cref="IQueryable{T}" /> to apply specifications, projections, filters, and
///     query strategies.
/// </summary>
internal static class QueryableExtensions
{
    /// <summary>
    ///     Recursively rebuilds a <see cref="MemberInitExpression" /> to include only the fields present in the selected
    ///     fields tree.
    /// </summary>
    /// <param name="source">The source expression to filter.</param>
    /// <param name="selectedFieldsTree">A dictionary mapping parent paths to their selected field names.</param>
    /// <param name="path">The current dot-separated path in the expression tree.</param>
    /// <returns>A new expression containing only the selected member bindings.</returns>
    private static Expression BuildMemberInit(Expression source, Dictionary<string, HashSet<string>> selectedFieldsTree,
        string path = "")
    {
        if (source is not MemberInitExpression init) return source;

        var bindings = init.Bindings
            .OfType<MemberAssignment>()
            .Where(b => IsSelected(path, b.Member.Name, selectedFieldsTree))
            .Select(b =>
            {
                var childPath = string.IsNullOrEmpty(path) ? b.Member.Name : $"{path}.{b.Member.Name}";

                if (IsSimpleType(b.Expression.Type))
                    return Expression.Bind(b.Member, b.Expression);

                if (IsCollectionType(b.Expression.Type, out var elementType))
                    return Expression.Bind(b.Member,
                        BuildCollectionInit(b.Expression, elementType, selectedFieldsTree, childPath));

                var childInit = BuildMemberInit(b.Expression, selectedFieldsTree, childPath);
                return Expression.Bind(b.Member, childInit);
            });

        return Expression.MemberInit(Expression.New(init.Type), bindings);
    }

    /// <summary>
    ///     Builds a coalesced Select expression for a collection property, applying field selection to each element.
    /// </summary>
    /// <param name="source">The source collection expression.</param>
    /// <param name="elementType">The element type of the collection.</param>
    /// <param name="selectedFieldsTree">A dictionary mapping parent paths to their selected field names.</param>
    /// <param name="path">The current dot-separated path in the expression tree.</param>
    /// <returns>A <see cref="BinaryExpression" /> that selects filtered fields from the collection or returns a default value.</returns>
    private static BinaryExpression BuildCollectionInit(
        Expression source,
        Type elementType,
        Dictionary<string, HashSet<string>> selectedFieldsTree,
        string path)
    {
        var itemParam = Expression.Parameter(elementType, "x");
        var itemInit = BuildMemberInit(itemParam, selectedFieldsTree, path);

        var selectMethod = typeof(Queryable)
            .GetMethods()
            .First(static m => m.Name == "Select" && m.GetParameters().Length == 2)
            .MakeGenericMethod(elementType, elementType);

        var selectCall = Expression.Call(selectMethod, source, Expression.Lambda(itemInit, itemParam));

        var defaultValue = Expression.Default(source.Type);
        return Expression.Coalesce(selectCall, defaultValue);
    }

    /// <summary>
    ///     Determines whether a property name is selected at the given path in the field selection tree.
    /// </summary>
    /// <param name="path">The dot-separated parent path.</param>
    /// <param name="propName">The property name to check.</param>
    /// <param name="tree">The selected fields tree.</param>
    /// <returns><see langword="true" /> if the property is selected; otherwise, <see langword="false" />.</returns>
    private static bool IsSelected(string path, string propName, Dictionary<string, HashSet<string>> tree)
    {
        return tree.TryGetValue(path, out var props) && props.Contains(propName);
    }

    /// <summary>
    ///     Determines whether the specified type is a simple/primitive type (including common value types and strings).
    /// </summary>
    /// <param name="type">The type to check.</param>
    /// <returns>
    ///     <see langword="true" /> if the type is a primitive, enum, string, decimal, DateTime, DateTimeOffset, Guid, or
    ///     TimeSpan; otherwise, <see langword="false" />.
    /// </returns>
    private static bool IsSimpleType(Type type)
    {
        type = Nullable.GetUnderlyingType(type) ?? type;
        return type.IsPrimitive
               || type.IsEnum
               || type == typeof(string)
               || type == typeof(decimal)
               || type == typeof(DateTime)
               || type == typeof(DateTimeOffset)
               || type == typeof(Guid)
               || type == typeof(TimeSpan);
    }

    /// <summary>
    ///     Determines whether the specified type implements <see cref="IEnumerable{T}" /> and extracts the element type.
    /// </summary>
    /// <param name="type">The type to check.</param>
    /// <param name="elementType">
    ///     When this method returns, contains the generic element type if the type is a collection;
    ///     otherwise, <see langword="null" />.
    /// </param>
    /// <returns><see langword="true" /> if the type is a generic collection; otherwise, <see langword="false" />.</returns>
    private static bool IsCollectionType(Type type, [NotNullWhen(true)] out Type? elementType)
    {
        var enumerableType = type.GetInterfaces()
            .FirstOrDefault(static i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>));

        if (enumerableType != null)
        {
            elementType = enumerableType.GetGenericArguments()[0];
            return true;
        }

        elementType = null;
        return false;
    }

    extension<TEntity>(IQueryable<TEntity> queryable) where TEntity : class, IEntity
    {
        /// <summary>
        ///     Applies a collection of filter expressions to the queryable using AND logic via LinqKit's
        ///     <see cref="PredicateBuilder" />.
        /// </summary>
        /// <param name="filters">The filter expressions to apply.</param>
        /// <returns>The filtered <see cref="IQueryable{T}" />.</returns>
        private IQueryable<TEntity> ApplyFilters(IEnumerable<Expression<Func<TEntity, bool>>> filters)
        {
            var predicate = PredicateBuilder.New<TEntity>();

            predicate = filters.Aggregate(predicate, static (current, filter) => current.And(filter));

            var expandedPredicate = (Expression<Func<TEntity, bool>>)predicate.Expand();
            return queryable.Where(expandedPredicate);
        }
    }

    extension<TEntity>(IQueryable<TEntity> queryable)
    {
        /// <summary>
        ///     Applies a collection of include functions to the queryable for eager loading of related entities.
        /// </summary>
        /// <param name="includes">The include functions to apply.</param>
        /// <returns>The <see cref="IQueryable{T}" /> with includes applied.</returns>
        private IQueryable<TEntity> ApplyIncludes(IEnumerable<Func<IQueryable<TEntity>, IQueryable<TEntity>>> includes)
        {
            return includes.Aggregate(queryable, static (current, include) => include(current));
        }
    }

    extension<TEntity>(IQueryable<TEntity> queryable) where TEntity : class, IEntity
    {
        /// <summary>
        ///     Applies a collection of ordering expressions to the queryable, using OrderBy for the first and ThenBy for
        ///     subsequent ones.
        /// </summary>
        /// <param name="orderBys">The ordering expressions with their direction (ascending/descending).</param>
        /// <returns>The ordered <see cref="IQueryable{T}" />.</returns>
        private IQueryable<TEntity> ApplyOrderBys(
            IEnumerable<Tuple<Expression<Func<TEntity, object>>, OrderByType>> orderBys)
        {
            return orderBys.Aggregate(queryable,
                (current, orderBy) => current.SmartOrderBy(orderBy.Item1, orderBy.Item2, current.Equals(queryable)));
        }

        /// <summary>
        ///     Applies an OrderBy or ThenBy clause depending on whether this is the first ordering expression in the chain.
        /// </summary>
        /// <param name="expression">The property expression to order by.</param>
        /// <param name="orderType">The ordering direction (ascending or descending).</param>
        /// <param name="isFirstIteration">Whether this is the first ordering clause being applied.</param>
        /// <returns>The ordered <see cref="IQueryable{T}" />.</returns>
        private IQueryable<TEntity> SmartOrderBy(Expression<Func<TEntity, object>> expression, OrderByType orderType,
            bool isFirstIteration)
        {
            return orderType switch
            {
                OrderByType.Asc when !isFirstIteration && queryable is IOrderedQueryable<TEntity> orderedQueryable =>
                    orderedQueryable.ThenBy(expression),
                OrderByType.Desc when !isFirstIteration && queryable is IOrderedQueryable<TEntity> orderedQueryable =>
                    orderedQueryable.ThenByDescending(expression),
                OrderByType.Desc => queryable.OrderByDescending(expression),
                _ => queryable.OrderBy(expression)
            };
        }

        /// <summary>
        ///     Applies a GroupBy expression to the queryable and flattens the result using SelectMany.
        /// </summary>
        /// <param name="groupBy">The grouping expression.</param>
        /// <returns>The grouped and flattened <see cref="IQueryable{T}" />.</returns>
        private IQueryable<TEntity> ApplyGroupBy(Expression<Func<TEntity, object>> groupBy)
        {
            return queryable.GroupBy(groupBy).SelectMany(static x => x);
        }

        /// <summary>
        ///     Limits the queryable to the specified number of rows using Take.
        /// </summary>
        /// <param name="takeRows">The maximum number of rows to return.</param>
        /// <returns>The <see cref="IQueryable{T}" /> limited to <paramref name="takeRows" /> elements.</returns>
        private IQueryable<TEntity> ApplyTakeRows(int takeRows)
        {
            return queryable.Take(takeRows);
        }

        /// <summary>
        ///     Applies the specified query splitting strategy (split or single query) to the queryable.
        /// </summary>
        /// <param name="querySplitStrategy">The <see cref="QuerySplittingBehavior" /> to apply.</param>
        /// <returns>The <see cref="IQueryable{T}" /> with the query split strategy applied.</returns>
        private IQueryable<TEntity> ApplyQuerySplitStrategy(QuerySplittingBehavior querySplitStrategy)
        {
            return querySplitStrategy switch
            {
                QuerySplittingBehavior.SplitQuery => queryable.AsSplitQuery(),
                _ => queryable.AsSingleQuery()
            };
        }

        /// <summary>
        ///     Applies the specified change tracking strategy (tracking, no-tracking, or no-tracking with identity resolution) to
        ///     the queryable.
        /// </summary>
        /// <param name="queryTrackStrategy">The <see cref="QueryTrackingBehavior" /> to apply.</param>
        /// <returns>The <see cref="IQueryable{T}" /> with the tracking strategy applied.</returns>
        private IQueryable<TEntity> ApplyQueryTrackStrategy(QueryTrackingBehavior queryTrackStrategy)
        {
            return queryTrackStrategy switch
            {
                QueryTrackingBehavior.NoTracking => queryable.AsNoTracking(),
                QueryTrackingBehavior.NoTrackingWithIdentityResolution =>
                    queryable.AsNoTrackingWithIdentityResolution(),
                _ => queryable.AsTracking()
            };
        }
    }

    extension<TEntity>(IQueryable<TEntity> query) where TEntity : class, IEntity
    {
        /// <summary>
        ///     Applies a verification's filters to the queryable. Returns the original query if verification is null or has no
        ///     filters.
        /// </summary>
        /// <param name="verification">The verification containing the filter expressions, or <see langword="null" />.</param>
        /// <returns>The filtered <see cref="IQueryable{T}" />.</returns>
        internal IQueryable<TEntity> ApplyVerification(IVerification<TEntity>? verification)
        {
            // Do not apply anything if specification is null
            if (verification is null) return query;

            // Modify the IQueryable

            return verification.Filters.Count > 0 ? query.ApplyFilters(verification.Filters) : query;
        }

        /// <summary>
        ///     Applies all specification criteria (filters, includes, group by, order by, take, split strategy, and tracking) to
        ///     the queryable.
        /// </summary>
        /// <param name="specification">The specification containing all query criteria, or <see langword="null" />.</param>
        /// <returns>The fully configured <see cref="IQueryable{T}" />.</returns>
        internal IQueryable<TEntity> ApplySpecification(ISpecification<TEntity>? specification)
        {
            // Do not apply anything if specification is null
            if (specification is null) return query;

            // Modify the IQueryable

            query = specification.Filters.Count > 0 ? query.ApplyFilters(specification.Filters) : query;
            query = specification.Includes.Count > 0 ? query.ApplyIncludes(specification.Includes) : query;
            query = specification.GroupBy is not null ? query.ApplyGroupBy(specification.GroupBy) : query;
            query = specification.OrderBys.Count > 0 ? query.ApplyOrderBys(specification.OrderBys) : query;
            query = specification.TakeRows is not null ? query.ApplyTakeRows((int)specification.TakeRows) : query;
            query = query.ApplyQuerySplitStrategy(specification.QuerySplitStrategy);
            query = query.ApplyQueryTrackStrategy(specification.QueryTrackStrategy);

            return query;
        }

        /// <summary>
        ///     Projects the queryable using the specified projection expression to transform entities into DTOs.
        /// </summary>
        /// <typeparam name="TDto">The DTO type to project to.</typeparam>
        /// <param name="projection">The projection defining the select expression.</param>
        /// <returns>An <see cref="IQueryable{T}" /> of projected DTOs.</returns>
        [SuppressMessage(
            "ReSharper",
            "ExceptionNotDocumentedOptional",
            Justification =
                "Potential exceptions originate from underlying implementation details and are not part of the method contract.")]
        internal IQueryable<TDto> ApplyProjection<TDto>(IProjection<TEntity, TDto> projection) where TDto : class, IDto
        {
            return query.Select(projection.Expression);
        }

        /// <summary>
        ///     Projects the queryable using the specified projection expression, filtering projected fields based on the GraphQL
        ///     resolver context.
        /// </summary>
        /// <typeparam name="TDto">The DTO type to project to.</typeparam>
        /// <param name="projection">The projection defining the select expression.</param>
        /// <param name="context">The GraphQL resolver context used to determine which fields were requested.</param>
        /// <returns>An <see cref="IQueryable{T}" /> of projected DTOs containing only the requested fields.</returns>
        [SuppressMessage(
            "ReSharper",
            "ExceptionNotDocumentedOptional",
            Justification =
                "Potential exceptions originate from underlying implementation details and are not part of the method contract.")]
        internal IQueryable<TDto> ApplyProjection<TDto>(IProjection<TEntity, TDto> projection,
            IResolverContext context) where TDto : class, IDto
        {
            var selectedFieldsTree = context.GetSelectedFieldsTree();
            var lambdaBody = BuildMemberInit(projection.Expression.Body, selectedFieldsTree);
            var lambda = Expression.Lambda<Func<TEntity, TDto>>(lambdaBody, projection.Expression.Parameters);
            return query.Select(lambda);
        }
    }
}