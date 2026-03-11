using System.Text.Json;
using JetBrains.Annotations;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.IdentityModel.Protocols.Configuration;
using Serilog;
using template.net10.api.Core;
using template.net10.api.Core.Extensions;
using template.net10.api.Core.Logger;
using template.net10.api.Settings.Interfaces;
using Path = System.IO.Path;

namespace template.net10.api.Settings.ServiceInstallers;

/// <summary>
///     Service installer that configures Serilog as the application logger, clears default ASP.NET Core
///     logging providers, sets up HTTP request/response property logging, and reads the version
///     from <c>package.json</c> to stamp log output. Load order: 1.
/// </summary>
[UsedImplicitly]
internal sealed class LoggerInstaller : IServiceInstaller
{
    /// <summary>
    ///     JSON deserializer options used when reading the <c>package.json</c> file.
    /// </summary>
    private static readonly JsonSerializerOptions Options = new JsonSerializerOptions().AddCoreOptions();

    /// <inheritdoc cref="IServiceInstaller.LoadOrder" />
    public short LoadOrder => 1;

    /// <inheritdoc cref="IServiceInstaller.InstallServiceAsync" />
    /// <exception cref="ArgumentNullException"><paramref name="builder" /> is <see langword="null" />.</exception>
    /// <exception cref="InvalidConfigurationException">Thrown when the OpenTelemetry log endpoint is unreachable or the Serilog/OpenTelemetry configuration is missing or invalid.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Given depth must be positive.</exception>
    /// <exception cref="InvalidOperationException">When the logger is already created</exception>
    public async Task InstallServiceAsync(WebApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        var config = builder.Configuration;
        //Remove microsoft logger instances
        builder.Logging.ClearProviders();
        //Define log min level, this is the fallback value if this value is not defined in the appsettings file.

        builder.Logging.SetMinimumLevel(LogLevel.Trace);
        //Define Serilog like default logger.

        builder.Services.AddSerilog();

        var version = await ReadPackageJsonVersionAsync().ConfigureAwait(false);

        SerilogLoggersFactory.RealApplicationLogFactory(config, builder.Environment.EnvironmentName, version);

        builder.Services.AddHttpLogging(static options =>
            options.LoggingFields = HttpLoggingFields.RequestPropertiesAndHeaders |
                                    HttpLoggingFields.ResponsePropertiesAndHeaders);
    }

    /// <summary>
    ///     Reads the <c>version</c> field from the project's <c>package.json</c> file in the
    ///     current working directory. Returns an empty string if the file does not exist or
    ///     the property is absent.
    /// </summary>
    /// <returns>The version string, or <see cref="string.Empty"/> if unavailable.</returns>
    private static async Task<string> ReadPackageJsonVersionAsync()
    {
        var ct = CancellationToken.None;
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), CoreConstants.PackageJsonFile);
        if (!File.Exists(filePath)) return string.Empty;

        using var reader = new StreamReader(filePath);
        var jsonContent = await reader.ReadToEndAsync(ct).ConfigureAwait(false);

        var jsonObject = JsonSerializer.Deserialize<JsonElement>(jsonContent, Options);

        return (jsonObject.TryGetProperty("version", out var version) ? version.GetString() : string.Empty) ??
               string.Empty;
    }
}