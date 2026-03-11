namespace template.net10.api.Core.Timeout;

/// <summary>
///     Contains constants for Entity Framework database context retry and timeout configuration.
/// </summary>
internal static class DbContextConstants
{
    /// <summary>
    ///     Maximum number of retry attempts for transient database failures.
    /// </summary>
    internal const short MaxRetryCount = 3;

    /// <summary>
    ///     Maximum delay between database retry attempts.
    /// </summary>
    internal static readonly TimeSpan MaxRetryDelay = TimeSpan.FromSeconds(2);

    /// <summary>
    ///     Database command timeout in seconds.
    /// </summary>
    internal static readonly int CommandTimeout = TimeSpan.FromSeconds(30).Seconds;
}

/// <summary>
///     Contains constants for HTTP request timeout policies and default durations.
/// </summary>
internal static class RequestConstants
{
    /// <summary>
    ///     The policy name for generic query request timeouts.
    /// </summary>
    internal const string RequestQueryGenericPolicy = nameof(RequestQueryGenericPolicy);

    /// <summary>
    ///     The policy name for generic command request timeouts.
    /// </summary>
    internal const string RequestCommandGenericPolicy = nameof(RequestCommandGenericPolicy);

    /// <summary>
    ///     Default timeout duration for command requests (10 seconds).
    /// </summary>
    internal static readonly TimeSpan RequestCommandGenericTimeout = TimeSpan.FromSeconds(10);

    /// <summary>
    ///     Default timeout duration for query requests (3 seconds).
    /// </summary>
    internal static readonly TimeSpan RequestQueryGenericTimeout = TimeSpan.FromSeconds(3);

    /// <summary>
    ///     Default fallback timeout duration for requests (5 seconds).
    /// </summary>
    internal static readonly TimeSpan RequestDefaultTimeout = TimeSpan.FromSeconds(5);
}