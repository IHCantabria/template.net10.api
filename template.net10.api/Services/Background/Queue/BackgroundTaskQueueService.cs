using System.Threading.Channels;
using JetBrains.Annotations;
using template.net10.api.Core.Attributes;
using template.net10.api.Core.Base;
using template.net10.api.Logger;
using template.net10.api.Services.Background.Interfaces;

namespace template.net10.api.Services.Background.Queue;

/// <summary>
///     <see cref="Channel{T}" />-backed implementation of <see cref="IBackgroundTaskQueue" />.
///     Bounded to <see cref="QueueCapacity" /> items with a single-reader optimisation.
///     When the queue is full, new items are dropped and a warning is logged.
/// </summary>
[UsedImplicitly]
[ServiceLifetime(ServiceLifetime.Singleton)]
internal sealed class BackgroundTaskQueue(ILogger<BackgroundTaskQueue> logger)
    : ServiceBase(logger), IBackgroundTaskQueue
{
    /// <summary>
    ///     Maximum number of pending work items. When the queue is full, new items are
    ///     dropped and a warning is logged.
    /// </summary>
    private const int QueueCapacity = 300;

    /// <summary>
    ///     The bounded <see cref="Channel{T}" /> that acts as the internal work-item queue.
    ///     Configured with a capacity of <see cref="QueueCapacity" /> and a single-reader optimisation
    ///     (<see cref="BoundedChannelOptions" /> = <see langword="true" />).
    ///     When the channel is full, <see cref="Enqueue" /> drops the incoming item and logs a warning.
    /// </summary>
    private readonly Channel<BackgroundWorkItem> _channel = Channel.CreateBounded<BackgroundWorkItem>(
        new BoundedChannelOptions(QueueCapacity) { SingleReader = true });

    /// <inheritdoc />
    /// <exception cref="ArgumentNullException"><paramref name="workItem" /> is <see langword="null" />.</exception>
    public void Enqueue(BackgroundWorkItem workItem)
    {
        ArgumentNullException.ThrowIfNull(workItem);
        if (!_channel.Writer.TryWrite(workItem))
            Logger.LogBackgroundQueueFull(QueueCapacity.ToString(), workItem.BackgroundTaskType, workItem.RequestType);
    }

    /// <inheritdoc />
    /// <exception cref="OperationCanceledException">
    ///     The cancellation token was canceled. This exception is stored into the
    ///     returned task.
    /// </exception>
    public ValueTask<BackgroundWorkItem> DequeueAsync(CancellationToken cancellationToken)
    {
        return _channel.Reader.ReadAsync(cancellationToken);
    }
}