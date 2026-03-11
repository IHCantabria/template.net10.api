using System.Reflection;
using JetBrains.Annotations;
using Microsoft.IdentityModel.Protocols.Configuration;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;
using template.net10.api.Core;
using template.net10.api.Settings.Filters;
using template.net10.api.Settings.Interfaces;
using template.net10.api.Settings.Options;

namespace template.net10.api.Settings.ServiceInstallers;

/// <summary>
///     Service installer that registers and configures the Swashbuckle Swagger generator,
///     including the OpenAPI document, JWT security scheme, custom operation/document filters,
///     XML comments, and server URL. Load order: 17.
/// </summary>
// ReSharper disable once ConditionalAnnotation
[UsedImplicitly]
internal sealed class SwaggerInstaller : IServiceInstaller
{
    /// <inheritdoc cref="IServiceInstaller.LoadOrder" />
    public short LoadOrder => 17;

    /// <inheritdoc cref="IServiceInstaller.InstallServiceAsync" />
    /// <exception cref="ArgumentNullException"><paramref name="builder" /> is <see langword="null" />.</exception>
    /// <exception cref="InvalidConfigurationException">
    ///     The Swagger configuration in the appsettings file is incorrect.
    ///     The Swagger Security configuration in the appsettings file is incorrect.
    /// </exception>
    public Task InstallServiceAsync(WebApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        var config = builder.Configuration;
        // Configure strongly typed options objects
        var swaggerOptions = config.GetSection(SwaggerOptions.Swagger).Get<SwaggerOptions>();
        OptionsValidator.ValidateSwaggerOptions(swaggerOptions);
        if (swaggerOptions is null)
            throw new InvalidConfigurationException(
                "The Swagger configuration in the appsettings file is incorrect.");
        // Configure strongly typed options objects
        var swaggerSecurityOptions =
            config.GetSection(SwaggerSecurityOptions.SwaggerSecurity).Get<SwaggerSecurityOptions>();
        OptionsValidator.ValidateSwaggerSecurityOptions(swaggerSecurityOptions);
        var version = config.Get<ProjectOptions>()?.Version ?? "";

        AddSwaggerGen(builder, swaggerOptions, swaggerSecurityOptions, version);
        return Task.CompletedTask;
    }

    /// <summary>
    ///     Registers <c>AddSwaggerGen</c> on the service collection, delegating full
    ///     Swashbuckle configuration to <see cref="ConfigureSwaggerGen" />.
    /// </summary>
    /// <param name="builder">The web application builder.</param>
    /// <param name="swaggerOptions">Resolved Swagger UI and document options.</param>
    /// <param name="swaggerSecurityOptions">
    ///     Resolved JWT security scheme options, or <see langword="null" /> if not
    ///     configured.
    /// </param>
    /// <param name="version">The API version string read from project configuration.</param>
    private static void AddSwaggerGen(WebApplicationBuilder builder, SwaggerOptions swaggerOptions,
        SwaggerSecurityOptions? swaggerSecurityOptions, string version)
    {
        // Register the swagger generator, defining 1 or more swagger documents
        builder.Services.AddSwaggerGen(c => ConfigureSwaggerGen(c, swaggerOptions, swaggerSecurityOptions, version));
    }

    /// <summary>
    ///     Applies the full Swashbuckle <see cref="SwaggerGenOptions" /> configuration: adds the OpenAPI document,
    ///     optional JWT security definition, custom filters, inline enum definitions, and XML documentation.
    /// </summary>
    /// <param name="c">The Swashbuckle options builder to configure.</param>
    /// <param name="swaggerOptions">Resolved Swagger options used for document metadata and endpoints.</param>
    /// <param name="swaggerSecurityOptions">Optional JWT security scheme configuration.</param>
    /// <param name="version">The API version string to embed in the document.</param>
    private static void ConfigureSwaggerGen(SwaggerGenOptions c, SwaggerOptions swaggerOptions,
        SwaggerSecurityOptions? swaggerSecurityOptions, string version)
    {
        AddSwaggerDoc(c, swaggerOptions, version);

        if (swaggerSecurityOptions is not null)
            AddSwaggerSecurity(c, swaggerSecurityOptions);
        c.OperationFilter<AuthOperationFilter>();
        c.CustomSchemaIds(static type => type.ToString()); // Avoid problems with nested models
        c.IgnoreObsoleteActions(); // Ignoring obsolete methods to improve performance
        c.IgnoreObsoleteProperties();
        c.OperationFilter<DocumentationOperationFilter>();
        c.UseInlineDefinitionsForEnums();
        c.DocumentFilter<RemoveSystemTypesDocumentFilter>();
        AddSwaggeConfig(c, swaggerOptions);
    }

    /// <summary>
    ///     Creates the OpenAPI document with title, version, long description, and license information
    ///     from <paramref name="swaggerOptions" />.
    /// </summary>
    /// <param name="swagger">The Swashbuckle options builder.</param>
    /// <param name="swaggerOptions">The Swagger configuration options.</param>
    /// <param name="version">The API version string to embed in the OpenAPI document.</param>
    private static void AddSwaggerDoc(SwaggerGenOptions swagger, SwaggerOptions swaggerOptions, string version)
    {
        swagger.SwaggerDoc(swaggerOptions.VersionSwagger, new OpenApiInfo
        {
            Title = swaggerOptions.Title,
            Version = version,
            Description = swaggerOptions.LongDescription,
            License = new OpenApiLicense
            {
                Name = swaggerOptions.License
            }
        });
    }

    /// <summary>
    ///     Adds the Bearer JWT security scheme definition to the OpenAPI document
    ///     using settings from <paramref name="swaggerSecurityOptions" />.
    /// </summary>
    /// <param name="swagger">The Swashbuckle options builder.</param>
    /// <param name="swaggerSecurityOptions">The JWT security scheme configuration.</param>
    private static void AddSwaggerSecurity(SwaggerGenOptions swagger, SwaggerSecurityOptions swaggerSecurityOptions)
    {
        var securityScheme = new OpenApiSecurityScheme
        {
            Description = swaggerSecurityOptions.Description,
            Name = swaggerSecurityOptions.Name,
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Scheme = swaggerSecurityOptions.SchemeName,
            BearerFormat = swaggerSecurityOptions.BearerFormat
        };
        swagger.AddSecurityDefinition(swaggerSecurityOptions.SchemeId, securityScheme);
    }

    /// <summary>
    ///     Adds the API server URL, enables XML comment inclusion from the executing assembly,
    ///     and activates Swashbuckle annotation support.
    /// </summary>
    /// <param name="swagger">The Swashbuckle options builder.</param>
    /// <param name="swaggerOptions">The Swagger options providing the server URL.</param>
    private static void AddSwaggeConfig(SwaggerGenOptions swagger, SwaggerOptions swaggerOptions)
    {
        //Add Server API url
        swagger.AddServer(new OpenApiServer
        {
            Url = swaggerOptions.ServerUrl.ToString()
        });
        // XML documentation
        swagger.IncludeXmlComments(Assembly.GetExecutingAssembly());
        // Add swagger UI annotations
        swagger.EnableAnnotations();
    }
}