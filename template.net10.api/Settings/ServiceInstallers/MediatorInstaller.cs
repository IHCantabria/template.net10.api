using JetBrains.Annotations;
using template.net10.api.Settings.Extensions;
using template.net10.api.Settings.Interfaces;

namespace template.net10.api.Settings.ServiceInstallers;

/// <summary>
///     Service installer that registers MediatR with all handlers from the application assembly
///     and configures the pipeline behaviors for logging, validation, and post-processing. Load order: 14.
/// </summary>
[UsedImplicitly]
internal sealed class MediatorInstaller : IServiceInstaller
{
    /// <inheritdoc cref="IServiceInstaller.LoadOrder" />
    public short LoadOrder => 14;

    /// <inheritdoc cref="IServiceInstaller.InstallServiceAsync" />
    /// <exception cref="ArgumentNullException"><paramref name="builder" /> is <see langword="null" />.</exception>
    public Task InstallServiceAsync(WebApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder.Services.AddMediatR(static c =>
        {
            c.RegisterServicesFromAssemblyContaining<Program>();
            c.AddBehaviours();
            c.AddPostProcesses();
        });
        return Task.CompletedTask;
    }
}