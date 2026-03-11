using HotChocolate.AspNetCore;
using JetBrains.Annotations;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerUI;
using template.net10.api.Settings.Interfaces;
using template.net10.api.Settings.Options;

namespace template.net10.api.Settings.PipelineConfigurators;

/// <summary>
///     Pipeline configurator that registers the API documentation and GraphQL IDE middleware:
///     Swagger UI, ReDoc, and Banana Cake Pop. Load order: 2.
/// </summary>
[UsedImplicitly]
internal sealed class UiConfigurator : IPipelineConfigurator
{
    /// <inheritdoc cref="IPipelineConfigurator.LoadOrder" />
    public short LoadOrder => 2;

    /// <inheritdoc cref="IPipelineConfigurator.ConfigurePipelineAsync" />
    /// <exception cref="ArgumentNullException"><paramref name="app" /> is <see langword="null" />.</exception>
    public Task ConfigurePipelineAsync(WebApplication app)
    {
        ArgumentNullException.ThrowIfNull(app);
        UseSwagger(app);
        UseReDoc(app);
        UseBananaCakePop(app);

        return Task.CompletedTask;
    }

    /// <summary>
    ///     Registers Swagger JSON endpoint and Swagger UI middleware using values from <see cref="SwaggerOptions" />.
    /// </summary>
    /// <param name="app">The web application to configure.</param>
    private static void UseSwagger(WebApplication app)
    {
        //Get swagger configuration from service with strongly typed options object.
        var swaggerConfiguration = app.Services.GetRequiredService<IOptions<SwaggerOptions>>().Value;
        // Enable middleware to serve generated Swagger as a JSON endpoint.
        app.UseSwagger(option => option.RouteTemplate = swaggerConfiguration.JsonRoute);
        // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
        app.UseSwaggerUI(option =>
        {
            option.SwaggerEndpoint(swaggerConfiguration.UiEndpoint, swaggerConfiguration.ShortDescription);
            option.DocumentTitle = swaggerConfiguration.DocumentTitle;
            option.DocExpansion(DocExpansion.List);
            //Turns off syntax highlight which causing performance issues...
            option.ConfigObject.AdditionalItems.Add("syntaxHighlight", false);
        });
    }

    /// <summary>
    ///     Registers the ReDoc documentation UI middleware using values from <see cref="ReDocOptions" />.
    /// </summary>
    /// <param name="app">The web application to configure.</param>
    private static void UseReDoc(WebApplication app)
    {
        //Get ReDoc configuration from service with strongly typed options object.
        var reDocConfiguration = app.Services.GetRequiredService<IOptions<ReDocOptions>>().Value;

        // Enable middleware to serve generated ReDoc documentation.
        app.UseReDoc(c =>
        {
            c.RoutePrefix = reDocConfiguration.RoutePrefix;
            c.DocumentTitle = reDocConfiguration.DocumentTitle;
            c.SpecUrl = reDocConfiguration.SpecUrl.ToString();
        });
    }

    /// <summary>
    ///     Maps the GraphQL endpoint and activates the Banana Cake Pop IDE tool for interactive query exploration.
    /// </summary>
    /// <param name="app">The endpoint route builder to map the GraphQL route on.</param>
    private static void UseBananaCakePop(IEndpointRouteBuilder app)
    {
        // Enable middleware to serve the GraphQL IDE.
        app.MapGraphQL().WithOptions(
            new GraphQLServerOptions
            {
                Tool =
                {
                    Enable = true
                }
            });
    }
}