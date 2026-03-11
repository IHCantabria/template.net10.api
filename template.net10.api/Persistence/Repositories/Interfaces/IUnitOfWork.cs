using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace template.net10.api.Persistence.Repositories.Interfaces;

/// <summary>
///     Unit of Work abstraction over a <typeparamref name="TDbContext"/> that coordinates persistence
///     and transaction management for a single logical operation.
/// </summary>
[SuppressMessage(
    "ReSharper",
    "UnusedMember.Global",
    Justification =
        "Repository interface methods are consumed indirectly through dependency injection and implementations.")]
internal interface IUnitOfWork<out TDbContext> where TDbContext : DbContext
{
    /// <summary>
    ///     Asynchronously persists all pending changes in the tracked context to the database.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A <see cref="LanguageExt.Common.Result{A}"/> wrapping <see langword="true"/> when the save succeeds.</returns>
    Task<LanguageExt.Common.Result<bool>> SaveChangesAsync(CancellationToken cancellationToken);

    /// <summary>
    ///     Asynchronously begins a new database transaction.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A <see cref="LanguageExt.Common.Result{A}"/> wrapping <see langword="true"/> when the transaction is started.</returns>
    Task<LanguageExt.Common.Result<bool>> BeginTransactionAsync(CancellationToken cancellationToken);

    /// <summary>
    ///     Asynchronously commits the active database transaction.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A <see cref="LanguageExt.Common.Result{A}"/> wrapping <see langword="true"/> when the commit succeeds.</returns>
    Task<LanguageExt.Common.Result<bool>> CommitTransactionAsync(CancellationToken cancellationToken);

    /// <summary>
    ///     Asynchronously rolls back the active database transaction.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A <see cref="LanguageExt.Common.Result{A}"/> wrapping <see langword="true"/> when the rollback succeeds.</returns>
    Task<LanguageExt.Common.Result<bool>> RollbackTransactionAsync(CancellationToken cancellationToken);

    /// <summary>
    ///     Asynchronously releases the active transaction, committing it and returning the connection to a clean state.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A <see cref="LanguageExt.Common.Result{A}"/> wrapping <see langword="true"/> when the release succeeds.</returns>
    Task<LanguageExt.Common.Result<bool>> ReleaseTransactionAsync(CancellationToken cancellationToken);

    /// <summary>
    ///     Asynchronously creates a named savepoint within the active transaction.
    /// </summary>
    /// <param name="name">The name of the savepoint to create.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A <see cref="LanguageExt.Common.Result{A}"/> wrapping <see langword="true"/> when the savepoint is created.</returns>
    Task<LanguageExt.Common.Result<bool>> AddSavepointAsync(string name, CancellationToken cancellationToken);

    /// <summary>
    ///     Asynchronously rolls the transaction back to the specified named savepoint.
    /// </summary>
    /// <param name="name">The name of the savepoint to roll back to.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A <see cref="LanguageExt.Common.Result{A}"/> wrapping <see langword="true"/> when the rollback succeeds.</returns>
    Task<LanguageExt.Common.Result<bool>> RollbackToSavepointAsync(string name, CancellationToken cancellationToken);

    /// <summary>
    ///     Asynchronously releases the specified named savepoint from the active transaction.
    /// </summary>
    /// <param name="name">The name of the savepoint to release.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A <see cref="LanguageExt.Common.Result{A}"/> wrapping <see langword="true"/> when the savepoint is released.</returns>
    Task<LanguageExt.Common.Result<bool>> ReleaseSavepointAsync(string name, CancellationToken cancellationToken);

    /// <summary>
    ///     Returns the underlying <typeparamref name="TDbContext"/> for direct data access.
    /// </summary>
    /// <returns>The <typeparamref name="TDbContext"/> instance managed by this unit of work.</returns>
    [MustDisposeResource]
    TDbContext DbContext();
}