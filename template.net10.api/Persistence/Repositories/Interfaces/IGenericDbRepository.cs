using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using LanguageExt;
using Microsoft.EntityFrameworkCore;
using template.net10.api.Core.Interfaces;
using template.net10.api.Persistence.Models.Interfaces;

namespace template.net10.api.Persistence.Repositories.Interfaces;

/// <summary>
///     Defines write-context (tracked) operations for a generic EF Core repository scoped to a <typeparamref name="TDbContext"/>.
/// </summary>
[SuppressMessage(
    "ReSharper",
    "UnusedMember.Global",
    Justification =
        "Repository interface methods are consumed indirectly through dependency injection and implementations.")]
internal interface IGenericDbRepositoryWriteContext<out TDbContext, TEntity>
    where TDbContext : DbContext where TEntity : class, IEntity
{
    /// <summary>
    ///     Returns the tracked <typeparamref name="TDbContext"/> instance used by this write context.
    /// </summary>
    /// <returns>The underlying tracked context.</returns>
    [MustDisposeResource]
    TDbContext DbContext();

    /// <summary>
    ///     Synchronously verifies whether any entity satisfies the given verification predicate.
    /// </summary>
    /// <param name="verification">The verification to apply, or <see langword="null"/> to check all entities.</param>
    /// <returns>A <see cref="Try{A}"/> wrapping <see langword="true"/> if any entity matches; otherwise <see langword="false"/>.</returns>
    Try<bool> Verificate(IVerification<TEntity>? verification);

    /// <summary>
    ///     Asynchronously verifies whether any entity satisfies the given verification predicate.
    /// </summary>
    /// <param name="verification">The verification to apply, or <see langword="null"/> to check all entities.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A <see cref="LanguageExt.Common.Result{A}"/> wrapping <see langword="true"/> if any entity matches; otherwise <see langword="false"/>.</returns>
    Task<LanguageExt.Common.Result<bool>> VerificateAsync(IVerification<TEntity>? verification,
        CancellationToken cancellationToken);

    /// <summary>
    ///     Asynchronously verifies that exactly one entity satisfies the given verification predicate.
    /// </summary>
    /// <param name="verification">The verification to apply, or <see langword="null"/> to check all entities.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A <see cref="LanguageExt.Common.Result{A}"/> wrapping <see langword="true"/> when exactly one entity matches; otherwise <see langword="false"/>.</returns>
    Task<LanguageExt.Common.Result<bool>> VerificateSingleAsync(IVerification<TEntity>? verification,
        CancellationToken cancellationToken);

    /// <summary>
    ///     Asynchronously retrieves all entities satisfying the specification from the tracked context.
    /// </summary>
    /// <param name="specification">The query specification, or <see langword="null"/> to retrieve all entities.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A <see cref="LanguageExt.Common.Result{A}"/> wrapping the matched entity collection.</returns>
    Task<LanguageExt.Common.Result<ICollection<TEntity>>> GetAsync(ISpecification<TEntity>? specification,
        CancellationToken cancellationToken);

    /// <summary>
    ///     Asynchronously retrieves entities satisfying the specification and projects them server-side to <typeparamref name="TDto"/>.
    /// </summary>
    /// <typeparam name="TDto">The target DTO type.</typeparam>
    /// <param name="specification">The query specification, or <see langword="null"/> to retrieve all entities.</param>
    /// <param name="projection">The EF Core server-side projection to apply.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A <see cref="LanguageExt.Common.Result{A}"/> wrapping the projected sequence.</returns>
    Task<LanguageExt.Common.Result<IEnumerable<TDto>>> GetAsync<TDto>(ISpecification<TEntity>? specification,
        IProjection<TEntity, TDto> projection,
        CancellationToken cancellationToken)
        where TDto : class, IDto;

    /// <summary>
    ///     Asynchronously retrieves the single entity matching the specification. Fails if zero or more than one is found.
    /// </summary>
    /// <param name="specification">The query specification.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A <see cref="LanguageExt.Common.Result{A}"/> wrapping the matched entity.</returns>
    Task<LanguageExt.Common.Result<TEntity>> GetSingleAsync(ISpecification<TEntity> specification,
        CancellationToken cancellationToken);

    /// <summary>
    ///     Asynchronously retrieves the single entity matching the specification and projects it to <typeparamref name="TDto"/>.
    /// </summary>
    /// <typeparam name="TDto">The target DTO type.</typeparam>
    /// <param name="specification">The query specification.</param>
    /// <param name="projection">The EF Core server-side projection to apply.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A <see cref="LanguageExt.Common.Result{A}"/> wrapping the projected DTO.</returns>
    Task<LanguageExt.Common.Result<TDto>> GetSingleAsync<TDto>(ISpecification<TEntity> specification,
        IProjection<TEntity, TDto> projection,
        CancellationToken cancellationToken)
        where TDto : class, IDto;

    /// <summary>
    ///     Asynchronously retrieves the first entity that matches the specification.
    /// </summary>
    /// <param name="specification">The query specification.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A <see cref="LanguageExt.Common.Result{A}"/> wrapping the first matching entity.</returns>
    Task<LanguageExt.Common.Result<TEntity>> GetFirstAsync(ISpecification<TEntity> specification,
        CancellationToken cancellationToken);

    /// <summary>
    ///     Asynchronously retrieves the first entity matching the specification and projects it to <typeparamref name="TDto"/>.
    /// </summary>
    /// <typeparam name="TDto">The target DTO type.</typeparam>
    /// <param name="specification">The query specification.</param>
    /// <param name="projection">The EF Core server-side projection to apply.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A <see cref="LanguageExt.Common.Result{A}"/> wrapping the first projected DTO.</returns>
    Task<LanguageExt.Common.Result<TDto>> GetFirstAsync<TDto>(ISpecification<TEntity> specification,
        IProjection<TEntity, TDto> projection,
        CancellationToken cancellationToken)
        where TDto : class, IDto;

    /// <summary>
    ///     Synchronously retrieves the first entity matching the specification and projects it to <typeparamref name="TDto"/>.
    /// </summary>
    /// <typeparam name="TDto">The target DTO type.</typeparam>
    /// <param name="specification">The query specification.</param>
    /// <param name="projection">The EF Core server-side projection to apply.</param>
    /// <returns>A <see cref="Try{A}"/> wrapping the first projected DTO.</returns>
    Try<TDto> GetFirst<TDto>(ISpecification<TEntity> specification, IProjection<TEntity, TDto> projection)
        where TDto : class, IDto;

    /// <summary>
    ///     Asynchronously inserts a new entity into the context.
    /// </summary>
    /// <param name="entity">The entity to insert.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A <see cref="LanguageExt.Common.Result{A}"/> wrapping the inserted entity.</returns>
    Task<LanguageExt.Common.Result<TEntity>> InsertAsync(TEntity entity, CancellationToken cancellationToken);

    /// <summary>
    ///     Asynchronously inserts a collection of entities into the context.
    /// </summary>
    /// <param name="entities">The entities to insert.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A <see cref="LanguageExt.Common.Result{A}"/> wrapping <see langword="true"/> when the bulk insert succeeds.</returns>
    Task<LanguageExt.Common.Result<bool>> InsertBulkAsync(IEnumerable<TEntity> entities,
        CancellationToken cancellationToken);

    /// <summary>
    ///     Marks the entity for deletion in the tracked context. Changes are persisted on <c>SaveChanges</c>.
    /// </summary>
    /// <param name="entity">The entity to delete.</param>
    /// <returns>A <see cref="Try{A}"/> wrapping the deleted entity.</returns>
    Try<TEntity> Delete(TEntity entity);

    /// <summary>
    ///     Asynchronously finds an entity by its primary key and marks it for deletion.
    /// </summary>
    /// <typeparam name="TKey">The type of the primary key.</typeparam>
    /// <param name="entityKey">The primary key value, or <see langword="null"/>.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A <see cref="LanguageExt.Common.Result{A}"/> wrapping the deleted entity.</returns>
    Task<LanguageExt.Common.Result<TEntity>> DeleteAsync<TKey>(TKey? entityKey, CancellationToken cancellationToken);

    /// <summary>
    ///     Marks the entity as modified in the tracked context. Changes are persisted on <c>SaveChanges</c>.
    /// </summary>
    /// <param name="entity">The entity to update.</param>
    /// <returns>A <see cref="Try{A}"/> wrapping the updated entity.</returns>
    Try<TEntity> Update(TEntity entity);

    /// <summary>
    ///     Asynchronously executes a non-query stored procedure on the tracked context.
    /// </summary>
    /// <param name="procedureCall">The stored procedure descriptor and its parameters.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A <see cref="LanguageExt.Common.Result{A}"/> wrapping <see langword="true"/> when the procedure executes successfully.</returns>
    Task<LanguageExt.Common.Result<bool>> ExecuteProcedureAsync(IProcedureCall procedureCall,
        CancellationToken cancellationToken);

    /// <summary>
    ///     Asynchronously executes a stored procedure that returns a result set and applies an optional specification.
    /// </summary>
    /// <param name="procedureCall">The stored procedure descriptor and its parameters.</param>
    /// <param name="specification">An optional specification to filter or order the result set.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A <see cref="LanguageExt.Common.Result{A}"/> wrapping the matched entity collection.</returns>
    Task<LanguageExt.Common.Result<ICollection<TEntity>>> ExecuteQueryProcedureAsync(IProcedureCall procedureCall,
        ISpecification<TEntity>? specification, CancellationToken cancellationToken);

    /// <summary>
    ///     Asynchronously executes a stored procedure, applies an optional specification, and projects results to <typeparamref name="TDto"/>.
    /// </summary>
    /// <typeparam name="TDto">The target DTO type.</typeparam>
    /// <param name="procedureCall">The stored procedure descriptor and its parameters.</param>
    /// <param name="specification">An optional specification to filter or order the result set.</param>
    /// <param name="projection">The EF Core server-side projection to apply.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A <see cref="LanguageExt.Common.Result{A}"/> wrapping the projected sequence.</returns>
    Task<LanguageExt.Common.Result<IEnumerable<TDto>>> ExecuteQueryProcedureAsync<TDto>(IProcedureCall procedureCall,
        ISpecification<TEntity>? specification, IProjection<TEntity, TDto> projection,
        CancellationToken cancellationToken) where TDto : class, IDto;
}

/// <summary>
///     Defines read-only stateless operations for a generic EF Core repository that creates short-lived
///     <typeparamref name="TDbContext"/> instances per operation.
/// </summary>
[SuppressMessage(
    "ReSharper",
    "UnusedMember.Global",
    Justification =
        "Repository interface methods are consumed indirectly through dependency injection and implementations.")]
internal interface IGenericDbRepositoryReadContext<TDbContext, TEntity>
    where TDbContext : DbContext where TEntity : class, IEntity
{
    /// <summary>
    ///     Asynchronously creates a new short-lived <typeparamref name="TDbContext"/> instance for direct use.
    ///     The caller is responsible for disposing it.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A new <typeparamref name="TDbContext"/> instance.</returns>
    [MustDisposeResource]
    Task<TDbContext> CreateDbContextAsync(CancellationToken cancellationToken);

    /// <summary>
    ///     Synchronously verifies whether any entity satisfies the given verification predicate.
    /// </summary>
    /// <param name="verification">The verification to apply, or <see langword="null"/> to check all entities.</param>
    /// <returns>A <see cref="Try{A}"/> wrapping <see langword="true"/> if any entity matches; otherwise <see langword="false"/>.</returns>
    Try<bool> Verificate(IVerification<TEntity>? verification);

    /// <summary>
    ///     Asynchronously verifies whether any entity satisfies the given verification predicate using a short-lived context.
    /// </summary>
    /// <param name="verification">The verification to apply, or <see langword="null"/> to check all entities.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A <see cref="LanguageExt.Common.Result{A}"/> wrapping <see langword="true"/> if any entity matches; otherwise <see langword="false"/>.</returns>
    Task<LanguageExt.Common.Result<bool>> VerificateAsync(IVerification<TEntity>? verification,
        CancellationToken cancellationToken);

    /// <summary>
    ///     Asynchronously verifies that exactly one entity satisfies the given verification predicate using a short-lived context.
    /// </summary>
    /// <param name="verification">The verification to apply, or <see langword="null"/> to check all entities.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A <see cref="LanguageExt.Common.Result{A}"/> wrapping <see langword="true"/> when exactly one entity matches; otherwise <see langword="false"/>.</returns>
    Task<LanguageExt.Common.Result<bool>> VerificateSingleAsync(IVerification<TEntity>? verification,
        CancellationToken cancellationToken);

    /// <summary>
    ///     Asynchronously retrieves all entities satisfying the specification using a short-lived context.
    /// </summary>
    /// <param name="specification">The query specification, or <see langword="null"/> to retrieve all entities.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A <see cref="LanguageExt.Common.Result{A}"/> wrapping the matched entity collection.</returns>
    Task<LanguageExt.Common.Result<ICollection<TEntity>>> GetAsync(ISpecification<TEntity>? specification,
        CancellationToken cancellationToken);

    /// <summary>
    ///     Asynchronously retrieves entities satisfying the specification and projects them server-side to <typeparamref name="TDto"/> using a short-lived context.
    /// </summary>
    /// <typeparam name="TDto">The target DTO type.</typeparam>
    /// <param name="specification">The query specification, or <see langword="null"/> to retrieve all entities.</param>
    /// <param name="projection">The EF Core server-side projection to apply.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A <see cref="LanguageExt.Common.Result{A}"/> wrapping the projected sequence.</returns>
    Task<LanguageExt.Common.Result<IEnumerable<TDto>>> GetAsync<TDto>(ISpecification<TEntity>? specification,
        IProjection<TEntity, TDto> projection,
        CancellationToken cancellationToken)
        where TDto : class, IDto;

    /// <summary>
    ///     Asynchronously retrieves the single entity matching the specification using a short-lived context.
    ///     Fails if zero or more than one entity is found.
    /// </summary>
    /// <param name="specification">The query specification.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A <see cref="LanguageExt.Common.Result{A}"/> wrapping the matched entity.</returns>
    Task<LanguageExt.Common.Result<TEntity>> GetSingleAsync(ISpecification<TEntity> specification,
        CancellationToken cancellationToken);

    /// <summary>
    ///     Asynchronously retrieves the single entity matching the specification and projects it to <typeparamref name="TDto"/> using a short-lived context.
    /// </summary>
    /// <typeparam name="TDto">The target DTO type.</typeparam>
    /// <param name="specification">The query specification.</param>
    /// <param name="projection">The EF Core server-side projection to apply.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A <see cref="LanguageExt.Common.Result{A}"/> wrapping the projected DTO.</returns>
    Task<LanguageExt.Common.Result<TDto>> GetSingleAsync<TDto>(ISpecification<TEntity> specification,
        IProjection<TEntity, TDto> projection,
        CancellationToken cancellationToken)
        where TDto : class, IDto;

    /// <summary>
    ///     Asynchronously retrieves the first entity matching the specification using a short-lived context.
    /// </summary>
    /// <param name="specification">The query specification.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A <see cref="LanguageExt.Common.Result{A}"/> wrapping the first matching entity.</returns>
    Task<LanguageExt.Common.Result<TEntity>> GetFirstAsync(ISpecification<TEntity> specification,
        CancellationToken cancellationToken);

    /// <summary>
    ///     Asynchronously retrieves the first entity matching the specification and projects it to <typeparamref name="TDto"/> using a short-lived context.
    /// </summary>
    /// <typeparam name="TDto">The target DTO type.</typeparam>
    /// <param name="specification">The query specification.</param>
    /// <param name="projection">The EF Core server-side projection to apply.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A <see cref="LanguageExt.Common.Result{A}"/> wrapping the first projected DTO.</returns>
    Task<LanguageExt.Common.Result<TDto>> GetFirstAsync<TDto>(ISpecification<TEntity> specification,
        IProjection<TEntity, TDto> projection,
        CancellationToken cancellationToken)
        where TDto : class, IDto;

    /// <summary>
    ///     Synchronously retrieves the first entity matching the specification and projects it to <typeparamref name="TDto"/> using a short-lived context.
    /// </summary>
    /// <typeparam name="TDto">The target DTO type.</typeparam>
    /// <param name="specification">The query specification.</param>
    /// <param name="projection">The EF Core server-side projection to apply.</param>
    /// <returns>A <see cref="Try{A}"/> wrapping the first projected DTO.</returns>
    Try<TDto> GetFirst<TDto>(ISpecification<TEntity> specification, IProjection<TEntity, TDto> projection)
        where TDto : class, IDto;

    /// <summary>
    ///     Asynchronously executes a non-query stored procedure using a short-lived context.
    /// </summary>
    /// <param name="procedureCall">The stored procedure descriptor and its parameters.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A <see cref="LanguageExt.Common.Result{A}"/> wrapping <see langword="true"/> when the procedure executes successfully.</returns>
    Task<LanguageExt.Common.Result<bool>> ExecuteProcedureAsync(IProcedureCall procedureCall,
        CancellationToken cancellationToken);

    /// <summary>
    ///     Asynchronously executes a stored procedure that returns a result set, applies an optional specification,
    ///     using a short-lived context.
    /// </summary>
    /// <param name="procedureCall">The stored procedure descriptor and its parameters.</param>
    /// <param name="specification">An optional specification to filter or order the result set.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A <see cref="LanguageExt.Common.Result{A}"/> wrapping the matched entity collection.</returns>
    Task<LanguageExt.Common.Result<ICollection<TEntity>>> ExecuteQueryProcedureAsync(IProcedureCall procedureCall,
        ISpecification<TEntity>? specification, CancellationToken cancellationToken);

    /// <summary>
    ///     Asynchronously executes a stored procedure, applies an optional specification, and projects results
    ///     to <typeparamref name="TDto"/> using a short-lived context.
    /// </summary>
    /// <typeparam name="TDto">The target DTO type.</typeparam>
    /// <param name="procedureCall">The stored procedure descriptor and its parameters.</param>
    /// <param name="specification">An optional specification to filter or order the result set.</param>
    /// <param name="projection">The EF Core server-side projection to apply.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A <see cref="LanguageExt.Common.Result{A}"/> wrapping the projected sequence.</returns>
    Task<LanguageExt.Common.Result<IEnumerable<TDto>>> ExecuteQueryProcedureAsync<TDto>(IProcedureCall procedureCall,
        ISpecification<TEntity>? specification, IProjection<TEntity, TDto> projection,
        CancellationToken cancellationToken) where TDto : class, IDto;
}