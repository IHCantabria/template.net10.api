namespace template.net10.api.Services.Background.Interfaces;

/// <summary>
///     Thread-safe, bounded queue for fire-and-forget background work items.
///     Producers (post-processors) enqueue items; the single consumer
///     (<see cref="BackgroundTaskService" />) dequeues and executes them.
/// </summary>
internal interface IBackgroundTaskQueue
{
    /// <summary>
    ///     Enqueues a work item on a best-effort basis. If the internal buffer is full,
    ///     the item is silently dropped and a warning is logged.
    /// </summary>
    /// <param name="workItem">The work item to enqueue.</param>
    void Enqueue(BackgroundWorkItem workItem);

    /// <summary>
    ///     Dequeues the next work item, waiting asynchronously until one is available
    ///     or <paramref name="cancellationToken" /> is cancelled.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the wait.</param>
    /// <returns>The next work item to process.</returns>
    ValueTask<BackgroundWorkItem> DequeueAsync(CancellationToken cancellationToken);
}