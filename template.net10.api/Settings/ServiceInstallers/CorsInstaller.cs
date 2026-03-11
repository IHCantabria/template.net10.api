using JetBrains.Annotations;
using Microsoft.IdentityModel.Protocols.Configuration;
using template.net10.api.Core;
using template.net10.api.Settings.Interfaces;
using template.net10.api.Settings.Options;

namespace template.net10.api.Settings.ServiceInstallers;

/// <summary>
///     Service installer that registers a named CORS policy from configuration, allowing the
///     configured origins, GET/POST/PUT/DELETE/OPTIONS methods, any header, and credentials. Load order: 15.
/// </summary>
[UsedImplicitly]
internal sealed class CorsInstaller : IServiceInstaller
{
    /// <summary>
    ///     The set of HTTP methods allowed by the registered CORS policy.
    /// </summary>
    private static readonly string[] AllowedMethods = ["GET", "POST", "PUT", "DELETE", "OPTIONS"];

    /// <inheritdoc cref="IServiceInstaller.LoadOrder" />
    public short LoadOrder => 15;

    /// <inheritdoc cref="IServiceInstaller.InstallServiceAsync" />
    /// <exception cref="ArgumentNullException"><paramref name="builder" /> is <see langword="null" />.</exception>
    /// <exception cref="InvalidConfigurationException">
    ///     The Cors configuration in the appsettings file is incorrect.
    /// </exception>
    public Task InstallServiceAsync(WebApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        // Configure strongly typed options objects.
        var config = builder.Configuration;
        var corsOptions = config.GetSection(CorsOptions.Cors).Get<CorsOptions>();
        OptionsValidator.ValidateCorsOptions(corsOptions);
        AddCors(builder, corsOptions);
        return Task.CompletedTask;
    }

    /// <summary>
    ///     Registers the CORS policy from <paramref name="apiOptions"/> with the configured origins,
    ///     <see cref="AllowedMethods"/>, any header, exposed ETags, and credentials support.
    ///     Does nothing if <paramref name="apiOptions"/> is <see langword="null"/>.
    /// </summary>
    /// <param name="builder">The web application builder to register the policy on.</param>
    /// <param name="apiOptions">The resolved CORS options, or <see langword="null"/> if configuration is missing.</param>
    private static void AddCors(WebApplicationBuilder builder, CorsOptions? apiOptions)
    {
        if (apiOptions is null) return;

        builder.Services.AddCors(o => o.AddPolicy(apiOptions.CorsPolicy,
            policyBuilder =>
            {
                policyBuilder.AllowAnyHeader().WithOrigins([..apiOptions.ArrayAllowedOrigins])
                    .WithExposedHeaders("Content-Disposition", "ETag").WithMethods(AllowedMethods).AllowCredentials();
            }));
    }
}