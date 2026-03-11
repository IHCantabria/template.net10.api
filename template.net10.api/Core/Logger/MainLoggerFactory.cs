using System.Diagnostics.CodeAnalysis;
using Microsoft.IdentityModel.Protocols.Configuration;
using Serilog;
using template.net10.api.Core.Logger.Extensions;
using template.net10.api.Core.Logger.Sinks;

namespace template.net10.api.Core.Logger;

/// <summary>
///     Factory for creating and configuring Serilog logger instances at different application lifecycle stages.
/// </summary>
internal static class SerilogLoggersFactory
{
    /// <summary>
    ///     Shared in-memory sink used to buffer log events until the real application logger is configured.
    /// </summary>
    private static readonly MemorySink MemorySink = new();

    /// <summary>
    ///     Creates the initial bootstrap logger with enrichment, minimum levels, and a temporary memory sink.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">Given depth must be positive.</exception>
    [SuppressMessage(
        "ReSharper",
        "ExceptionNotDocumentedOptional",
        Justification =
            "Potential exceptions originate from underlying implementation details and are not part of the method contract.")]
    internal static void MainLogFactory()
    {
        Log.Logger = new LoggerConfiguration()
            .EnrichLog()
            .ConfigureMinLevels()
            .WriteTo.Async(static a => a.Sink(MemorySink)) // Temporal memory sink
            .CreateBootstrapLogger();
    }

    /// <summary>
    ///     Creates the real application logger with full configuration, sinks, and flushes buffered memory sink events.
    /// </summary>
    /// <param name="builderConfiguration">The application configuration manager.</param>
    /// <param name="envName">The current environment name.</param>
    /// <param name="version">The application version.</param>
    /// <exception cref="ArgumentOutOfRangeException">Given depth must be positive.</exception>
    /// <exception cref="InvalidConfigurationException">Thrown when a required Serilog sink or configuration section is missing or invalid in the provided <paramref name="builderConfiguration"/>.</exception>
    /// <exception cref="InvalidOperationException">When the logger is already created</exception>
    [SuppressMessage(
        "ReSharper",
        "ExceptionNotDocumentedOptional",
        Justification =
            "Potential exceptions originate from underlying implementation details and are not part of the method contract.")]
    internal static void RealApplicationLogFactory(ConfigurationManager builderConfiguration,
        string envName, string version)
    {
        Log.Logger = new LoggerConfiguration()
            .EnrichLog()
            .ConfigureMinLevels()
            .ReadFrom.Configuration(builderConfiguration)
            .ConfigureSinks(builderConfiguration, envName, version)
            .CreateLogger();

        MemorySink.FlushToLogger(Log.Logger);
    }

    /// <summary>
    ///     Creates a fallback logger with local file sink when the main logger configuration fails, and flushes buffered
    ///     events.
    /// </summary>
    /// <exception cref="InvalidOperationException">When the logger is already created</exception>
    [SuppressMessage(
        "ReSharper",
        "ExceptionNotDocumentedOptional",
        Justification =
            "Potential exceptions originate from underlying implementation details and are not part of the method contract.")]
    internal static void FallbackLogFactory()
    {
        Log.Logger = new LoggerConfiguration()
            .EnrichLog()
            .ConfigureMinLevels()
            .ConfigureSinkLocal()
            .CreateLogger();

        MemorySink.FlushToLogger(Log.Logger);
        MainLoggerMethods.LogInitFallBack();
    }
}