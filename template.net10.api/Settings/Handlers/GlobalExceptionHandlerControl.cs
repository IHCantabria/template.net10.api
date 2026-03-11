using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Extensions.Localization;
using template.net10.api.Core.Factory;
using template.net10.api.Localize.Resources;
using template.net10.api.Logger;

namespace template.net10.api.Settings.Handlers;

/// <summary>
///     Global ASP.NET Core <see cref="IExceptionHandler"/> that catches all unhandled exceptions,
///     logs them, and writes a structured ProblemDetails response based on the HTTP status code.
/// </summary>
internal sealed class GlobalExceptionHandlerControl(
    IProblemDetailsService problemDetailsService,
    IStringLocalizer<ResourceMain> localizer,
    ILogger<GlobalExceptionHandlerControl> logger)
    : IExceptionHandler
{
    /// <summary>
    ///     String localizer used to produce localized error messages in ProblemDetails responses.
    /// </summary>
    private readonly IStringLocalizer<ResourceMain> _localizer =
        localizer ?? throw new ArgumentNullException(nameof(localizer));

    /// <summary>
    ///     Logger used to record server-side exception details for diagnostics.
    /// </summary>
    private readonly ILogger _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    /// <summary>
    ///     Service used to serialize and write the ProblemDetails payload to the HTTP response.
    /// </summary>
    private readonly IProblemDetailsService _problemDetailsService =
        problemDetailsService ?? throw new ArgumentNullException(nameof(problemDetailsService));

    /// <summary>
    ///     Attempts to handle <paramref name="exception"/> by logging it and writing a ProblemDetails
    ///     response derived from the current HTTP status code and the exception message.
    /// </summary>
    /// <param name="httpContext">The current HTTP context.</param>
    /// <param name="exception">The unhandled exception to process.</param>
    /// <param name="cancellationToken">Token to cancel the write operation.</param>
    /// <returns>
    ///     A <see cref="ValueTask{TResult}"/> of <see langword="true"/> when the exception was handled
    ///     and a response was written; otherwise <see langword="false"/>.
    /// </returns>
    public ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogExceptionServer(exception);
        var problemDetails =
            ProblemDetailsFactoryCore.CreateProblemDetailsByHttpStatusCode(
                (HttpStatusCode)httpContext.Response.StatusCode, exception,
                _localizer);
        return _problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = problemDetails
        });
    }
}