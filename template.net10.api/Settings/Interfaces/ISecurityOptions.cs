namespace template.net10.api.Settings.Interfaces;

/// <summary>
///     Marker interface for options classes that represent security configurations.
///     Provides the canonical configuration section key <see cref="Security"/>.
/// </summary>
internal interface ISecurityOptions
{
    /// <summary>
    ///     The configuration section key used to bind security options.
    /// </summary>
    const string Security = nameof(Security);
}