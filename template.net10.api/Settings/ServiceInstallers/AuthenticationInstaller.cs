using System.Text;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Protocols.Configuration;
using Microsoft.IdentityModel.Tokens;
using template.net10.api.Core;
using template.net10.api.Settings.Events;
using template.net10.api.Settings.Interfaces;
using template.net10.api.Settings.Options;

namespace template.net10.api.Settings.ServiceInstallers;

/// <summary>
///     Service installer that configures JWT Bearer authentication, token validation parameters,
///     and IdentityModel PII logging for non-production environments. Load order: 18.
/// </summary>
[UsedImplicitly]
internal sealed class AuthenticationInstaller : IServiceInstaller
{
    /// <summary>
    ///     Cached JWT options resolved from configuration, populated during <see cref="InstallServiceAsync"/>
    ///     and consumed by <see cref="ConfigureJwtBearer"/>.
    /// </summary>
    private JwtOptions? _config;

    /// <inheritdoc cref="IServiceInstaller.LoadOrder" />
    public short LoadOrder => 18;

    /// <inheritdoc cref="IServiceInstaller.InstallServiceAsync" />
    /// <exception cref="ArgumentNullException"><paramref name="builder" /> is <see langword="null" />.</exception>
    public Task InstallServiceAsync(WebApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        RegisterServices(builder.Services);
        var (jwtOptions, environment) = LoadAndValidateOptions(builder);

        ConfigureIdentityModelLogging(environment);
        ConfigureAuthentication(builder.Services, environment);

        _config = jwtOptions;
        return Task.CompletedTask;
    }

    /// <summary>
    ///     Registers <see cref="AppJwtBearerEvents"/> as a scoped service so it can be resolved
    ///     by the JWT Bearer middleware during request processing.
    /// </summary>
    /// <param name="services">The application service collection.</param>
    private static void RegisterServices(IServiceCollection services)
    {
        services.AddScoped<AppJwtBearerEvents>();
    }

    /// <summary>
    ///     Reads JWT options from the <c>Security:Jwt</c> configuration section, validates them,
    ///     and returns them together with the current host environment.
    /// </summary>
    /// <param name="builder">The web application builder providing configuration and environment.</param>
    /// <returns>A tuple containing the validated <see cref="JwtOptions"/> and the <see cref="IWebHostEnvironment"/>.</returns>
    private static (JwtOptions? jwtOptions, IWebHostEnvironment Environment) LoadAndValidateOptions(
        WebApplicationBuilder builder)
    {
        var jwtOptions = builder.Configuration
            .GetSection(JwtOptions.Jwt)
            .Get<JwtOptions>();

        OptionsValidator.ValidateJwtOptions(jwtOptions);

        return (jwtOptions, builder.Environment);
    }

    /// <summary>
    ///     Enables full PII logging and complete security artifact logging in IdentityModel
    ///     when running in Development, Local, or Test environments.
    /// </summary>
    /// <param name="environment">The host environment used to determine whether PII logging should be activated.</param>
    private static void ConfigureIdentityModelLogging(IHostEnvironment environment)
    {
        if (environment.EnvironmentName is not (Envs.Development or Envs.Local or Envs.Test))
            return;

        IdentityModelEventSource.ShowPII = true;
        IdentityModelEventSource.LogCompleteSecurityArtifact = true;
    }

    /// <summary>
    ///     Registers JWT Bearer as the default authentication and challenge scheme
    ///     and delegates JWT configuration to <see cref="ConfigureJwtBearer"/>.
    /// </summary>
    /// <param name="services">The application service collection.</param>
    /// <param name="environment">The host environment, passed to JWT bearer options for HTTPS enforcement.</param>
    private void ConfigureAuthentication(IServiceCollection services, IHostEnvironment environment)
    {
        services.AddAuthentication(static x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x => ConfigureJwtBearer(x, environment));
    }

    /// <summary>
    ///     Configures JWT Bearer token validation parameters from <see cref="_config"/>, registers
    ///     <see cref="AppJwtBearerEvents"/> as the events type, and disables HTTPS metadata
    ///     enforcement in non-production environments.
    /// </summary>
    /// <param name="options">The JWT Bearer options to configure.</param>
    /// <param name="environment">The host environment used to adjust security settings per environment.</param>
    private void ConfigureJwtBearer(JwtBearerOptions options, IHostEnvironment environment)
    {
        if (_config is null)
            throw new InvalidConfigurationException("The Jwt configuration in the appsettings file is incorrect.");

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            ValidIssuer = _config.Issuer,
            IssuerSigningKeys =
                [new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.Secret))],
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidAudience = _config.Audience,
            ValidateLifetime = true,
            LifetimeValidator = LifetimeValidator
        };
        if (environment.EnvironmentName is Envs.Development or Envs.Local or Envs.Test)
            options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.IncludeErrorDetails = true;
        options.EventsType = typeof(AppJwtBearerEvents);
    }

    /// <summary>
    ///     Validates that a JWT token has not expired by comparing its expiry date against the current time.
    /// </summary>
    /// <param name="notBefore">The earliest valid date for the token, or <see langword="null"/> if not set.</param>
    /// <param name="expires">The expiry date of the token, or <see langword="null"/> if not set.</param>
    /// <param name="securityToken">The security token being validated (unused).</param>
    /// <param name="validationParameters">The token validation parameters (unused).</param>
    /// <returns><see langword="true"/> if the token has not yet expired; <see langword="false"/> otherwise.</returns>
    private static bool LifetimeValidator(DateTime? notBefore,
        DateTime? expires,
        SecurityToken securityToken,
        TokenValidationParameters validationParameters)
    {
        return expires != null && expires > DateTime.Now;
    }
}