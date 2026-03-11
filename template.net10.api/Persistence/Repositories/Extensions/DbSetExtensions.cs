using Microsoft.EntityFrameworkCore;
using template.net10.api.Persistence.Models.Interfaces;

namespace template.net10.api.Persistence.Repositories.Extensions;

/// <summary>
///     C# 13 extension block providing helper methods on <see cref="DbSet{TEntity}"/> for finding entities by key.
/// </summary>
internal static class DbSetExtensions
{
    extension<TEntity>(DbSet<TEntity> set) where TEntity : class, IEntity
    {
        /// <summary>
        ///     Asynchronously finds an entity by its primary key, automatically extracting a
        ///     <see cref="CancellationToken"/> if it is the last element of <paramref name="keyValues"/>.
        /// </summary>
        /// <param name="keyValues">The primary key values. Optionally, the last element may be a <see cref="CancellationToken"/>.</param>
        /// <returns>The entity if found; otherwise <see langword="null"/>.</returns>
        /// <exception cref="OperationCanceledException">If the <see cref="CancellationToken" /> is canceled.</exception>
        internal async ValueTask<TEntity?> FindItemAsync(params object?[] keyValues)
        {
            return keyValues[^1] is CancellationToken ct
                ? await set.FindAsync(keyValues[..^1], ct).ConfigureAwait(false)
                : await set.FindAsync(keyValues).ConfigureAwait(false);
        }
    }
}