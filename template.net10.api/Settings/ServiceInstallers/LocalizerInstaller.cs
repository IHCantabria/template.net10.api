using JetBrains.Annotations;
using Microsoft.Extensions.Localization;
using template.net10.api.Settings.Interfaces;

namespace template.net10.api.Settings.ServiceInstallers;

/// <summary>
///     Service installer that registers the ASP.NET Core localization services required
///     for <see cref="IStringLocalizer{T}" /> to resolve localized resource strings. Load order: 5.
/// </summary>
[UsedImplicitly]
internal sealed class LocalizerInstaller : IServiceInstaller
{
    /// <inheritdoc cref="IServiceInstaller.LoadOrder" />
    public short LoadOrder => 5;

    /// <inheritdoc cref="IServiceInstaller.InstallServiceAsync" />
    /// <exception cref="ArgumentNullException"><paramref name="builder" /> is <see langword="null" />.</exception>
    public Task InstallServiceAsync(WebApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder.Services.AddLocalization();
        return Task.CompletedTask;
    }
}