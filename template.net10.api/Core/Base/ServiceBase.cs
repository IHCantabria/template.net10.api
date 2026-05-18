using template.net10.api.Core.Attributes;
using template.net10.api.Core.Interfaces;
using template.net10.api.Logger;

namespace template.net10.api.Core.Base;

/// <summary>
///     Base class for all normal service implementations, providing common logging infrastructure and scoped lifetime
///     registration.
/// </summary>
[ServiceLifetime(ServiceLifetime.Scoped)]
[ServiceType(ServiceType.Normal)]
internal abstract class ServiceBase : IServiceImplementation
{
    /// <summary>
    ///     The logger instance used for diagnostic output in service operations.
    /// </summary>
    protected readonly ILogger Logger;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ServiceBase" /> class.
    /// </summary>
    /// <param name="logger">The logger instance for diagnostic output.</param>
    /// <exception cref="ArgumentNullException"><paramref name="logger" /> is <see langword="null" />.</exception>
    protected ServiceBase(ILogger logger)
    {
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        if (Logger.IsEnabled(LogLevel.Debug)) Logger.LogServiceBaseInjected(logger.GetType().ToString());
    }
}