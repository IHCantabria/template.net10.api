namespace template.net10.api.Settings.Middlewares;

/// <summary>
///     Middleware that enriches 401 and 403 responses with a well-formed <c>WWW-Authenticate</c> Bearer header,
///     using the error and description stored in <c>HttpContext.Items</c> by the JWT bearer events.
/// </summary>
internal sealed class AuthenticationHeaderMiddleware(RequestDelegate next)
{
    /// <summary>
    ///     The next middleware delegate in the request pipeline.
    /// </summary>
    private readonly RequestDelegate _next = next ?? throw new ArgumentNullException(nameof(next));

    /// <summary>
    ///     Processes the HTTP request by registering a response-starting callback that appends a
    ///     <c>WWW-Authenticate</c> Bearer header on 401 and 403 responses, then delegates execution
    ///     to the next middleware in the pipeline.
    /// </summary>
    /// <param name="context">The HTTP context for the current request.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous middleware operation.</returns>
    /// <exception cref="Exception">A delegate callback throws an exception.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="context" /> is <see langword="null" />.</exception>
    public Task InvokeAsync(HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        context.Response.OnStarting(() =>
        {
            switch (context.Response.StatusCode)
            {
                case StatusCodes.Status401Unauthorized:
                {
                    SetUnauthorizedHeader(context);
                    break;
                }

                case StatusCodes.Status403Forbidden:
                {
                    SetForbiddenHeader(context);
                    break;
                }
            }

            return Task.CompletedTask;
        });

        return _next(context);
    }

    /// <summary>
    ///     Removes any existing <c>WWW-Authenticate</c> header and sets a Bearer error header
    ///     with the error code and description for a 401 Unauthorized response.
    /// </summary>
    /// <param name="context">The current HTTP context.</param>
    private static void SetUnauthorizedHeader(HttpContext context)
    {
        var err = context.Items.TryGetValue("BearerError", out var e) ? e as string : "invalid_token";
        var desc = context.Items.TryGetValue("BearerErrorDescription", out var d)
            ? d as string
            : "The access token is missing, invalid, or expired";

        context.Response.Headers.Remove("WWW-Authenticate");
        context.Response.Headers.Append(
            "WWW-Authenticate",
            $"Bearer error=\"{err}\", error_description=\"{desc}\""
        );
    }

    /// <summary>
    ///     Removes any existing <c>WWW-Authenticate</c> header and sets a Bearer <c>insufficient_scope</c> header
    ///     for a 403 Forbidden response.
    /// </summary>
    /// <param name="context">The current HTTP context.</param>
    private static void SetForbiddenHeader(HttpContext context)
    {
        var desc = context.Items.TryGetValue("BearerErrorDescription", out var d)
            ? d as string
            : "The token does not have sufficient scope for this resource";

        context.Response.Headers.Remove("WWW-Authenticate");
        context.Response.Headers.Append(
            "WWW-Authenticate",
            $"Bearer error=\"insufficient_scope\", error_description=\"{desc}\""
        );
    }
}