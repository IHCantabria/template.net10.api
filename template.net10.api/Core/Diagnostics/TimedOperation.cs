using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace template.net10.api.Core.Diagnostics;

/// <summary>
///     Provides utility methods for measuring the elapsed time of synchronous and asynchronous operations
///     without polluting call sites with <see cref="Stopwatch" /> boilerplate.
///     Uses <see cref="Stopwatch.GetTimestamp" /> and <see cref="Stopwatch.GetElapsedTime(long)" /> internally
///     to avoid heap-allocating a <see cref="Stopwatch" /> instance, consistent with the measurement
///     pattern used across <c>LoggingBehavior</c> and the <c>ILoggerExtensions</c> in this project.
/// </summary>
[SuppressMessage(
    "ReSharper",
    "UnusedMember.Global",
    Justification = "General-purpose helper methods; not all members are used in every scenario.")]
[SuppressMessage(
    "ReSharper",
    "UnusedType.Global",
    Justification = "General-purpose helper type; usage depends on consumer requirements.")]
internal static class TimedOperation
{
    /// <summary>
    ///     Executes an asynchronous operation that returns a value and measures its elapsed time.
    /// </summary>
    /// <typeparam name="T">The return type of the operation.</typeparam>
    /// <param name="operation">The asynchronous operation to time.</param>
    /// <returns>A tuple containing the operation result and elapsed milliseconds.</returns>
    /// <exception cref="Exception">A delegate callback throws an exception.</exception>
    internal static async Task<(T Result, long ElapsedMs)> RunAsync<T>(Func<Task<T>> operation)
    {
        var start = Stopwatch.GetTimestamp();
        var result = await operation().ConfigureAwait(false);
        var delta = Stopwatch.GetElapsedTime(start);
        return (result, (long)delta.TotalMilliseconds);
    }

    /// <summary>
    ///     Executes an asynchronous operation with no return value and measures its elapsed time.
    /// </summary>
    /// <param name="operation">The asynchronous operation to time.</param>
    /// <returns>Elapsed milliseconds.</returns>
    /// <exception cref="Exception">A delegate callback throws an exception.</exception>
    internal static async Task<long> RunAsync(Func<Task> operation)
    {
        var start = Stopwatch.GetTimestamp();
        await operation().ConfigureAwait(false);
        var delta = Stopwatch.GetElapsedTime(start);
        return (long)delta.TotalMilliseconds;
    }

    /// <summary>
    ///     Executes a synchronous operation that returns a value and measures its elapsed time.
    /// </summary>
    /// <typeparam name="T">The return type of the operation.</typeparam>
    /// <param name="operation">The synchronous operation to time.</param>
    /// <returns>A tuple containing the operation result and elapsed milliseconds.</returns>
    /// <exception cref="Exception">A delegate callback throws an exception.</exception>
    internal static (T Result, long ElapsedMs) Run<T>(Func<T> operation)
    {
        var start = Stopwatch.GetTimestamp();
        var result = operation();
        var delta = Stopwatch.GetElapsedTime(start);
        return (result, (long)delta.TotalMilliseconds);
    }

    /// <summary>
    ///     Executes a synchronous operation with no return value and measures its elapsed time.
    /// </summary>
    /// <param name="operation">The synchronous operation to time.</param>
    /// <returns>Elapsed milliseconds.</returns>
    /// <exception cref="Exception">A delegate callback throws an exception.</exception>
    internal static long Run(Action operation)
    {
        var start = Stopwatch.GetTimestamp();
        operation();
        var delta = Stopwatch.GetElapsedTime(start);
        return (long)delta.TotalMilliseconds;
    }
}