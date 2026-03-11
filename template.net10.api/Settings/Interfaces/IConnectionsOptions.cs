namespace template.net10.api.Settings.Interfaces;

/// <summary>
///     Marker interface for options classes that represent connection configurations.
///     Provides the canonical configuration section key <see cref="Connections"/>.
/// </summary>
internal interface IConnectionsOptions
{
    /// <summary>
    ///     The configuration section key used to bind connection options.
    /// </summary>
    const string Connections = nameof(Connections);
}