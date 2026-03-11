using System.Diagnostics;

namespace template.net10.api.Settings.Middlewares;

/// <summary>
///     Middleware that enriches the current OpenTelemetry <see cref="Activity"/> with thread-level tags
///     (<c>request.thread.id</c>, <c>request.thread.name</c>) for tracing and diagnostics.
/// </summary>
internal sealed class TelemetryMiddleware(RequestDelegate next)
{
    /// <summary>
    ///     The next middleware delegate in the request pipeline.
    /// </summary>
    private readonly RequestDelegate _next = next ?? throw new ArgumentNullException(nameof(next));

    /// <summary>
    ///     Tags the current <see cref="Activity"/> with the managed thread ID and name, then
    ///     forwards the request to the next middleware in the pipeline.
    /// </summary>
    /// <param name="context">The current HTTP context.</param>
    /// <returns>A <see cref="Task"/> that completes when the pipeline finishes.</returns>
    /// <exception cref="InvalidOperationException">
    ///     A set operation was requested, but the <see langword="Name" /> property has
    ///     already been set.
    /// </exception>
    /// <exception cref="Exception">A delegate callback throws an exception.</exception>
    public Task InvokeAsync(HttpContext context)
    {
        var activity = Activity.Current;
        if (activity == null) return _next(context);

        var threadId = Environment.CurrentManagedThreadId;
        var threadName = Thread.CurrentThread.Name ?? string.Empty;

        activity.SetTag("request.thread.id", threadId);
        activity.SetTag("request.thread.name", threadName);

        return _next(context);
    }
}