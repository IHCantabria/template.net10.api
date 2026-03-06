using System.Diagnostics;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;

namespace template.net10.api.Settings.Extensions;

/// <summary>
///     ADD DOCUMENTATION
/// </summary>
internal static class ProblemDetailsContextExtensions
{
    /// <summary>
    ///     ADD DOCUMENTATION
    /// </summary>
    private static Dictionary<string, object?> MergeExtensions(IDictionary<string, object?> serverExtensions,
        IDictionary<string, object?> clientExtensions)
    {
        Dictionary<string, object?> mergedExtensions = new(serverExtensions);

        //Should be Serial
        foreach (var entry in clientExtensions) mergedExtensions[entry.Key] = entry.Value;

        return mergedExtensions;
    }

    /// <summary>
    ///     ADD DOCUMENTATION
    /// </summary>
    private static bool ShouldHiddenDetails(IHostEnvironment env, int? status)
    {
        return env.IsProduction() && status >= StatusCodes.Status500InternalServerError;
    }

    /// <summary>
    ///     ADD DOCUMENTATION
    /// </summary>
    private static bool ContainsRequestId(IDictionary<string, object?> extensions)
    {
        return extensions.ContainsKey("requestId");
    }

    /// <summary>
    ///     ADD DOCUMENTATION
    /// </summary>
    private static bool ContainsTraceId(IDictionary<string, object?> extensions)
    {
        return extensions.ContainsKey("traceId");
    }

    /// <summary>
    ///     ADD DOCUMENTATION
    /// </summary>
    private static bool ContainsCode(IDictionary<string, object?> extensions)
    {
        return extensions.ContainsKey("code");
    }

    /// <summary>
    ///     ADD DOCUMENTATION
    /// </summary>
    private static bool NotContainsInstance(ProblemDetailsContext ctx)
    {
        return ctx.ProblemDetails.Instance is null;
    }

    /// <summary>
    ///     ADD DOCUMENTATION
    /// </summary>
    private static bool ContainsMethod(IDictionary<string, object?> extensions)
    {
        return extensions.ContainsKey("method");
    }

    /// <summary>
    ///     ADD DOCUMENTATION
    /// </summary>
    private static string GetInstance(HttpContext ctx)
    {
        return $"{ctx.Request.Scheme}://{ctx.Request.Host}{ctx.Request.Path}";
    }

    extension(ProblemDetailsContext ctx)
    {
        /// <summary>
        ///     ADD DOCUMENTATION
        /// </summary>
        internal void HiddenDetails(IHostEnvironment env)
        {
            if (ShouldHiddenDetails(env,
                    ctx.ProblemDetails.Status))
                ctx.ProblemDetails.Detail = null;
        }

        /// <summary>
        ///     ADD DOCUMENTATION
        /// </summary>
        /// <exception cref="ArgumentNullException"><paramref name="ctx.ProblemDetails.Extensions" /> is <see langword="null" />.</exception>
        internal void AddMethodField()
        {
            if (ContainsMethod(ctx.ProblemDetails.Extensions)) return;

            var httpMethod = ctx.HttpContext.Request.Method;
            ctx.ProblemDetails.Extensions.TryAdd("method", httpMethod);
        }

        /// <summary>
        ///     ADD DOCUMENTATION
        /// </summary>
        internal void AddInstanceField()
        {
            if (NotContainsInstance(ctx))
                ctx.ProblemDetails.Instance = GetInstance(ctx.HttpContext);
        }

        /// <summary>
        ///     ADD DOCUMENTATION
        /// </summary>
        /// <exception cref="ArgumentNullException"><paramref name="ctx.ProblemDetails.Extensions" /> is <see langword="null" />.</exception>
        internal void AddRequestIdField()
        {
            if (ContainsRequestId(ctx.ProblemDetails.Extensions)) return;

            var requestId = Activity.Current?.Id ?? ctx.HttpContext.TraceIdentifier;
            ctx.ProblemDetails.Extensions.TryAdd("requestId", requestId);
        }

        /// <summary>
        ///     ADD DOCUMENTATION
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
        ///     ADD DOCUMENTATION
        /// </summary>
        /// <exception cref="ArgumentNullException"><paramref name="ctx.ProblemDetails.Extensions" /> is <see langword="null" />.</exception>
        internal void AddCodeField()
        {
            if (ContainsCode(ctx.ProblemDetails.Extensions)) return;

            const string code = "BE-BROKEN-ARROW";
            ctx.ProblemDetails.Extensions.TryAdd("code", code);
        }

        /// <summary>
        ///     ADD DOCUMENTATION
        /// </summary>
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