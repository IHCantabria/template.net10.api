using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using template.net10.api.Settings.Extensions;
using template.net10.api.Settings.Handlers;
using template.net10.api.Settings.Interfaces;

namespace template.net10.api.Settings.ServiceInstallers;

/// <summary>
///     Service installer that registers <see cref="GlobalExceptionHandlerControl" /> and configures
///     the RFC 7807 problem-details pipeline, enriching responses with request ID, trace ID,
///     HTTP method, and instance URL. Load order: 2.
/// </summary>
[UsedImplicitly]
internal sealed class ExceptionsControlInstaller : IServiceInstaller
{
    /// <inheritdoc cref="IServiceInstaller.LoadOrder" />
    public short LoadOrder => 2;

    /// <inheritdoc cref="IServiceInstaller.InstallServiceAsync" />
    /// <exception cref="ArgumentNullException"><paramref name="builder" /> is <see langword="null" />.</exception>
    public Task InstallServiceAsync(WebApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder.Services.AddExceptionHandler<GlobalExceptionHandlerControl>();
        builder.Services.AddProblemDetails(setup =>
            setup.CustomizeProblemDetails = ctx => CustomizeProblemDetails(builder.Environment, ctx));
        return Task.CompletedTask;
    }

    /// <summary>
    ///     Enriches the <see cref="ProblemDetailsContext" /> with diagnostic fields (instance, method, request ID,
    ///     trace ID, error code) and applies any client-supplied <see cref="ProblemDetails" /> from the HTTP context.
    ///     In production environments, detail text is suppressed for 5xx responses.
    /// </summary>
    /// <param name="env">The current host environment, used to decide whether to hide error details.</param>
    /// <param name="ctx">The problem details context to customize.</param>
    private static void CustomizeProblemDetails(IHostEnvironment env, ProblemDetailsContext ctx)
    {
        var httpContextProblemDetails =
            ctx.HttpContext.Features.Get<ProblemDetails>();
        ctx.AddInstanceField();
        ctx.AddMethodField();
        ctx.AddRequestIdField();
        ctx.AddTraceIdField();
        ctx.AddCodeField();
        if (httpContextProblemDetails is not null) ctx.UseHttpContextProblemDetails(httpContextProblemDetails);

        //Details Error Removed for server errors in Production
        ctx.HiddenDetails(env);
    }
}