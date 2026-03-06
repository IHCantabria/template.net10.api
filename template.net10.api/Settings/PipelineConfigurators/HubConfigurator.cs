using JetBrains.Annotations;
using template.net10.api.Hubs.Extensions;
using template.net10.api.Settings.Interfaces;

namespace template.net10.api.Settings.PipelineConfigurators;

/// <summary>
///     ADD DOCUMENTATION
/// </summary>
[UsedImplicitly]
internal sealed class HubsConfigurator : IPipelineConfigurator
{
    /// <inheritdoc cref="IPipelineConfigurator.LoadOrder" />
    public short LoadOrder => 5;


    /// <inheritdoc cref="IPipelineConfigurator.ConfigurePipelineAsync" />
    public Task ConfigurePipelineAsync(WebApplication app)
    {
        app.ConfigureHubs();

        return Task.CompletedTask;
    }
}