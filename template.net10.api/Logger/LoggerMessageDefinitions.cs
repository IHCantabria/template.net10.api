namespace template.net10.api.Logger;

/// <summary>
///     Defines structured log message templates used by the source-generated logger methods.
/// </summary>
internal static class LoggerMessageDefinitions
{
    /// <summary>
    ///     Log message template indicating a service has been successfully injected.
    /// </summary>
    internal const string ServiceInjected = "Service {serviceType} injected successfully";

    /// <summary>
    ///     Log message template indicating a controller has been successfully injected.
    /// </summary>
    internal const string ControllerInjected = "Controller {controllerType} injected successfully";

    /// <summary>
    ///     Log message template indicating a repository has been successfully injected.
    /// </summary>
    internal const string RepositoryInjected = "Repository {repositoryType} injected successfully";

    /// <summary>
    ///     Log message template indicating a web client has been successfully injected.
    /// </summary>
    internal const string WebClientInjected = "Web Client {webClientType} injected successfully";

    /// <summary>
    ///     Log message template for server-side exceptions.
    /// </summary>
    internal const string ExceptionServer = "Server Exception occurred.";

    /// <summary>
    ///     Log message template indicating the database health check failed.
    /// </summary>
    internal const string StatusDbFail = "DB is not running.";

    /// <summary>
    ///     Log message template for client-side exceptions.
    /// </summary>
    internal const string ExceptionClient = "Client Exception occurred.";

    /// <summary>
    ///     Log message template for an incoming action request.
    /// </summary>
    internal const string ActionRequestReceived = "Action Request Received: {methodName} at path {requestPath}";

    /// <summary>
    ///     Log message template for a successfully completed action request.
    /// </summary>
    internal const string ActionRequestResponsed = "Action Request Responsed: {methodName} at path {requestPath}";

    /// <summary>
    ///     Log message template for an action request that completed with an error status code.
    /// </summary>
    internal const string ActionRequestResponsedError =
        "Action Request Responsed: {methodName} at path {requestPath} with ERROR code: {statusCode}";

    /// <summary>
    ///     Log message template for the start of a MediatR request handling.
    /// </summary>
    internal const string HandlingRequest = "Handling Request {requestType}";

    /// <summary>
    ///     Log message template for a successfully handled MediatR request including elapsed time.
    /// </summary>
    internal const string HandledRequestSuccess = "Handled Request {requestType}, with state Success in {time}";

    /// <summary>
    ///     Log message template for a MediatR request that returned an empty result.
    /// </summary>
    internal const string HandledRequestEmpty =
        "Handled Request {requestType}, finished in {time} with empty Result";

    /// <summary>
    ///     Log message template for a MediatR request that resulted in an error.
    /// </summary>
    internal const string HandledRequestError = "Handled Request {requestType}, with state ERROR in {time}";

    /// <summary>
    ///     Log message template for the start of a MediatR post-process execution.
    /// </summary>
    internal const string HandlingPostProcess =
        "Handling Post Process {postProcessType} for the Request {requestType}";

    /// <summary>
    ///     Log message template for a completed MediatR post-process including elapsed time.
    /// </summary>
    internal const string HandledPostProcess =
        "Handled Post Process {postProcessType} for the {requestType} in {time}";
}