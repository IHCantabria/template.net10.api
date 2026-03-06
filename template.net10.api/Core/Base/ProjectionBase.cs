using System.Linq.Expressions;
using template.net10.api.Core.Interfaces;
using template.net10.api.Persistence.Models.Interfaces;

namespace template.net10.api.Core.Base;

/// <summary>
///     ADD DOCUMENTATION
/// </summary>
internal class ProjectionBase<TEntity, TDto> : IProjection<TEntity, TDto>
    where TEntity : class, IEntity where TDto : class, IDto
{
    /// <summary>
    ///     ADD DOCUMENTATION
    /// </summary>
    protected ProjectionBase(Expression<Func<TEntity, TDto>> expression)
    {
        Expression = expression;
    }

    /// <inheritdoc cref="IProjection{TEntity, TDto}.Expression" />
    public Expression<Func<TEntity, TDto>> Expression { get; }
}