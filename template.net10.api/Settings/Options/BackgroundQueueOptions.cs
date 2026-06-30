using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Options;

namespace template.net10.api.Settings.Options;

/// <summary>
///     Strongly-typed configuration for the background task queue. Controls the per-pool capacity,
///     the degree of parallelism (worker count) of each pool and the back-pressure timeout applied
///     when a pool is full. Bound from the <c>BackgroundQueue</c> configuration section.
/// </summary>
/// <remarks>
///     The queue is split into three independent pools:
///     <list type="bullet">
///         <item>
///             <b>Database</b>: fast, DB-bound work items (state transitions, metadata writes). Defaults to a
///             single worker so that writes to shared aggregates (e.g. <c>Dataset</c>) stay serialized, which
///             preserves correctness in the absence of optimistic-concurrency tokens. Increasing
///             <see cref="DatabaseWorkerCount" /> above 1 requires the handlers to be safe under concurrency
///             (idempotent / guarded by unique constraints or row-version tokens).
///         </item>
///         <item>
///             <b>External</b>: slow, network-bound work items (e.g. GeoPublisher HTTP dispatch). These do not
///             mutate shared DB aggregates, so they can be safely processed by several workers in parallel,
///             which prevents a single slow call from head-of-line blocking the rest of the queue.
///         </item>
///         <item>
///             <b>Notification</b>: push-based, best-effort work items (e.g. SignalR/WebSocket broadcasts). Like
///             external work they do not touch shared DB aggregates, so they run in parallel; isolating them keeps
///             a slow client or a GeoPublisher backlog from interfering with one another.
///         </item>
///     </list>
/// </remarks>
[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)]
internal sealed record BackgroundQueueOptions
{
    /// <summary>
    ///     The configuration section key used to bind <see cref="BackgroundQueueOptions" />.
    /// </summary>
    public const string BackgroundQueue = nameof(BackgroundQueue);

    /// <summary>
    ///     Maximum number of pending work items buffered in the database pool before back-pressure applies.
    /// </summary>
    [Required]
    [Range(1, 1_000_000)]
    public required int DatabaseCapacity { get; init; }

    /// <summary>
    ///     Maximum number of pending work items buffered in the external (network) pool before back-pressure applies.
    /// </summary>
    [Required]
    [Range(1, 1_000_000)]
    public required int ExternalCapacity { get; init; }

    /// <summary>
    ///     Maximum number of pending work items buffered in the notification (SignalR/WebSocket) pool before
    ///     back-pressure applies.
    /// </summary>
    [Required]
    [Range(1, 1_000_000)]
    public required int NotificationCapacity { get; init; }

    /// <summary>
    ///     Number of concurrent workers draining the database pool. Defaults to <c>1</c> (serial) to keep writes
    ///     to shared aggregates ordered. Only raise this when the database work items are safe under concurrency.
    /// </summary>
    [Required]
    [Range(1, 256)]
    public required int DatabaseWorkerCount { get; init; }

    /// <summary>
    ///     Number of concurrent workers draining the external (network) pool. Defaults to <c>8</c> so that several
    ///     slow HTTP dispatches can progress in parallel without blocking one another.
    /// </summary>
    [Required]
    [Range(1, 256)]
    public required int ExternalWorkerCount { get; init; }

    /// <summary>
    ///     Number of concurrent workers draining the notification (SignalR/WebSocket) pool. These broadcasts are
    ///     push-based and do not touch shared DB aggregates, so they can run in parallel.
    /// </summary>
    [Required]
    [Range(1, 256)]
    public required int NotificationWorkerCount { get; init; }

    /// <summary>
    ///     Maximum time a producer waits for free space when the target pool is full before the work item is
    ///     dropped and an error is logged. Bounds how long a request can be slowed down by
    ///     back-pressure during an extreme peak.
    /// </summary>
    [Range(typeof(TimeSpan), "00:00:00", "01:00:00",
        ErrorMessage = "EnqueueTimeout must be between 0 seconds and 1 hour.")]
    public required TimeSpan EnqueueTimeout { get; init; }
}

/// <summary>
///     Source-generated <see cref="IValidateOptions{TOptions}" /> validator for <see cref="BackgroundQueueOptions" />.
/// </summary>
[OptionsValidator]
internal sealed partial class BackgroundQueueOptionsValidator : IValidateOptions<BackgroundQueueOptions>;