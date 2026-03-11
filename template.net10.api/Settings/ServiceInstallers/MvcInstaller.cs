using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using template.net10.api.Settings.Interfaces;

namespace template.net10.api.Settings.ServiceInstallers;

/// <summary>
///     Service installer that configures global MVC options: disables async-suffix action name stripping
///     and sets <see cref="CultureInfo.InvariantCulture" /> as the default thread culture. Load order: 6.
/// </summary>
[UsedImplicitly]
internal sealed class MvcInstaller : IServiceInstaller
{
    /// <inheritdoc cref="IServiceInstaller.LoadOrder" />
    public short LoadOrder => 6;

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
        builder.Services.Configure<MvcOptions>(static options =>
        {
            options.SuppressAsyncSuffixInActionNames = false;

            var culture = CultureInfo.InvariantCulture;

            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
        });
        return Task.CompletedTask;
    }
}