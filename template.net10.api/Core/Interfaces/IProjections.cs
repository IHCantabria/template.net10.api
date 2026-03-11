using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using template.net10.api.Persistence.Models.Interfaces;

namespace template.net10.api.Core.Interfaces;

/// <summary>
///     Defines a projection expression that maps an entity to a DTO for query-time selection.
/// </summary>
/// <typeparam name="TEntity">The entity type to project from.</typeparam>
/// <typeparam name="TDto">The DTO type to project to.</typeparam>
[SuppressMessage("Design",
    "CA1515:Consider making public types internal",
    Justification =
        "The interface is part of the public API contract and must remain publicly accessible.")]
internal interface IProjection<TEntity, TDto> where TEntity : class, IEntity where TDto : class, IDto
{
    /// <summary>
    ///     Gets the projection expression that maps <typeparamref name="TEntity"/> to <typeparamref name="TDto"/>.
    /// </summary>
    Expression<Func<TEntity, TDto>> Expression { get; }
}