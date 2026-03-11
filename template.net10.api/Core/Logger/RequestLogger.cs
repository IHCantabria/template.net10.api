using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using Serilog.Context;
using template.net10.api.Logger;

namespace template.net10.api.Core.Logger;

/// <summary>
///     Provides methods to log HTTP request and response details including query, route, body parameters, and form fields.
/// </summary>
internal static class RequestLogger
{
    /// <summary>
    ///     Maximum allowed request body size in bytes (10 MB) for logging purposes.
    /// </summary>
    private const int MaxBodySizeBytes = 1024 * 1024 * 10; // Max 10MB

    /// <summary>
    ///     Logs an incoming HTTP request with its method, path, query parameters, route parameters, body content, and form
    ///     fields.
    /// </summary>
    /// <param name="context">The current HTTP context.</param>
    /// <param name="logger">The logger instance to write to.</param>
    [SuppressMessage("ReSharper", "InconsistentContextLogPropertyNaming",
        Justification = "OpenTelemetry log properties follow a hierarchical naming convention (parent.child).")]
    internal static async Task LogActionRequestAsync(HttpContext context, ILogger logger)
    {
        var methodName = context.Request.Method;
        var requestPath = context.Request.Path;

        var queryParams = ExtractQueryParams(context);
        var routeParams = ExtractRouteParams(context);
        var (bodyContent, formFields) = await ExtractRequestBodyAsync(context).ConfigureAwait(false);

        using (LogContext.PushProperty("request.params.query", queryParams, true))
        using (LogContext.PushProperty("request.params.route", routeParams, true))
        using (LogContext.PushProperty("request.params.body", bodyContent ?? string.Empty, true))
        using (LogContext.PushProperty("request.params.form_field", formFields ?? [],
                   true))
        {
            if (logger.IsEnabled(LogLevel.Information)) logger.LogActionRequestReceived(methodName, requestPath);
        }
    }

    /// <summary>
    ///     Extracts query string parameters from the current HTTP request as a dictionary.
    /// </summary>
    /// <param name="context">The current HTTP context.</param>
    /// <returns>A dictionary of query parameter key-value pairs.</returns>
    private static Dictionary<string, string> ExtractQueryParams(HttpContext context)
    {
        return context.Request.Query.ToDictionary(static kvp => kvp.Key, static kvp => kvp.Value.ToString());
    }

    /// <summary>
    ///     Extracts route data values from the current HTTP request as a dictionary.
    /// </summary>
    /// <param name="context">The current HTTP context.</param>
    /// <returns>A dictionary of route parameter key-value pairs.</returns>
    private static Dictionary<string, string> ExtractRouteParams(HttpContext context)
    {
        return context.GetRouteData()?.Values
                   .ToDictionary(static kvp => kvp.Key, static kvp => kvp.Value?.ToString() ?? string.Empty)
               ?? [];
    }

    /// <summary>
    ///     Extracts the request body content and form fields from POST, PUT, PATCH, or DELETE requests.
    /// </summary>
    /// <param name="context">The current HTTP context.</param>
    /// <returns>A tuple containing the JSON body content and form field dictionary, or nulls for non-body HTTP methods.</returns>
    private static async Task<(string? BodyContent, Dictionary<string, string>? FormFields)>
        ExtractRequestBodyAsync(HttpContext context)
    {
        if (context.Request.Method is not ("POST" or "PUT" or "PATCH" or "DELETE"))
            return (null, null);

        var bodyContent = await TryReadJsonBodyAsync(context).ConfigureAwait(false);
        var formFields = await TryReadFormFieldsAsync(context).ConfigureAwait(false);
        return (bodyContent, formFields);
    }

    /// <summary>
    ///     Reads the JSON body from the request stream with buffering enabled, limited to <see cref="MaxBodySizeBytes" />.
    /// </summary>
    /// <param name="context">The current HTTP context.</param>
    /// <returns>The JSON body content string, or <see langword="null" /> if the content type is not JSON.</returns>
    private static async Task<string?> TryReadJsonBodyAsync(HttpContext context)
    {
        var contentType = context.Request.ContentType ?? string.Empty;
        if (!contentType.Contains("application/json", StringComparison.OrdinalIgnoreCase))
            return null;

        context.Request.EnableBuffering();

        using var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true);
        var body = await ReadLimitedAsync(reader).ConfigureAwait(false);
        context.Request.Body.Position = 0;

        return body;
    }

    /// <summary>
    ///     Reads form fields from the request if the content type is URL-encoded or multipart form data.
    /// </summary>
    /// <param name="context">The current HTTP context.</param>
    /// <returns>A dictionary of form field key-value pairs, or <see langword="null" /> if the content type does not match.</returns>
    private static async Task<Dictionary<string, string>?> TryReadFormFieldsAsync(HttpContext context)
    {
        var contentType = context.Request.ContentType ?? string.Empty;
        if (!contentType.Contains("application/x-www-form-urlencoded", StringComparison.OrdinalIgnoreCase) &&
            !contentType.Contains("multipart/form-data", StringComparison.OrdinalIgnoreCase))
            return null;

        context.Request.EnableBuffering();
        var form = await context.Request.ReadFormAsync().ConfigureAwait(false);
        var formFields = form.ToDictionary(static kvp => kvp.Key, static kvp => kvp.Value.ToString());

        context.Request.Body.Position = 0;
        return formFields;
    }

    /// <summary>
    ///     Reads up to <see cref="MaxBodySizeBytes" /> characters from the specified text reader.
    /// </summary>
    /// <param name="reader">The text reader to read from.</param>
    /// <returns>The content read from the reader, truncated to the maximum allowed size.</returns>
    private static async Task<string> ReadLimitedAsync(TextReader reader)
    {
        var buffer = new char[MaxBodySizeBytes / sizeof(char)];
        var read = await reader.ReadBlockAsync(buffer, 0, buffer.Length).ConfigureAwait(false);
        return new string(buffer, 0, read);
    }

    /// <summary>
    ///     Logs a successful HTTP response with the resolved controller action name and request path.
    /// </summary>
    /// <param name="context">The current HTTP context.</param>
    /// <param name="logger">The logger instance to write to.</param>
    internal static void LogActionResponseSuccess(HttpContext context,
        ILogger logger)
    {
        var routeData = context.GetRouteData();
        var controller = routeData?.Values["controller"]?.ToString();
        var action = routeData?.Values["action"]?.ToString();
        var methodName = string.IsNullOrEmpty(controller) || string.IsNullOrEmpty(action)
            ? context.Request.Method
            : $"{controller}.{action}";
        var requestPath = context.Request.Path;
        if (logger.IsEnabled(LogLevel.Information)) logger.LogActionRequestResponsedSuccess(methodName, requestPath);
    }

    /// <summary>
    ///     Logs an HTTP error response with the controller action name, request path, status code, and response body.
    /// </summary>
    /// <param name="context">The current HTTP context.</param>
    /// <param name="responseText">The error response body text.</param>
    /// <param name="logger">The logger instance to write to.</param>
    [SuppressMessage("ReSharper", "InconsistentContextLogPropertyNaming",
        Justification =
            "Open Telemetry fields should have this format: father.child")]
    internal static void LogActionResponseError(HttpContext context, string responseText,
        ILogger logger)
    {
        var routeData = context.GetRouteData();
        var controller = routeData?.Values["controller"]?.ToString();
        var action = routeData?.Values["action"]?.ToString();
        var methodName = string.IsNullOrEmpty(controller) || string.IsNullOrEmpty(action)
            ? context.Request.Method
            : $"{controller}.{action}";
        var requestPath = context.Request.Path;
        using (LogContext.PushProperty("request.response.error", responseText))
        {
            logger.LogActionRequestResponsedError(methodName, requestPath,
                context.Response.StatusCode.ToString(CultureInfo.InvariantCulture));
        }
    }
}