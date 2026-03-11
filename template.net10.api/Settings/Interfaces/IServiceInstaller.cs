namespace template.net10.api.Settings.Interfaces;

/// <summary>
///     Defines a service registration step executed during application startup before the DI container is built.
///     Implementations are discovered by convention and invoked ordered by <see cref="LoadOrder"/>.
/// </summary>
internal interface IServiceInstaller
{
    /// <summary>
    ///     Determines the execution order of this installer relative to others. Lower values run first.
    /// </summary>
    short LoadOrder { get; }

    /// <summary>
    ///     Registers services into the DI container via the provided <paramref name="builder"/>.
    /// </summary>
    /// <param name="builder">The <see cref="WebApplicationBuilder"/> used to register services.</param>
    /// <returns>A <see cref="Task"/> that completes when registration is finished.</returns>
    Task InstallServiceAsync(WebApplicationBuilder builder);
}