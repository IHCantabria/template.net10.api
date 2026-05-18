using System.Linq.Expressions;
using template.net10.api.Core.Interfaces;
using template.net10.api.Persistence.Models.Interfaces;

namespace template.net10.api.Core.Base;

/// <summary>
///     Provides a base implementation for entity-to-DTO projections using LINQ expressions.
/// </summary>
/// <typeparam name="TEntity">The entity type to project from.</typeparam>
/// <typeparam name="TDto">The DTO type to project to.</typeparam>
internal abstract class ProjectionBase<TEntity, TDto>(Expression<Func<TEntity, TDto>> expression)
    : IProjection<TEntity, TDto>
    where TEntity : class, IEntity where TDto : class, IDto
{
    /// <inheritdoc cref="IProjection{TEntity, TDto}.Expression" />
    public Expression<Func<TEntity, TDto>> Expression { get; } = expression;
}