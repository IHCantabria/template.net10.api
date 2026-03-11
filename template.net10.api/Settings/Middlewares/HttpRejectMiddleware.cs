using Microsoft.Extensions.Localization;
using template.net10.api.Core.Factory;
using template.net10.api.Localize.Resources;

namespace template.net10.api.Settings.Middlewares;

/// <summary>
///     Middleware that rejects all non-HTTPS requests with a 400 Bad Request ProblemDetails response,
///     enforcing HTTPS-only communication for the API.
/// </summary>
internal sealed class HttpRejectMiddleware(
    RequestDelegate next,
    IProblemDetailsService problemDetailsService,
    IStringLocalizer<ResourceMain> localizer)
{
    /// <summary>
    ///     String localizer used to produce the localized error message for non-HTTPS requests.
    /// </summary>
    private readonly IStringLocalizer<ResourceMain> _localizer =
        localizer ?? throw new ArgumentNullException(nameof(localizer));

    /// <summary>
    ///     The next middleware delegate in the request pipeline.
    /// </summary>
    private readonly RequestDelegate _next = next ?? throw new ArgumentNullException(nameof(next));

    /// <summary>
    ///     Service used to write the ProblemDetails payload to the HTTP response.
    /// </summary>
    private readonly IProblemDetailsService _problemDetailsService =
        problemDetailsService ?? throw new ArgumentNullException(nameof(problemDetailsService));

    /// <summary>
    ///     Passes HTTPS requests to the next middleware; rejects plain HTTP requests with a 400 response.
    /// </summary>
    /// <param name="context">The current HTTP context.</param>
    /// <returns>A <see cref="Task"/> that completes when the response is finished.</returns>
    /// <exception cref="Exception">A delegate callback throws an exception.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="context" /> is <see langword="null" />.</exception>
    public async Task InvokeAsync(HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        if (context.Request.IsHttps)
        {
            await _next(context).ConfigureAwait(false);
        }
        else
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            var problemDetails = ProblemDetailsFactoryCore.CreateProblemDetailsBadRequestHttpNotSupported(_localizer);
            await _problemDetailsService.TryWriteAsync(new ProblemDetailsContext
            {
                HttpContext = context,
                ProblemDetails = problemDetails
            }).ConfigureAwait(false);
        }
    }
}