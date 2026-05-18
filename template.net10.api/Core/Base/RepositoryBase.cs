using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using template.net10.api.Logger;

namespace template.net10.api.Core.Base;

/// <summary>
///     Base class for repositories that require a database context for stateful data access operations.
/// </summary>
/// <param name="context">The Entity Framework database context.</param>
/// <param name="logger">The logger instance for diagnostic output.</param>
/// <exception cref="ArgumentNullException"><paramref name="context" /> is <see langword="null" />.</exception>
internal abstract class StatefulRepositoryBase(DbContext context, ILogger logger) : RepositoryBase(logger)
{
    /// <summary>
    ///     The Entity Framework database context used for data access operations.
    /// </summary>
    internal readonly DbContext Context = context ?? throw new ArgumentNullException(nameof(context));
}

/// <summary>
///     Base class for repositories that operate without a database context for stateless operations.
/// </summary>
/// <param name="logger">The logger instance for diagnostic output.</param>
/// <exception cref="ArgumentNullException"><paramref name="logger" /> is <see langword="null" />.</exception>
internal abstract class StatelessRepositoryBase(ILogger logger) : RepositoryBase(logger);

/// <summary>
///     Abstract base class for all repository implementations, providing common logging infrastructure.
/// </summary>
[SuppressMessage(
    "ReSharper",
    "MemberCanBePrivate.Global",
    Justification = "Members are intended to be accessed by derived classes.")]
internal abstract class RepositoryBase
{
    /// <summary>
    ///     The logger instance used for diagnostic output in repository operations.
    /// </summary>
    protected readonly ILogger Logger;

    /// <summary>
    ///     Initializes a new instance of the <see cref="RepositoryBase" /> class.
    /// </summary>
    /// <param name="logger">The logger instance for diagnostic output.</param>
    /// <exception cref="ArgumentNullException"><paramref name="logger" /> is <see langword="null" />.</exception>
    protected RepositoryBase(ILogger logger)
    {
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        if (Logger.IsEnabled(LogLevel.Debug)) Logger.LogRepositoryBaseInjected(logger.GetType().ToString());
    }
}