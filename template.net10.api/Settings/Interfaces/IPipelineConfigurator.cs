namespace template.net10.api.Settings.Interfaces;

/// <summary>
///     Defines a pipeline configuration step executed during application startup after the DI container is built.
///     Implementations are discovered by convention and invoked ordered by <see cref="LoadOrder"/>.
/// </summary>
internal interface IPipelineConfigurator
{
    /// <summary>
    ///     Determines the execution order of this configurator relative to others. Lower values run first.
    /// </summary>
    short LoadOrder { get; }

    /// <summary>
    ///     Configures ASP.NET Core middleware and routing on the provided <paramref name="app"/>.
    /// </summary>
    /// <param name="app">The built <see cref="WebApplication"/> to configure.</param>
    /// <returns>A <see cref="Task"/> that completes when configuration is finished.</returns>
    Task ConfigurePipelineAsync(WebApplication app);
}