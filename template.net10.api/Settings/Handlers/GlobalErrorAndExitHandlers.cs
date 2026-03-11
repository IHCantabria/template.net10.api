using Serilog;
using template.net10.api.Core.Exceptions;
using template.net10.api.Core.Logger;

namespace template.net10.api.Settings.Handlers;

/// <summary>
///     Registers process-level and AppDomain-level error and exit event handlers
///     to ensure all unhandled exceptions and graceful shutdowns are captured and logged via Serilog.
/// </summary>
internal static class GlobalErrorAndExitHandlers
{
    /// <summary>
    ///     Registers all global handlers: <see cref="AppDomain.UnhandledException"/>,
    ///     <see cref="TaskScheduler.UnobservedTaskException"/>, <see cref="AppDomain.ProcessExit"/>,
    ///     and <see cref="AppDomain.DomainUnload"/>.
    /// </summary>
    public static void Register()
    {
        AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
        TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;
        AppDomain.CurrentDomain.ProcessExit += OnProcessExit;
        AppDomain.CurrentDomain.DomainUnload += OnDomainUnload;
    }

    /// <summary>
    ///     Handles <see cref="AppDomain.UnhandledException"/>. Logs the exception critically and,
    ///     if the runtime is terminating, flushes the Serilog sink before the process exits.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="args">Contains the exception object and a flag indicating whether the runtime is terminating.</param>
    private static void OnUnhandledException(object? sender, UnhandledExceptionEventArgs args)
    {
        var ex = args.ExceptionObject as Exception
                 ?? new CoreException($"Unhandled exception object: {args.ExceptionObject}");

        MainLoggerMethods.LogCriticalUnhandledException(ex);

        if (!args.IsTerminating) return;

        MainLoggerMethods.LogShutdown();
        Log.CloseAndFlush();
    }

    /// <summary>
    ///     Handles <see cref="TaskScheduler.UnobservedTaskException"/>. Logs the exception and marks it
    ///     as observed to prevent the process from being terminated.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="args">Contains the unobserved <see cref="AggregateException"/>.</param>
    private static void OnUnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs args)
    {
        MainLoggerMethods.LogUnobservedTaskException(args.Exception);
        args.SetObserved();
    }

    /// <summary>
    ///     Handles <see cref="AppDomain.ProcessExit"/>. Logs the process-exit event for observability.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">Event data (unused).</param>
    private static void OnProcessExit(object? sender, EventArgs e)
    {
        MainLoggerMethods.LogProcessExit();
    }

    /// <summary>
    ///     Handles <see cref="AppDomain.DomainUnload"/>. Logs the domain-unload event for observability.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">Event data (unused).</param>
    private static void OnDomainUnload(object? sender, EventArgs e)
    {
        MainLoggerMethods.LogDomainUnload();
    }
}