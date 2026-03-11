using System.Linq.Expressions;
using template.net10.api.Core.Base;
using template.net10.api.Core.Interfaces;
using template.net10.api.Persistence.Models.Interfaces;

namespace template.net10.api.Domain.DTOs;

/// <summary>
///     Defines a projection expression that maps an entity to a data transfer object.
/// </summary>
/// <typeparam name="TEntity">The source entity type.</typeparam>
/// <typeparam name="TDto">The target DTO type.</typeparam>
internal sealed class Projection<TEntity, TDto>(Expression<Func<TEntity, TDto>> expression)
    : ProjectionBase<TEntity, TDto>(expression)
    where TEntity : class, IEntity
    where TDto : class, IDto;