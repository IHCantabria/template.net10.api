namespace template.net10.api.Settings.Middlewares;

/// <summary>
///     Middleware that appends security-related HTTP response headers (CSP, X-Content-Type-Options,
///     X-Frame-Options, Referrer-Policy, Permissions-Policy) to every response to harden the API
///     against common browser-based attacks.
/// </summary>
internal sealed class SecurityHeadersMiddleware(RequestDelegate next)
{
    /// <summary>
    ///     The next middleware delegate in the request pipeline.
    /// </summary>
    private readonly RequestDelegate _next = next ?? throw new ArgumentNullException(nameof(next));

    /// <summary>
    ///     Registers a response-starting callback that sets all security headers, then forwards the request.
    /// </summary>
    /// <param name="context">The current HTTP context.</param>
    /// <returns>A <see cref="Task"/> that completes when the request pipeline finishes.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="context" /> is <see langword="null" />.</exception>
    /// <exception cref="Exception">A delegate callback throws an exception.</exception>
    public Task InvokeAsync(HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        context.Response.OnStarting(() =>
        {
            SetSecurityHeaders(context);

            return Task.CompletedTask;
        });

        return _next(context);
    }

    /// <summary>
    ///     Sets all security response headers on the current <paramref name="context"/> response.
    /// </summary>
    /// <param name="context">The current HTTP context whose response headers are to be set.</param>
    private static void SetSecurityHeaders(HttpContext context)
    {
        // Content-Security-Policy Header
        context.Response.Headers["Content-Security-Policy"] = BuildCsp();

        // X-Content-Type-Options
        context.Response.Headers["X-Content-Type-Options"] = "nosniff";

        // / X-Frame-Options (legacy browser compatibility; modern protection should be implemented using CSP frame-ancestors)
        context.Response.Headers["X-Frame-Options"] = "SAMEORIGIN";

        // Referrer-Policy
        context.Response.Headers["Referrer-Policy"] = "no-referrer";

        // X-Permitted-Cross-Domain-Policies
        context.Response.Headers["X-Permitted-Cross-Domain-Policies"] = "none";

        // Permissions-Policy
        context.Response.Headers["Permissions-Policy"] =
            "accelerometer=(), camera=(), geolocation=(), gyroscope=(), magnetometer=(), microphone=(), payment=(), usb=()";
    }

    /// <summary>
    ///     Builds the <c>Content-Security-Policy</c> header value restricting script, style, image,
    ///     font, and connection sources to trusted origins only.
    /// </summary>
    /// <returns>The CSP directive string.</returns>
    private static string BuildCsp()
    {
        return string.Join(' ',
            "default-src 'none';",
            "base-uri 'none';",
            "form-action 'none';",
            "frame-ancestors 'none';",
            "object-src 'none';",
            "script-src 'self' 'unsafe-eval';",
            "script-src-elem 'self';",
            "style-src 'self' 'unsafe-inline';",
            "style-src-elem 'self' 'unsafe-inline' https://fonts.googleapis.com;",
            "img-src 'self' data: https://cdn.redoc.ly;",
            "font-src 'self' https://fonts.gstatic.com;",
            "connect-src 'self' ws: wss:;",
            "manifest-src 'self';",
            "worker-src 'self' blob:;");
    }
}