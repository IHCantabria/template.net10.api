namespace template.net10.api.Logger;

/// <summary>
///     Provides source-generated high-performance logger extension methods for structured logging throughout the application.
/// </summary>
internal static partial class LoggerMessageMethods
{
    /// <summary>
    ///     Logs a debug message when a controller base class is successfully injected.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="controllerType">The type name of the injected controller.</param>
    [LoggerMessage(Level = LogLevel.Debug,
        Message = LoggerMessageDefinitions.ControllerInjected)]
    internal static partial void LogControllerBaseInjected(this ILogger logger, string controllerType);

    /// <summary>
    ///     Logs a debug message when a service base class is successfully injected.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="serviceType">The type name of the injected service.</param>
    [LoggerMessage(Level = LogLevel.Debug,
        Message = LoggerMessageDefinitions.ServiceInjected)]
    internal static partial void LogServiceBaseInjected(this ILogger logger, string serviceType);

    /// <summary>
    ///     Logs a debug message when a repository base class is successfully injected.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="repositoryType">The type name of the injected repository.</param>
    [LoggerMessage(Level = LogLevel.Debug,
        Message = LoggerMessageDefinitions.RepositoryInjected)]
    internal static partial void LogRepositoryBaseInjected(this ILogger logger, string repositoryType);

    /// <summary>
    ///     Logs a debug message when a web client base class is successfully injected.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="webClientType">The type name of the injected web client.</param>
    [LoggerMessage(Level = LogLevel.Debug,
        Message = LoggerMessageDefinitions.WebClientInjected)]
    internal static partial void LogWebClientBaseInjected(this ILogger logger, string webClientType);

    /// <summary>
    ///     Logs a server-side exception at error level.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="ex">The exception that occurred on the server.</param>
    [LoggerMessage(Level = LogLevel.Error, Message = LoggerMessageDefinitions.ExceptionServer)]
    internal static partial void LogExceptionServer(this ILogger logger, Exception ex);

    /// <summary>
    ///     Logs an incoming action request with method name and path.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="methodName">The HTTP method name of the request.</param>
    /// <param name="requestPath">The path of the incoming request.</param>
    [LoggerMessage(Level = LogLevel.Information,
        Message = LoggerMessageDefinitions.ActionRequestReceived)]
    internal static partial void LogActionRequestReceived(this ILogger logger, string? methodName, string? requestPath);

    /// <summary>
    ///     Logs a successful action request response with method name and path.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="methodName">The HTTP method name of the request.</param>
    /// <param name="requestPath">The path of the request.</param>
    [LoggerMessage(Level = LogLevel.Information,
        Message = LoggerMessageDefinitions.ActionRequestResponsed)]
    internal static partial void
        LogActionRequestResponsedSuccess(this ILogger logger, string? methodName, string? requestPath);

    /// <summary>
    ///     Logs an action request that completed with an error status code.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="methodName">The HTTP method name of the request.</param>
    /// <param name="requestPath">The path of the request.</param>
    /// <param name="statusCode">The HTTP status code of the error response.</param>
    [LoggerMessage(Level = LogLevel.Error,
        Message = LoggerMessageDefinitions.ActionRequestResponsedError)]
    internal static partial void
        LogActionRequestResponsedError(this ILogger logger, string? methodName, string? requestPath,
            string? statusCode);

    /// <summary>
    ///     Logs the start of a MediatR request handling.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="requestType">The type name of the MediatR request being handled.</param>
    [LoggerMessage(Level = LogLevel.Information,
        Message = LoggerMessageDefinitions.HandlingRequest)]
    internal static partial void LogHandlingRequest(this ILogger logger, string requestType);

    /// <summary>
    ///     Logs a successfully handled MediatR request with elapsed time.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="requestType">The type name of the handled request.</param>
    /// <param name="time">The formatted elapsed time string.</param>
    [LoggerMessage(Level = LogLevel.Information,
        Message = LoggerMessageDefinitions.HandledRequestSuccess)]
    internal static partial void LogHandledRequestSuccess(this ILogger logger, string requestType, string time);

    /// <summary>
    ///     Logs a MediatR request that returned an empty (null) result.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="requestType">The type name of the handled request.</param>
    /// <param name="time">The formatted elapsed time string.</param>
    [LoggerMessage(Level = LogLevel.Warning, Message = LoggerMessageDefinitions.HandledRequestEmpty)]
    internal static partial void LogHandledRequestIsEmpty(this ILogger logger, string requestType, string time);

    /// <summary>
    ///     Logs a MediatR request that resulted in an error.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="requestType">The type name of the handled request.</param>
    /// <param name="time">The formatted elapsed time string.</param>
    [LoggerMessage(Level = LogLevel.Warning, Message = LoggerMessageDefinitions.HandledRequestError)]
    internal static partial void LogHandledRequestError(this ILogger logger, string requestType, string time);

    /// <summary>
    ///     Logs a client-side exception at warning level.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="ex">The client exception that occurred.</param>
    [LoggerMessage(Level = LogLevel.Warning, Message = LoggerMessageDefinitions.ExceptionClient)]
    internal static partial void LogExceptionClient(this ILogger logger, Exception ex);

    /// <summary>
    ///     Logs the start of a MediatR post-process execution.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="postProcessType">The type name of the post-processor.</param>
    /// <param name="requestType">The type name of the originating request.</param>
    [LoggerMessage(Level = LogLevel.Information, Message = LoggerMessageDefinitions.HandlingPostProcess)]
    internal static partial void
        LogHandlingPostProcess(this ILogger logger, string postProcessType, string requestType);

    /// <summary>
    ///     Logs a completed MediatR post-process with elapsed time.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="postProcessType">The type name of the post-processor.</param>
    /// <param name="requestType">The type name of the originating request.</param>
    /// <param name="time">The formatted elapsed time string.</param>
    [LoggerMessage(Level = LogLevel.Information, Message = LoggerMessageDefinitions.HandledPostProcess)]
    internal static partial void LogHandledPostProcess(this ILogger logger, string postProcessType, string requestType,
        string time);

    /// <summary>
    ///     Logs a database health check failure at error level.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="ex">The exception from the failed database connection attempt.</param>
    [LoggerMessage(Level = LogLevel.Error, Message = LoggerMessageDefinitions.StatusDbFail)]
    internal static partial void LogStatusDbFail(this ILogger logger, Exception ex);
}