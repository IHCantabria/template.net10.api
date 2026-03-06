namespace template.net10.api.Settings.Interfaces;

/// <summary>
///     ADD DOCUMENTATION
/// </summary>
internal interface IPipelineConfigurator
{
    /// <summary>
    ///     ADD DOCUMENTATION
    /// </summary>
    short LoadOrder { get; }

    /// <summary>
    ///     ADD DOCUMENTATION
    /// </summary>
    Task ConfigurePipelineAsync(WebApplication app);
}