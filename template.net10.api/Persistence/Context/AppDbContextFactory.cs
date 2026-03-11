using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace template.net10.api.Persistence.Context;

/// <summary>
///     Pooled <see cref="IDbContextFactory{TContext}"/> wrapper for <see cref="AppDbContext"/>.
///     Delegates creation to the underlying pooled factory, enabling efficient reuse of context instances.
/// </summary>
internal sealed class AppDbContextFactory(IDbContextFactory<AppDbContext> pooledFactory)
    : IDbContextFactory<AppDbContext>
{
    /// <summary>
    ///     The inner EF Core pooled factory used to create and return context instances.
    /// </summary>
    private readonly IDbContextFactory<AppDbContext> _pooledFactory =
        pooledFactory ?? throw new ArgumentNullException(nameof(pooledFactory));

    /// <summary>
    ///     Creates and returns a new <see cref="AppDbContext"/> instance from the pool.
    /// </summary>
    /// <returns>A new <see cref="AppDbContext"/> instance.</returns>
    [MustDisposeResource]
    public AppDbContext CreateDbContext()
    {
        return _pooledFactory.CreateDbContext();
    }

    /// <summary>
    ///     Asynchronously creates and returns a new <see cref="AppDbContext"/> instance from the pool.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A task representing the asynchronous creation of an <see cref="AppDbContext"/>.</returns>
    /// <exception cref="OperationCanceledException">If the <see cref="CancellationToken" /> is canceled.</exception>
    [MustDisposeResource]
    public Task<AppDbContext> CreateDbContextAsync(CancellationToken cancellationToken = default)
    {
        return _pooledFactory.CreateDbContextAsync(cancellationToken);
    }
}