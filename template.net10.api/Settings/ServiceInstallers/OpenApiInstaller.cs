using JetBrains.Annotations;
using template.net10.api.Settings.Interfaces;

namespace template.net10.api.Settings.ServiceInstallers;

/// <summary>
///     Service installer that registers the ASP.NET Core minimal API endpoint explorer required
///     by Swagger/OpenAPI to discover endpoint metadata. Load order: 16.
/// </summary>
[UsedImplicitly]
internal sealed class OpenApiInstaller : IServiceInstaller
{
    /// <inheritdoc cref="IServiceInstaller.LoadOrder" />
    public short LoadOrder => 16;

    /// <inheritdoc cref="IServiceInstaller.InstallServiceAsync" />
    /// <exception cref="ArgumentNullException"><paramref name="builder" /> is <see langword="null" />.</exception>
    public Task InstallServiceAsync(WebApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        return Task.CompletedTask;
    }
}