using System.Linq.Expressions;
using template.net10.api.Core.Interfaces;
using template.net10.api.Persistence.Models.Interfaces;

namespace template.net10.api.Core.Base;

/// <summary>
///     Provides a base implementation for entity-to-DTO projections using LINQ expressions.
/// </summary>
/// <typeparam name="TEntity">The entity type to project from.</typeparam>
/// <typeparam name="TDto">The DTO type to project to.</typeparam>
internal class ProjectionBase<TEntity, TDto> : IProjection<TEntity, TDto>
    where TEntity : class, IEntity where TDto : class, IDto
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ProjectionBase{TEntity, TDto}"/> class with the specified projection expression.
    /// </summary>
    /// <param name="expression">The LINQ expression that defines the projection from entity to DTO.</param>
    protected ProjectionBase(Expression<Func<TEntity, TDto>> expression)
    {
        Expression = expression;
    }

    /// <inheritdoc cref="IProjection{TEntity, TDto}.Expression" />
    public Expression<Func<TEntity, TDto>> Expression { get; }
}