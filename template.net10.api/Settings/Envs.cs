namespace template.net10.api.Settings;

/// <summary>
///     Defines the environment name constants used to identify the active deployment environment.
/// </summary>
internal static class Envs
{
    /// <summary>
    ///     Environment name for local development, used when running the application on a developer's machine.
    /// </summary>
    internal const string Local = "local";

    /// <summary>
    ///     Environment name for local testing, automated testing and CI pipelines.
    /// </summary>
    internal const string Test = "test";

    /// <summary>
    ///     Environment name for the development/integration server.
    /// </summary>
    internal const string Development = "dev";

    /// <summary>
    ///     Environment name for the pre-production (staging) server.
    /// </summary>
    internal const string PreProduction = "pre";

    /// <summary>
    ///     Environment name for the production server.
    /// </summary>
    internal const string Production = "prod";
}