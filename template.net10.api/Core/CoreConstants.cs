namespace template.net10.api.Core;

/// <summary>
///     Contains core application constants used across the application.
/// </summary>
internal static class CoreConstants
{
    /// <summary>
    ///     The file name for the package.json configuration file.
    /// </summary>
    internal const string PackageJsonFile = "package.json";

    /// <summary>
    ///     The prefix used for backend API error codes.
    /// </summary>
    internal const string ApiErrorCodesPrefix = "BE-";

    /// <summary>
    ///     Gets a unique identifier generated once per application instance.
    /// </summary>
    internal static Guid GuidInstance { get; } = Guid.NewGuid();
}