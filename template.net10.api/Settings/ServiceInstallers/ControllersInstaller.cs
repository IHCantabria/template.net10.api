using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using template.net10.api.Core.Extensions;
using template.net10.api.Core.Factory;
using template.net10.api.Core.Json;
using template.net10.api.Localize.Resources;
using template.net10.api.Settings.Attributes;
using template.net10.api.Settings.Interfaces;

namespace template.net10.api.Settings.ServiceInstallers;

/// <summary>
///     Service installer that registers MVC controllers with content-negotiation,
///     a custom invalid-model-state response factory, the <see cref="ActionHidingConvention" />,
///     and JSON serialization options (snake_case, enum strings). Load order: 12.
/// </summary>
[UsedImplicitly]
internal sealed class ControllersInstaller : IServiceInstaller
{
    /// <inheritdoc cref="IServiceInstaller.LoadOrder" />
    public short LoadOrder => 12;

    /// <inheritdoc cref="IServiceInstaller.InstallServiceAsync" />
    /// <exception cref="ArgumentNullException"><paramref name="builder" /> is <see langword="null" />.</exception>
    [SuppressMessage(
        "ReSharper",
        "ExceptionNotDocumentedOptional",
        Justification =
            "Potential exceptions originate from underlying implementation details and are not part of the method contract.")]
    public Task InstallServiceAsync(WebApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder.Services.AddControllers(x => ConfigureControllers(x, builder))
            .AddJsonOptions(ConfigureJsonOptions)
            .ConfigureApiBehaviorOptions(static x =>
            {
                x.InvalidModelStateResponseFactory = static context =>
                {
                    var localizer = context.HttpContext.RequestServices
                        .GetRequiredService<IStringLocalizer<ResourceMain>>();
                    var problemDetails =
                        ProblemDetailsFactoryCore.CreateProblemDetailsBadRequestValidationPayload(context.ModelState,
                            localizer);
                    return new ObjectResult(problemDetails) { StatusCode = problemDetails.Status };
                };
            });
        return Task.CompletedTask;
    }

    /// <summary>
    ///     Configures <see cref="MvcOptions" /> to respect the browser Accept header, return 406
    ///     on unacceptable content types, and apply the <see cref="ActionHidingConvention" /> for
    ///     the current environment.
    /// </summary>
    /// <param name="options">The MVC options to configure.</param>
    /// <param name="builder">The web application builder used to read the current environment name.</param>
    private static void ConfigureControllers(MvcOptions options, WebApplicationBuilder builder)
    {
        options.RespectBrowserAcceptHeader = true;
        options.ReturnHttpNotAcceptable = true;
        options.Conventions.Add(new ActionHidingConvention(builder.Environment.EnvironmentName));
    }

    /// <summary>
    ///     Configures JSON serialization to use snake_case property names, a custom
    ///     <see cref="NamingPolicyConverter" />, and <see cref="JsonStringEnumConverter" />.
    /// </summary>
    /// <param name="options">The <see cref="JsonOptions" /> to configure.</param>
    private static void ConfigureJsonOptions(JsonOptions options)
    {
        options.JsonSerializerOptions.AddCoreOptions();
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
        options.JsonSerializerOptions.Converters.Add(new NamingPolicyConverter(new HttpContextAccessor()));
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    }
}