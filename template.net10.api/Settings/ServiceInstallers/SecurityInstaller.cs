using JetBrains.Annotations;
using template.net10.api.Settings.Interfaces;

namespace template.net10.api.Settings.ServiceInstallers;

/// <summary>
///     Service installer that adds HSTS (HTTP Strict Transport Security) with a one-year max age,
///     preload flag, and include-subdomains directive. Load order: 21.
/// </summary>
[UsedImplicitly]
internal sealed class SecurityInstaller : IServiceInstaller
{
    /// <summary>
    ///     The HSTS max-age duration set to one year (365 days), as recommended by the HSTS preload list.
    /// </summary>
    private readonly TimeSpan _maxAge = TimeSpan.FromDays(365);

    /// <inheritdoc cref="IServiceInstaller.LoadOrder" />
    public short LoadOrder => 21;

    /// <inheritdoc cref="IServiceInstaller.InstallServiceAsync" />
    /// <exception cref="ArgumentNullException"><paramref name="builder" /> is <see langword="null" />.</exception>
    public Task InstallServiceAsync(WebApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder.Services.AddHsts(options =>
        {
            options.Preload = true;
            options.IncludeSubDomains = true;
            options.MaxAge = _maxAge;
        });

        return Task.CompletedTask;
    }
}