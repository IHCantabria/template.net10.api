using System.Diagnostics;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;

namespace template.net10.api.Settings.Extensions;

/// <summary>
///     Extension methods for <see cref="ProblemDetailsContext"/> to enrich RFC 7807 problem-detail responses
///     with contextual diagnostic fields such as request ID, trace ID, HTTP method, and instance URL.
/// </summary>
internal static class ProblemDetailsContextExtensions
{
    /// <summary>
    ///     Merges two extension dictionaries, with entries from <paramref name="clientExtensions"/> taking
    ///     precedence over matching keys from <paramref name="serverExtensions"/>.
    /// </summary>
    /// <param name="serverExtensions">The base extension dictionary produced by the server.</param>
    /// <param name="clientExtensions">Client-supplied extensions that may override server values.</param>
    /// <returns>A new merged dictionary containing entries from both sources.</returns>
    private static Dictionary<string, object?> MergeExtensions(IDictionary<string, object?> serverExtensions,
        IDictionary<string, object?> clientExtensions)
    {
        Dictionary<string, object?> mergedExtensions = new(serverExtensions);

        //Should be Serial
        foreach (var entry in clientExtensions) mergedExtensions[entry.Key] = entry.Value;

        return mergedExtensions;
    }

    /// <summary>
    ///     Returns <see langword="true"/> when running in a production environment and the HTTP status
    ///     is 500 or above, indicating that internal error details must be hidden from the response.
    /// </summary>
    /// <param name="env">The current host environment.</param>
    /// <param name="status">The HTTP status code of the problem response, or <see langword="null"/> if unset.</param>
    /// <returns><see langword="true"/> if details should be suppressed; <see langword="false"/> otherwise.</returns>
    private static bool ShouldHiddenDetails(IHostEnvironment env, int? status)
    {
        return env.IsProduction() && status >= StatusCodes.Status500InternalServerError;
    }

    /// <summary>
    ///     Returns <see langword="true"/> if the <c>requestId</c> key is already present in <paramref name="extensions"/>,
    ///     preventing duplicate enrichment.
    /// </summary>
    /// <param name="extensions">The problem-details extensions dictionary to check.</param>
    /// <returns><see langword="true"/> if the key exists; <see langword="false"/> otherwise.</returns>
    private static bool ContainsRequestId(IDictionary<string, object?> extensions)
    {
        return extensions.ContainsKey("requestId");
    }

    /// <summary>
    ///     Returns <see langword="true"/> if the <c>traceId</c> key is already present in <paramref name="extensions"/>,
    ///     preventing duplicate enrichment.
    /// </summary>
    /// <param name="extensions">The problem-details extensions dictionary to check.</param>
    /// <returns><see langword="true"/> if the key exists; <see langword="false"/> otherwise.</returns>
    private static bool ContainsTraceId(IDictionary<string, object?> extensions)
    {
        return extensions.ContainsKey("traceId");
    }

    /// <summary>
    ///     Returns <see langword="true"/> if the <c>code</c> key is already present in <paramref name="extensions"/>,
    ///     preventing duplicate enrichment.
    /// </summary>
    /// <param name="extensions">The problem-details extensions dictionary to check.</param>
    /// <returns><see langword="true"/> if the key exists; <see langword="false"/> otherwise.</returns>
    private static bool ContainsCode(IDictionary<string, object?> extensions)
    {
        return extensions.ContainsKey("code");
    }

    /// <summary>
    ///     Returns <see langword="true"/> if the problem details <see cref="ProblemDetails.Instance"/> field has
    ///     not yet been populated, indicating it should be derived from the current request.
    /// </summary>
    /// <param name="ctx">The problem details context to inspect.</param>
    /// <returns><see langword="true"/> if <c>Instance</c> is <see langword="null"/>; <see langword="false"/> otherwise.</returns>
    private static bool NotContainsInstance(ProblemDetailsContext ctx)
    {
        return ctx.ProblemDetails.Instance is null;
    }

    /// <summary>
    ///     Returns <see langword="true"/> if the <c>method</c> key is already present in <paramref name="extensions"/>,
    ///     preventing duplicate enrichment.
    /// </summary>
    /// <param name="extensions">The problem-details extensions dictionary to check.</param>
    /// <returns><see langword="true"/> if the key exists; <see langword="false"/> otherwise.</returns>
    private static bool ContainsMethod(IDictionary<string, object?> extensions)
    {
        return extensions.ContainsKey("method");
    }

    /// <summary>
    ///     Constructs the full request URI string (scheme + host + path) to be set as the
    ///     problem details <c>instance</c> field per RFC 7807.
    /// </summary>
    /// <param name="ctx">The current <see cref="HttpContext"/> containing the request information.</param>
    /// <returns>The absolute URI string identifying the resource that triggered the error.</returns>
    private static string GetInstance(HttpContext ctx)
    {
        return $"{ctx.Request.Scheme}://{ctx.Request.Host}{ctx.Request.Path}";
    }

    extension(ProblemDetailsContext ctx)
    {
        /// <summary>
        ///     Clears the <see cref="ProblemDetails.Detail"/> field when the environment is production
        ///     and the status code is 500 or above, preventing internal error information from leaking to clients.
        /// </summary>
        /// <param name="env">The current host environment used to determine if details should be hidden.</param>
        internal void HiddenDetails(IHostEnvironment env)
        {
            if (ShouldHiddenDetails(env,
                    ctx.ProblemDetails.Status))
                ctx.ProblemDetails.Detail = null;
        }

        /// <summary>
        ///     Adds the HTTP request method (e.g. <c>GET</c>, <c>POST</c>) to the problem-details extensions
        ///     under the key <c>method</c>, if not already present.
        /// </summary>
        /// <exception cref="ArgumentNullException"><paramref name="ctx.ProblemDetails.Extensions" /> is <see langword="null" />.</exception>
        internal void AddMethodField()
        {
            if (ContainsMethod(ctx.ProblemDetails.Extensions)) return;

            var httpMethod = ctx.HttpContext.Request.Method;
            ctx.ProblemDetails.Extensions.TryAdd("method", httpMethod);
        }

        /// <summary>
        ///     Populates the <see cref="ProblemDetails.Instance"/> field with the full request URI
        ///     if it has not already been set.
        /// </summary>
        internal void AddInstanceField()
        {
            if (NotContainsInstance(ctx))
                ctx.ProblemDetails.Instance = GetInstance(ctx.HttpContext);
        }

        /// <summary>
        ///     Adds a <c>requestId</c> extension field sourced from <see cref="Activity.Current"/>.
        ///     <see cref="Activity.Id"/> or <see cref="HttpContext.TraceIdentifier"/> if no active activity exists.
        ///     Has no effect if the key is already present.
        /// </summary>
        /// <exception cref="ArgumentNullException"><paramref name="ctx.ProblemDetails.Extensions" /> is <see langword="null" />.</exception>
        internal void AddRequestIdField()
        {
            if (ContainsRequestId(ctx.ProblemDetails.Extensions)) return;

            var requestId = Activity.Current?.Id ?? ctx.HttpContext.TraceIdentifier;
            ctx.ProblemDetails.Extensions.TryAdd("requestId", requestId);
        }

        /// <summary>
        ///     Adds a <c>traceId</c> extension field from the OpenTelemetry activity associated with the current
        ///     HTTP request via <see cref="IHttpActivityFeature"/>, if available and not already set.
        /// </summary>
        /// <exception cref="ArgumentNullException"><paramref name="ctx.ProblemDetails.Extensions" /> is <see langword="null" />.</exception>
        internal void AddTraceIdField()
        {
            if (ContainsTraceId(ctx.ProblemDetails.Extensions)) return;

            var activityFeature = ctx.HttpContext.Features.Get<IHttpActivityFeature>();
            var activity = activityFeature?.Activity;
            if (activity is not null)
                ctx.ProblemDetails.Extensions.TryAdd("traceId", activity.TraceId.ToString());
        }

        /// <summary>
        ///     Adds a static <c>code</c> extension field with the value <c>BE-BROKEN-ARROW</c> as a
        ///     machine-readable error code for generic or unhandled server errors, if not already present.
        /// </summary>
        /// <exception cref="ArgumentNullException"><paramref name="ctx.ProblemDetails.Extensions" /> is <see langword="null" />.</exception>
        internal void AddCodeField()
        {
            if (ContainsCode(ctx.ProblemDetails.Extensions)) return;

            const string code = "BE-BROKEN-ARROW";
            ctx.ProblemDetails.Extensions.TryAdd("code", code);
        }

        /// <summary>
        ///     Overlays a client-supplied <see cref="ProblemDetails"/> onto the context's response, merging
        ///     status, title, detail, type, instance, and extensions. Client extension entries take precedence
        ///     over existing server entries when keys conflict.
        /// </summary>
        /// <param name="clientProblemDetails">The client-provided problem details to apply over the current response.</param>
        internal void UseHttpContextProblemDetails(ProblemDetails clientProblemDetails)
        {
            ctx.ProblemDetails.Status = clientProblemDetails.Status ?? ctx.ProblemDetails.Status;
            ctx.HttpContext.Response.StatusCode = clientProblemDetails.Status ?? StatusCodes.Status400BadRequest;
            ctx.ProblemDetails.Title = clientProblemDetails.Title ?? ctx.ProblemDetails.Title;
            ctx.ProblemDetails.Detail = clientProblemDetails.Detail ?? ctx.ProblemDetails.Detail;
            ctx.ProblemDetails.Type = clientProblemDetails.Type ?? ctx.ProblemDetails.Type;
            ctx.ProblemDetails.Instance = clientProblemDetails.Instance ?? ctx.ProblemDetails.Instance;
            ctx.ProblemDetails.Extensions = clientProblemDetails.Extensions.Count > 0
                ? MergeExtensions(ctx.ProblemDetails.Extensions, clientProblemDetails.Extensions)
                : ctx.ProblemDetails.Extensions;
        }
    }
}