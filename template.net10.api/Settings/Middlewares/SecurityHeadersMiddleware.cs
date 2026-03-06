namespace template.net10.api.Settings.Middlewares;

/// <summary>
///     ADD DOCUMENTATION
/// </summary>
internal sealed class SecurityHeadersMiddleware(RequestDelegate next)
{
    /// <summary>
    ///     ADD DOCUMENTATION
    /// </summary>
    private readonly RequestDelegate _next = next ?? throw new ArgumentNullException(nameof(next));

    /// <summary>
    ///     ADD DOCUMENTATION
    /// </summary>
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
    ///     ADD DOCUMENTATION
    /// </summary>
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
    ///     ADD DOCUMENTATION
    /// </summary>
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