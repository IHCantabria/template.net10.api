using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using template.net10.api.Logger;

namespace template.net10.api.Core.Base;

/// <summary>
///     Base class for repositories that require a database context for stateful data access operations.
/// </summary>
internal class StatefulRepositoryBase : RepositoryBase
{
    /// <summary>
    ///     The Entity Framework database context used for data access operations.
    /// </summary>
    internal readonly DbContext Context;

    /// <summary>
    ///     Initializes a new instance of the <see cref="StatefulRepositoryBase"/> class.
    /// </summary>
    /// <param name="context">The Entity Framework database context.</param>
    /// <param name="logger">The logger instance for diagnostic output.</param>
    /// <exception cref="ArgumentNullException">
    ///     <paramref name="context"/> is <see langword="null"/>.
    ///     -or-
    ///     <paramref name="logger"/> is <see langword="null"/>.
    /// </exception>
    protected StatefulRepositoryBase(DbContext context, ILogger<StatefulRepositoryBase> logger) :
        base(logger)
    {
        Context = context ?? throw new ArgumentNullException(nameof(context));
    }
}

/// <summary>
///     Base class for repositories that operate without a database context for stateless operations.
/// </summary>
internal class StatelessRepositoryBase : RepositoryBase
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="StatelessRepositoryBase"/> class.
    /// </summary>
    /// <param name="logger">The logger instance for diagnostic output.</param>
    /// <exception cref="ArgumentNullException"><paramref name="logger"/> is <see langword="null"/>.</exception>
    protected StatelessRepositoryBase(ILogger<StatelessRepositoryBase> logger) : base(logger)
    {
    }
}

/// <summary>
///     Abstract base class for all repository implementations, providing common logging infrastructure.
/// </summary>
[SuppressMessage(
    "ReSharper",
    "MemberCanBePrivate.Global",
    Justification = "Members are intended to be accessed by derived classes.")]
internal class RepositoryBase
{
    /// <summary>
    ///     The logger instance used for diagnostic output in repository operations.
    /// </summary>
    internal readonly ILogger Logger;

    /// <summary>
    ///     Initializes a new instance of the <see cref="RepositoryBase"/> class.
    /// </summary>
    /// <param name="logger">The logger instance for diagnostic output.</param>
    /// <exception cref="ArgumentNullException"><paramref name="logger"/> is <see langword="null"/>.</exception>
    protected RepositoryBase(ILogger<RepositoryBase> logger)
    {
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        if (Logger.IsEnabled(LogLevel.Debug)) Logger.LogRepositoryBaseInjected(logger.GetType().ToString());
    }
}