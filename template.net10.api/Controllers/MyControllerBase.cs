using System.Diagnostics.CodeAnalysis;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using template.net10.api.Localize.Resources;
using template.net10.api.Logger;

namespace template.net10.api.Controllers;

/// <summary>
///     Abstract base controller providing shared dependencies and common functionality for all API controllers.
/// </summary>
[SuppressMessage("Design",
    "CA1515:Consider making public types internal",
    Justification =
        "Controllers must remain public to allow OpenAPI discovery and correct API exposure.")]
[SuppressMessage(
    "ReSharper",
    "MemberCanBePrivate.Global",
    Justification = "Members are intended to be accessed by derived classes.")]
public abstract class MyControllerBase : ControllerBase
{
    /// <summary>
    ///     Localization service for resolving translated resource strings.
    /// </summary>
    internal readonly IStringLocalizer<ResourceMain> Localizer;

    /// <summary>
    ///     Logger instance for recording diagnostic information.
    /// </summary>
    internal readonly ILogger Logger;

    /// <summary>
    ///     MediatR mediator instance for dispatching commands and queries.
    /// </summary>
    internal readonly IMediator Mediator;

    /// <summary>
    ///     Initializes a new instance of the <see cref="MyControllerBase"/> class with the required dependencies.
    /// </summary>
    /// <param name="mediator">The MediatR mediator for dispatching requests.</param>
    /// <param name="localizer">The string localizer for resource translations.</param>
    /// <param name="logger">The logger instance for diagnostic output.</param>
    /// <exception cref="ArgumentNullException">
    ///     <paramref name="mediator"/> is <see langword="null"/>.
    ///     -or-
    ///     <paramref name="localizer"/> is <see langword="null"/>.
    ///     -or-
    ///     <paramref name="logger"/> is <see langword="null"/>.
    /// </exception>
    protected MyControllerBase(IMediator mediator, IStringLocalizer<ResourceMain> localizer,
        ILogger<MyControllerBase> logger)
    {
        Mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        Localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
        if (Logger.IsEnabled(LogLevel.Debug)) Logger.LogControllerBaseInjected(logger.GetType().ToString());
    }
}