using System.Text.Json;
using JetBrains.Annotations;
using template.net10.api.Settings.Interfaces;

namespace template.net10.api.Settings.ServiceInstallers;

/// <summary>
///     Service installer that registers SignalR with JSON protocol (camelCase serialization) and
///     enables detailed error messages in non-production environments. Load order: 25.
/// </summary>
[UsedImplicitly]
internal sealed class SignalRInstaller : IServiceInstaller
{
    /// <inheritdoc cref="IServiceInstaller.LoadOrder" />
    public short LoadOrder => 25;

    /// <inheritdoc cref="IServiceInstaller.InstallServiceAsync" />
    /// <exception cref="ArgumentNullException"><paramref name="builder" /> is <see langword="null" />.</exception>
    public Task InstallServiceAsync(WebApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder.Services
            .AddSignalR(hubOptions => hubOptions.EnableDetailedErrors =
                builder.Environment.EnvironmentName is Envs.Development or Envs.Local).AddJsonProtocol(static options =>
                options.PayloadSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase);
        return Task.CompletedTask;
    }
}