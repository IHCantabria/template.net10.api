using Microsoft.EntityFrameworkCore;
using template.net10.api.Persistence.Models.Interfaces;

namespace template.net10.api.Persistence.Repositories.Extensions;

/// <summary>
///     ADD DOCUMENTATION
/// </summary>
internal static class DbSetExtensions
{
    extension<TEntity>(DbSet<TEntity> set) where TEntity : class, IEntity
    {
        /// <summary>
        ///     ADD DOCUMENTATION
        /// </summary>
        /// <exception cref="OperationCanceledException">If the <see cref="CancellationToken" /> is canceled.</exception>
        internal async ValueTask<TEntity?> FindItemAsync(params object?[] keyValues)
        {
            return keyValues[^1] is CancellationToken ct
                ? await set.FindAsync(keyValues[..^1], ct).ConfigureAwait(false)
                : await set.FindAsync(keyValues).ConfigureAwait(false);
        }
    }
}