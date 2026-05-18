using System.Diagnostics;
using template.net10.api.Logger;

namespace template.net10.api.Behaviors.Extensions;

/// <summary>
///     Extension methods for <see cref="ILogger" /> that provide structured, performance-aware
///     logging helpers for MediatR post-processors. Each method wraps a high-resolution
///     <see cref="System.Diagnostics.Stopwatch" /> timestamp with the corresponding log call so
///     that elapsed time is computed and included in the log entry.
/// </summary>
internal static class ILoggerExtensions
{
    extension(ILogger logger)
    {
        /// <summary>
        ///     Captures a high-resolution start timestamp and emits a structured log entry indicating
        ///     that the specified MediatR post-processor has begun handling the given request type.
        /// </summary>
        /// <param name="postprocessorType">The name of the post-processor class being executed (used in the log entry).</param>
        /// <param name="requestType">The name of the MediatR request type being post-processed (used in the log entry).</param>
        /// <returns>
        ///     A <see cref="long" /> timestamp obtained from <see cref="System.Diagnostics.Stopwatch.GetTimestamp" />
        ///     that must be passed to <see cref="PrepareLogHandledPostProcess" /> to compute elapsed time.
        /// </returns>
        internal long PrepareLogHandlingPostProcess(string postprocessorType, string requestType)
        {
            var start = Stopwatch.GetTimestamp();
            logger.LogHandlingPostProcess(postprocessorType, requestType);
            return start;
        }

        /// <summary>
        ///     Computes the elapsed time since <paramref name="start" /> and, when
        ///     <see cref="LogLevel.Information" /> is enabled, emits a structured log entry indicating
        ///     that the specified MediatR post-processor has finished processing the given request type.
        /// </summary>
        /// <param name="postprocessorType">The name of the post-processor class that finished executing (used in the log entry).</param>
        /// <param name="requestType">The name of the MediatR request type that was post-processed (used in the log entry).</param>
        /// <param name="start">
        ///     The high-resolution start timestamp returned by <see cref="PrepareLogHandlingPostProcess" />.
        /// </param>
        internal void PrepareLogHandledPostProcess(string postprocessorType, string requestType, long start)
        {
            var delta = Stopwatch.GetElapsedTime(start);
            if (logger.IsEnabled(LogLevel.Information))
                logger.LogHandledPostProcess(postprocessorType, requestType,
                    $"{delta.TotalMilliseconds:F1}ms");
        }
    }
}