using System.Diagnostics.CodeAnalysis;
using System.IO.Compression;
using System.Text;
using template.net10.api.Core.Logger;

namespace template.net10.api.Settings.Middlewares;

/// <summary>
///     Middleware that logs HTTP request details on entry and logs the response body on completion,
///     capturing error responses (4xx/5xx) with their body content for structured diagnostics.
/// </summary>
internal sealed class HttpRequestResponseLoggingMiddleware(
    RequestDelegate next,
    ILogger<HttpRequestResponseLoggingMiddleware> logger)
{
    /// <summary>
    ///     Logger used to record request and response details.
    /// </summary>
    private readonly ILogger _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    /// <summary>
    ///     The next middleware delegate in the request pipeline.
    /// </summary>
    private readonly RequestDelegate _next = next ?? throw new ArgumentNullException(nameof(next));

    /// <summary>
    ///     Logs the incoming request, buffers the response body, and logs the response outcome.
    ///     On error status codes the response body is also captured and logged.
    /// </summary>
    /// <param name="context">The current HTTP context.</param>
    /// <returns>A <see cref="Task"/> that completes when the pipeline finishes.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="context" /> is <see langword="null" />.</exception>
    /// <exception cref="Exception">A delegate callback throws an exception.</exception>
    [SuppressMessage("ReSharper", "CA2007",
        Justification =
            "ConfigureAwait cant be injected in the Buffer Memory Stream")]
    public async Task InvokeAsync(HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        await RequestLogger.LogActionRequestAsync(context, _logger).ConfigureAwait(false);

        var originalBody = context.Response.Body;
        await using var buffer = new MemoryStream();
        context.Response.Body = buffer;

        try
        {
            await _next(context).ConfigureAwait(false);
            await HandleResponseAsync(context, buffer, originalBody).ConfigureAwait(false);
        }
        finally
        {
            context.Response.Body = originalBody;
        }
    }

    /// <summary>
    ///     Logs the response outcome and copies the buffered body back to the original response stream.
    ///     For 4xx/5xx responses the body text is read and included in the error log.
    /// </summary>
    /// <param name="context">The current HTTP context.</param>
    /// <param name="buffer">The memory stream that captured the response body.</param>
    /// <param name="originalBody">The original response body stream to restore.</param>
    private async Task HandleResponseAsync(HttpContext context, Stream buffer, Stream originalBody)
    {
        if (context.Response.StatusCode >= 400)
        {
            var text = await ReadResponseTextAsync(buffer, context.Response.Headers)
                .ConfigureAwait(false);
            RequestLogger.LogActionResponseError(context, text, _logger);
        }
        else
        {
            RequestLogger.LogActionResponseSuccess(context, _logger);
        }

        buffer.Seek(0, SeekOrigin.Begin);
        await buffer.CopyToAsync(originalBody).ConfigureAwait(false);
    }

    /// <summary>
    ///     Reads the buffered response body as a UTF-8 string, transparently decompressing
    ///     GZip or Brotli encoding when indicated by the response <c>Content-Encoding</c> header.
    /// </summary>
    /// <param name="buffer">The memory stream containing the raw response bytes.</param>
    /// <param name="headers">The response headers used to detect content encoding.</param>
    /// <returns>The response body as a plain text string.</returns>
    private static async Task<string> ReadResponseTextAsync(
        Stream buffer,
        IHeaderDictionary headers)
    {
        buffer.Seek(0, SeekOrigin.Begin);

        var payload = buffer;
        if (headers.TryGetValue("Content-Encoding", out var enc))
        {
            var encVal = enc.ToString().ToUpperInvariant();
            if (encVal.Contains("GZIP", StringComparison.InvariantCultureIgnoreCase))
                payload = new GZipStream(buffer, CompressionMode.Decompress, true);
            else if (encVal.Contains("BR", StringComparison.InvariantCultureIgnoreCase))
                payload = new BrotliStream(buffer, CompressionMode.Decompress, true);
        }

        using var reader = new StreamReader(
            payload,
            Encoding.UTF8,
            false,
            1024,
            true);

        var text = await reader.ReadToEndAsync().ConfigureAwait(false);
        if (payload != buffer)
            await payload.DisposeAsync().ConfigureAwait(false);

        return text;
    }
}