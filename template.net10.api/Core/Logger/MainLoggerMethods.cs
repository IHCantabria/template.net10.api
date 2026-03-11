using System.Diagnostics.CodeAnalysis;
using Serilog;
using template.net10.api.Business;

namespace template.net10.api.Core.Logger;

/// <summary>
///     Centralizes all main application lifecycle logging methods using Serilog.
/// </summary>
[SuppressMessage("ReSharper", "ClassTooBig",
    Justification = "The class intentionally centralizes all main logging methods in a single location.")]
internal static class MainLoggerMethods
{
    /// <summary>
    ///     Logs that the main service is starting.
    /// </summary>
    internal static void LogStartingMainService()
    {
        Log.Information(MainLoggerMessageDefinitions.StartingMainService, BusinessConstants.ApiName);
    }

    /// <summary>
    ///     Logs that the application builder is starting.
    /// </summary>
    internal static void LogBuilderStarting()
    {
        Log.Information(MainLoggerMessageDefinitions.BuilderStarting, BusinessConstants.ApiName);
    }

    /// <summary>
    ///     Logs that the application builder has started.
    /// </summary>
    internal static void LogBuilderStarted()
    {
        Log.Information(MainLoggerMessageDefinitions.BuilderStarted, BusinessConstants.ApiName);
    }

    /// <summary>
    ///     Logs that services are being installed in the application builder.
    /// </summary>
    internal static void LogInstallingServices()
    {
        Log.Information(MainLoggerMessageDefinitions.InstallingServices, BusinessConstants.ApiName);
    }

    /// <summary>
    ///     Logs that all services have been installed in the application builder.
    /// </summary>
    internal static void LogInstalledServices()
    {
        Log.Information(MainLoggerMessageDefinitions.InstalledServices, BusinessConstants.ApiName);
    }

    /// <summary>
    ///     Logs that the application container is being built.
    /// </summary>
    internal static void LogContainerBuilding()
    {
        Log.Information(MainLoggerMessageDefinitions.ContainerBuilding, BusinessConstants.ApiName);
    }

    /// <summary>
    ///     Logs that the application container has been built.
    /// </summary>
    internal static void LogContainerBuilded()
    {
        Log.Information(MainLoggerMessageDefinitions.ContainerBuilded, BusinessConstants.ApiName);
    }

    /// <summary>
    ///     Logs that the application configuration phase is starting.
    /// </summary>
    internal static void LogStartingConfig()
    {
        Log.Information(MainLoggerMessageDefinitions.StartingConfig, BusinessConstants.ApiName);
    }

    /// <summary>
    ///     Logs that the application configuration phase has completed.
    /// </summary>
    internal static void LogCompletedConfig()
    {
        Log.Information(MainLoggerMessageDefinitions.CompletedConfig, BusinessConstants.ApiName);
    }

    /// <summary>
    ///     Logs that the main service is initializing.
    /// </summary>
    internal static void LogRunningMainService()
    {
        Log.Information(MainLoggerMessageDefinitions.RunningMainService, BusinessConstants.ApiName);
    }

    /// <summary>
    ///     Logs that the main service is ready to accept requests.
    /// </summary>
    internal static void LogReadyMainService()
    {
        Log.Information(MainLoggerMessageDefinitions.ReadyMainService, BusinessConstants.ApiName);
    }

    /// <summary>
    ///     Logs a fatal error when the main pipeline encounters an uncontrolled exception.
    /// </summary>
    /// <param name="ex">The exception that caused the pipeline failure.</param>
    internal static void LogCriticalMainPipelineError(Exception ex)
    {
        Log.Fatal(ex, MainLoggerMessageDefinitions.CriticalMainPipelineError, BusinessConstants.ApiName);
    }

    /// <summary>
    ///     Logs a warning that the fallback logger has been initiated due to a configuration error.
    /// </summary>
    internal static void LogInitFallBack()
    {
        Log.Warning(MainLoggerMessageDefinitions.InitFallback, BusinessConstants.ApiName);
    }

    /// <summary>
    ///     Logs that the service is shutting down.
    /// </summary>
    internal static void LogShutdown()
    {
        Log.Information(MainLoggerMessageDefinitions.Shutdown, BusinessConstants.ApiName);
    }

    /// <summary>
    ///     Logs that the OpenTelemetry metric collector has been enabled.
    /// </summary>
    internal static void LogMetricCollectorEnable()
    {
        Log.Information(MainLoggerMessageDefinitions.OpenTelemetryMetricCollectorEnable, BusinessConstants.ApiName);
    }

    /// <summary>
    ///     Logs that the OpenTelemetry metric collector has been disabled.
    /// </summary>
    internal static void LogMetricCollectorDisable()
    {
        Log.Information(MainLoggerMessageDefinitions.OpenTelemetryMetricCollectorDisable, BusinessConstants.ApiName);
    }

    /// <summary>
    ///     Logs that the OpenTelemetry trace collector has been enabled.
    /// </summary>
    internal static void LogTraceCollectorEnable()
    {
        Log.Information(MainLoggerMessageDefinitions.OpenTelemetryTraceCollectorEnable, BusinessConstants.ApiName);
    }

    /// <summary>
    ///     Logs that the OpenTelemetry trace collector has been disabled.
    /// </summary>
    internal static void LogTraceCollectorDisable()
    {
        Log.Information(MainLoggerMessageDefinitions.OpenTelemetryTraceCollectorDisable, BusinessConstants.ApiName);
    }

    /// <summary>
    ///     Logs a fatal error when a critical unhandled exception occurs in the system.
    /// </summary>
    /// <param name="ex">The unhandled exception.</param>
    internal static void LogCriticalUnhandledException(Exception ex)
    {
        Log.Fatal(ex, MainLoggerMessageDefinitions.CriticalUnhandledException, BusinessConstants.ApiName);
    }

    /// <summary>
    ///     Logs a warning when an unobserved task exception is detected.
    /// </summary>
    /// <param name="ex">The unobserved task exception.</param>
    internal static void LogUnobservedTaskException(Exception ex)
    {
        Log.Warning(ex, MainLoggerMessageDefinitions.UnobservedTaskException, BusinessConstants.ApiName);
    }

    /// <summary>
    ///     Logs that the process is exiting.
    /// </summary>
    internal static void LogProcessExit()
    {
        Log.Information(MainLoggerMessageDefinitions.ProcessExit, BusinessConstants.ApiName);
    }

    /// <summary>
    ///     Logs that the AppDomain is unloading.
    /// </summary>
    internal static void LogDomainUnload()
    {
        Log.Information(MainLoggerMessageDefinitions.DomainUnload, BusinessConstants.ApiName);
    }
}