namespace template.net10.api.Core.Logger;

/// <summary>
///     Contains constant message templates used by the main application logger.
/// </summary>
internal static class MainLoggerMessageDefinitions
{
    /// <summary>
    ///     Message template logged when the main service begins starting.
    /// </summary>
    internal const string StartingMainService = "Starting {Name} Service.";

    /// <summary>
    ///     Message template logged when the application builder starts initializing.
    /// </summary>
    internal const string BuilderStarting = "Starting App Builder for {Name} Service.";

    /// <summary>
    ///     Message template logged when the application builder has finished initializing.
    /// </summary>
    internal const string BuilderStarted = "Started App Builder for {Name} Service.";

    /// <summary>
    ///     Message template logged when services are being installed in the application builder.
    /// </summary>
    internal const string InstallingServices = "Installing Services in the App Builder of the {Name} Service.";

    /// <summary>
    ///     Message template logged when all services have been installed in the application builder.
    /// </summary>
    internal const string InstalledServices = "Installed Services in the App Builder of the {Name} Service.";

    /// <summary>
    ///     Message template logged when the application container is being built.
    /// </summary>
    internal const string ContainerBuilding = "Building App Container for {Name} Service.";

    /// <summary>
    ///     Message template logged when the application container has been built.
    /// </summary>
    internal const string ContainerBuilded = "Builded App Container for {Name} Service.";

    /// <summary>
    ///     Message template logged when the application configuration phase begins.
    /// </summary>
    internal const string StartingConfig = "Starting {Name} Configuration.";

    /// <summary>
    ///     Message template logged when the application configuration phase completes.
    /// </summary>
    internal const string CompletedConfig = "Completed {Name} configuration.";

    /// <summary>
    ///     Message template logged when the main service is initializing.
    /// </summary>
    internal const string RunningMainService = "Initializing Service {Name}.";

    /// <summary>
    ///     Message template logged when the main service is ready to accept requests.
    /// </summary>
    internal const string ReadyMainService = "Service {Name} ready to use.";

    /// <summary>
    ///     Message template logged when the service stops due to an uncontrolled exception in the main pipeline.
    /// </summary>
    internal const string CriticalMainPipelineError =
        "Stopped {Name} due to a critical not controlled exception in the main pipeline.";

    /// <summary>
    ///     Message template logged when the service stops due to a critical unhandled system exception.
    /// </summary>
    internal const string CriticalUnhandledException =
        "Stopped {Name} due to a critical not controlled exception in the system.";

    /// <summary>
    ///     Message template logged when an unobserved task exception is detected.
    /// </summary>
    internal const string UnobservedTaskException =
        "Unobserved task exception detected in a Task launched in the {Name} system.";

    /// <summary>
    ///     Message template logged when the process is exiting.
    /// </summary>
    internal const string ProcessExit =
        "Process {Name} is exiting.";

    /// <summary>
    ///     Message template logged when the AppDomain is unloading.
    /// </summary>
    internal const string DomainUnload =
        "AppDomain {Name} unloading.";

    /// <summary>
    ///     Message template logged when the fallback logger is initiated due to a configuration error.
    /// </summary>
    internal const string InitFallback =
        "An error occurred before or during the configuration of the main application log for {Name} , initiating Fallback log.";

    /// <summary>
    ///     Message template logged when the service is shutting down.
    /// </summary>
    internal const string Shutdown = "Shutdown {Name}.";

    /// <summary>
    ///     Message template logged when the OpenTelemetry metric collector is enabled.
    /// </summary>
    internal const string OpenTelemetryMetricCollectorEnable = "OpenTelemetry Metric Collector is enabled for {Name}.";

    /// <summary>
    ///     Message template logged when the OpenTelemetry metric collector is disabled.
    /// </summary>
    internal const string OpenTelemetryMetricCollectorDisable =
        "OpenTelemetry Metric Collector is disabled for {Name}.";

    /// <summary>
    ///     Message template logged when the OpenTelemetry trace collector is enabled.
    /// </summary>
    internal const string OpenTelemetryTraceCollectorEnable = "OpenTelemetry Trace Collector is enabled for {Name}.";

    /// <summary>
    ///     Message template logged when the OpenTelemetry trace collector is disabled.
    /// </summary>
    internal const string OpenTelemetryTraceCollectorDisable = "OpenTelemetry Trace Collector is disabled for {Name}.";
}