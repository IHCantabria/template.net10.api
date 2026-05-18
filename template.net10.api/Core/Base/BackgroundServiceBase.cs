using System.Diagnostics.CodeAnalysis;
using template.net10.api.Core.Attributes;
using template.net10.api.Core.Interfaces;
using template.net10.api.Logger;

namespace template.net10.api.Core.Base;

/// <summary>
///     Abstract base class for all hosted background services in the application.
///     Inherits from <see cref="BackgroundService" /> and implements <see cref="IServiceImplementation" />.
///     Ensures a valid <see cref="ILogger" /> instance is injected and annotates derived classes with
///     <see cref="ServiceType.Background" /> so the DI registration scanner can identify and register
///     them as hosted services automatically.
/// </summary>
[ServiceType(ServiceType.Background)]
[SuppressMessage(
    "ReSharper",
    "MemberCanBePrivate.Global",
    Justification = "Members are intended to be accessed by derived classes.")]
internal abstract class BackgroundServiceBase : BackgroundService, IServiceImplementation
{
    /// <summary>
    ///     The logger instance used for diagnostic output in service operations.
    /// </summary>
    protected readonly ILogger Logger;

    /// <summary>
    ///     Initializes a new instance of the <see cref="BackgroundServiceBase" /> class.
    /// </summary>
    /// <param name="logger">The logger instance for diagnostic output.</param>
    /// <exception cref="ArgumentNullException"><paramref name="logger" /> is <see langword="null" />.</exception>
    protected BackgroundServiceBase(ILogger logger)
    {
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        if (Logger.IsEnabled(LogLevel.Debug)) Logger.LogServiceBaseInjected(logger.GetType().ToString());
    }

    /// <inheritdoc />
    /// <exception cref="NotImplementedException"></exception>
    protected abstract override Task ExecuteAsync(CancellationToken stoppingToken);
}