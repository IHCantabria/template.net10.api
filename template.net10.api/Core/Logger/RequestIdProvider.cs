using System.Diagnostics;

namespace template.net10.api.Core.Logger;

/// <summary>
///     Provides ambient request ID storage and retrieval using <see cref="AsyncLocal{T}"/> and <see cref="Activity"/>.
/// </summary>
internal static class RequestIdProvider
{
    /// <summary>
    ///     Async-local storage for the current HTTP request trace identifier.
    /// </summary>
    private static readonly AsyncLocal<string> CurrentTraceIdentifier = new();

    /// <summary>
    ///     Sets the trace identifier for the current async execution context.
    /// </summary>
    /// <param name="traceIdentifier">The trace identifier to store.</param>
    public static void SetCurrentTraceIdentifier(string traceIdentifier)
    {
        CurrentTraceIdentifier.Value = traceIdentifier;
    }

    /// <summary>
    ///     Gets the current request ID from the active <see cref="Activity"/> or the stored trace identifier.
    /// </summary>
    /// <returns>The current request ID, or <see langword="null"/> if none is available.</returns>
    public static string? GetCurrentRequestId()
    {
        return Activity.Current?.Id ?? CurrentTraceIdentifier.Value;
    }
}