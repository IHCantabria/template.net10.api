using System.Diagnostics;
using template.net10.api.Logger;

namespace template.net10.api.Services.Background.Extensions;

/// <summary>
///     Extension methods for <see cref="ILogger" /> that provide structured, performance-aware
///     logging helpers for background task processing. Each method wraps a high-resolution
///     <see cref="System.Diagnostics.Stopwatch" /> timestamp with the corresponding log call so
///     that elapsed time is computed and included in the log entry.
/// </summary>
internal static class ILoggerExtensions
{
    extension(ILogger logger)
    {
        /// <summary>
        ///     Captures a high-resolution start timestamp and emits a structured log entry indicating
        ///     that the specified background task has started execution for the given originating request type.
        /// </summary>
        /// <param name="backgroundType">The name of the background task class being executed (used in the log entry).</param>
        /// <param name="requestType">The name of the MediatR request type that originated the work item (used in the log entry).</param>
        /// <returns>
        ///     A <see cref="long" /> timestamp obtained from <see cref="System.Diagnostics.Stopwatch.GetTimestamp" />
        ///     that must be passed to <see cref="PrepareLogBackgroundTaskCompleted" /> to compute elapsed time.
        /// </returns>
        internal long PrepareLogBackgroundTaskStarted(string backgroundType, string requestType)
        {
            var start = Stopwatch.GetTimestamp();
            logger.LogBackgroundTaskStarted(backgroundType, requestType);
            return start;
        }

        /// <summary>
        ///     Computes the elapsed time since <paramref name="start" /> and, when
        ///     <see cref="LogLevel.Information" /> is enabled, emits a structured log entry indicating
        ///     that the specified background task has completed execution for the given originating request type.
        /// </summary>
        /// <param name="backgroundType">The name of the background task class that finished executing (used in the log entry).</param>
        /// <param name="requestType">The name of the MediatR request type that originated the work item (used in the log entry).</param>
        /// <param name="start">
        ///     The high-resolution start timestamp returned by <see cref="PrepareLogBackgroundTaskStarted" />.
        /// </param>
        internal void PrepareLogBackgroundTaskCompleted(string backgroundType, string requestType, long start)
        {
            var delta = Stopwatch.GetElapsedTime(start);
            if (logger.IsEnabled(LogLevel.Information))
                logger.LogBackgroundTaskCompleted(backgroundType, requestType,
                    $"{delta.TotalMilliseconds:F1}ms");
        }
    }
}