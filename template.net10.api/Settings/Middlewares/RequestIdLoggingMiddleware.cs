using template.net10.api.Core.Logger;

namespace template.net10.api.Settings.Middlewares;

/// <summary>
///     Middleware that captures the ASP.NET Core <see cref="HttpContext.TraceIdentifier"/> for each request
///     and stores it in the static <c>RequestIdProvider</c> so it can be included in log entries.
/// </summary>
internal sealed class RequestIdLoggingMiddleware(RequestDelegate next)
{
    /// <summary>
    ///     The next middleware delegate in the request pipeline.
    /// </summary>
    private readonly RequestDelegate _next = next ?? throw new ArgumentNullException(nameof(next));

    /// <summary>
    ///     Stores the current request's trace identifier in <c>RequestIdProvider</c> and forwards
    ///     the request to the next middleware in the pipeline.
    /// </summary>
    /// <param name="context">The current HTTP context.</param>
    /// <returns>A <see cref="Task"/> that completes when the request pipeline finishes.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="context" /> is <see langword="null" />.</exception>
    /// <exception cref="Exception">A delegate callback throws an exception.</exception>
    public Task InvokeAsync(HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        // Store the HttpContext.TraceIdentifier in the static provider
        RequestIdProvider.SetCurrentTraceIdentifier(context.TraceIdentifier);

        // Continue processing the request
        return _next(context);
    }
}