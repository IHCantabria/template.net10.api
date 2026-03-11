using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using MediatR;
using template.net10.api.Logger;

namespace template.net10.api.Behaviors;

/// <summary>
///     MediatR pipeline behavior that logs request handling, including execution timing and result status.
/// </summary>
/// <typeparam name="TRequest">The type of the request being handled.</typeparam>
/// <typeparam name="TResponse">The type of the response returned by the handler.</typeparam>
internal sealed class LoggingBehavior<TRequest, TResponse>(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    /// <summary>
    ///     Property name constant for <see cref="LanguageExt.Common.Result{T}.IsBottom"/>.
    /// </summary>
    private const string IsBottom = nameof(LanguageExt.Common.Result<>.IsBottom);

    /// <summary>
    ///     Property name constant for <see cref="LanguageExt.Common.Result{T}.IsSuccess"/>.
    /// </summary>
    private const string IsSuccess = nameof(LanguageExt.Common.Result<>.IsSuccess);

    /// <summary>
    ///     Property name constant for <see cref="LanguageExt.Common.Result{T}.IsFaulted"/>, used to detect faulted results via reflection.
    /// </summary>
    private const string IsFaulted = nameof(LanguageExt.Common.Result<>.IsFaulted);

    /// <summary>
    ///     Field name constant used to access the internal exception field of a <c>Result</c> via reflection.
    /// </summary>
    private const string Exception = "exception";

    /// <summary>
    ///     Logger instance scoped to <see cref="LoggingBehavior{TRequest, TResponse}"/>.
    /// </summary>
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger =
        logger ?? throw new ArgumentNullException(nameof(logger));

    /// <summary>
    ///     Handles an incoming request by delegating to the logging pipeline logic.
    /// </summary>
    /// <param name="request">The incoming MediatR request.</param>
    /// <param name="next">The delegate for the next handler in the pipeline.</param>
    /// <param name="cancellationToken">A token to observe for cancellation.</param>
    /// <returns>The response produced by the next handler in the pipeline.</returns>
    public Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        return BehaviorLogicAsync(next);
    }

    /// <summary>
    ///     Executes the core logging pipeline: logs the request start, invokes the next handler, and logs the result with elapsed time.
    /// </summary>
    /// <param name="next">The delegate for the next handler in the pipeline.</param>
    /// <returns>The response produced by the next handler.</returns>
    private async Task<TResponse> BehaviorLogicAsync(RequestHandlerDelegate<TResponse> next)
    {
        var start = Stopwatch.GetTimestamp();
        _logger.LogHandlingRequest(typeof(TRequest).Name);
        //PRE REQUEST
        var result = await next().ConfigureAwait(false);
        //POST REQUEST
        LogHandledRequest(result, start);
        return result;
    }

    /// <summary>
    ///     Calculates the elapsed time from the given start timestamp and logs the response accordingly.
    /// </summary>
    /// <param name="result">The response returned by the handler.</param>
    /// <param name="startTimestamp">The high-resolution timestamp captured before the request was handled.</param>
    private void LogHandledRequest(TResponse? result, long startTimestamp)
    {
        var delta = Stopwatch.GetElapsedTime(startTimestamp);
        LogResponse(result, delta);
    }

    /// <summary>
    ///     Inspects the response and dispatches to the appropriate log method based on whether the result is null, a <c>Result</c> type, or a plain value.
    /// </summary>
    /// <param name="result">The response to inspect.</param>
    /// <param name="delta">The elapsed time for the request.</param>
    private void LogResponse(TResponse? result, TimeSpan delta)
    {
        if (result is null)
        {
            LogResponseEmpty(delta);
            return;
        }

        if (IsResultType(result.GetType()))
        {
            LogResult(result, delta);
            return;
        }

        LogResponseSuccess(delta);
    }

    /// <summary>
    ///     Logs that the response was empty (null) along with the elapsed time.
    /// </summary>
    /// <param name="delta">The elapsed time for the request.</param>
    private void LogResponseEmpty(TimeSpan delta)
    {
        _logger.LogHandledRequestIsEmpty(typeof(TRequest).Name, $"{delta.TotalMilliseconds}ms");
    }

    /// <summary>
    ///     Logs a successful response along with the elapsed time.
    /// </summary>
    /// <param name="delta">The elapsed time for the request.</param>
    private void LogResponseSuccess(TimeSpan delta)
    {
        if (_logger.IsEnabled(LogLevel.Information))
            _logger.LogHandledRequestSuccess(typeof(TRequest).Name, $"{delta.TotalMilliseconds}ms");
    }

    /// <summary>
    ///     Logs an error response including the exception details and elapsed time.
    /// </summary>
    /// <param name="ex">The exception extracted from the result.</param>
    /// <param name="delta">The elapsed time for the request.</param>
    private void LogResponseError(Exception ex, TimeSpan delta)
    {
        LogResponseError(delta);
        _logger.LogExceptionClient(ex);
    }

    /// <summary>
    ///     Logs that the request was handled with an error along with the elapsed time.
    /// </summary>
    /// <param name="delta">The elapsed time for the request.</param>
    private void LogResponseError(TimeSpan delta)
    {
        _logger.LogHandledRequestError(typeof(TRequest).Name, $"{delta.TotalMilliseconds}ms");
    }

    /// <summary>
    ///     Inspects a <c>Result</c> response via reflection to determine if it contains an exception, and logs the outcome.
    /// </summary>
    /// <param name="result">The response to inspect.</param>
    /// <param name="delta">The elapsed time for the request.</param>
    [SuppressMessage("Security",
        "S3011:Reflection should not be used to increase accessibility of classes, methods, or fields",
        Justification =
            "Access to the non-public field is necessary because the logic in this file is used to solve an integration problem between the Automapper and HotChocolate libraries.")]
    private void LogResult(TResponse result, TimeSpan delta)
    {
        var type = result?.GetType();
        var exceptionInfo = type?.GetField(Exception, BindingFlags.NonPublic | BindingFlags.Instance);

        // Get the value of the exception field
        if (exceptionInfo?.GetValue(result) is Exception exception)
            LogResponseError(exception, delta);
        else
            LogResponseSuccess(delta);
    }

    /// <summary>
    ///     Determines whether the specified type is a LanguageExt <c>Result</c> type by checking for the presence of <c>IsFaulted</c>, <c>IsSuccess</c>, and <c>IsBottom</c> properties.
    /// </summary>
    /// <param name="type">The type to evaluate.</param>
    /// <returns><see langword="true"/> if the type is a <c>Result</c> type; otherwise, <see langword="false"/>.</returns>
    private static bool IsResultType(Type type)
    {
        return type.GetProperty(IsFaulted) != null
               && type.GetProperty(IsSuccess) != null
               && type.GetProperty(IsBottom) != null;
    }
}