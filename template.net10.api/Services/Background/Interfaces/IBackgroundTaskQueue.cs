namespace template.net10.api.Services.Background.Interfaces;

/// <summary>
///     Thread-safe, bounded queue for fire-and-forget background work items, split into independent
///     pools by <see cref="BackgroundWorkCategory" />. Producers (post-processors) enqueue items;
///     the consumers (<see cref="BackgroundTaskService" /> workers) dequeue and execute them.
/// </summary>
internal interface IBackgroundTaskQueue
{
    /// <summary>
    ///     Enqueues a work item into the pool selected by its <see cref="BackgroundWorkItem.Category" />.
    ///     When the target pool is full, the call applies bounded back-pressure: it waits up to the configured
    ///     enqueue timeout for free space and only then drops the item, logging an error and recording a metric.
    /// </summary>
    /// <param name="workItem">The work item to enqueue.</param>
    /// <param name="cancellationToken">Token to abort the back-pressure wait.</param>
    ValueTask EnqueueAsync(BackgroundWorkItem workItem, CancellationToken cancellationToken);

    /// <summary>
    ///     Dequeues the next work item from the pool identified by <paramref name="category" />, waiting
    ///     asynchronously until one is available or <paramref name="cancellationToken" /> is cancelled.
    /// </summary>
    /// <param name="category">The pool to read from.</param>
    /// <param name="cancellationToken">Token to cancel the wait.</param>
    /// <returns>The next work item to process.</returns>
    ValueTask<BackgroundWorkItem> DequeueAsync(BackgroundWorkCategory category, CancellationToken cancellationToken);
}

/// <summary>
///     Identifies which pool a <see cref="BackgroundWorkItem" /> is processed by, so that slow network-bound
///     and push-based work are isolated from fast database-bound work.
/// </summary>
internal enum BackgroundWorkCategory
{
    /// <summary>
    ///     Fast, database-bound work (e.g. state transitions, metadata writes). Drained serially by default to keep
    ///     writes to shared aggregates ordered.
    /// </summary>
    Database = 0,

    /// <summary>
    ///     Slow, network-bound work (e.g. GeoPublisher HTTP dispatch). Drained by several workers in parallel.
    /// </summary>
    External = 1,

    /// <summary>
    ///     Push-based, best-effort work (e.g. SignalR/WebSocket broadcasts). Does not touch shared DB aggregates;
    ///     drained by several workers in parallel and isolated from both database and external work.
    /// </summary>
    Notification = 2
}