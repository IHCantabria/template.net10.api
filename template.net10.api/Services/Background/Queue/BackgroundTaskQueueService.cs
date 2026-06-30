using System.Diagnostics.CodeAnalysis;
using System.Threading.Channels;
using JetBrains.Annotations;
using Microsoft.Extensions.Options;
using template.net10.api.Core.Attributes;
using template.net10.api.Core.Base;
using template.net10.api.Logger;
using template.net10.api.Services.Background.Interfaces;
using template.net10.api.Settings.Options;

namespace template.net10.api.Services.Background.Queue;

/// <summary>
///     <see cref="Channel{T}" />-backed implementation of <see cref="IBackgroundTaskQueue" /> split into three
///     bounded pools (<see cref="BackgroundWorkCategory.Database" />, <see cref="BackgroundWorkCategory.External" />
///     and <see cref="BackgroundWorkCategory.Notification" />).
///     Each pool is sized and parallelised independently through <see cref="BackgroundQueueOptions" />.
///     When a pool is full, <see cref="EnqueueAsync" /> logs a back-pressure warning and waits up to
///     <see cref="BackgroundQueueOptions.EnqueueTimeout" /> for free space; if the timeout elapses the item is
///     dropped and an error is logged.
/// </summary>
[UsedImplicitly]
[ServiceLifetime(ServiceLifetime.Singleton)]
internal sealed class BackgroundTaskQueue : ServiceBase, IBackgroundTaskQueue
{
    /// <summary>
    ///     The bounded pool backing <see cref="BackgroundWorkCategory.Database" /> work items.
    /// </summary>
    private readonly Channel<BackgroundWorkItem> _databaseChannel;

    /// <summary>
    ///     The bounded pool backing <see cref="BackgroundWorkCategory.External" /> work items.
    /// </summary>
    private readonly Channel<BackgroundWorkItem> _externalChannel;

    /// <summary>
    ///     The bounded pool backing <see cref="BackgroundWorkCategory.Notification" /> work items.
    /// </summary>
    private readonly Channel<BackgroundWorkItem> _notificationChannel;

    /// <summary>
    ///     The background queue configuration controlling per-pool capacity and the back-pressure timeout.
    /// </summary>
    private readonly BackgroundQueueOptions _options;

    /// <summary>
    ///     Initializes the three bounded pools from <paramref name="options" />.
    /// </summary>
    /// <param name="logger">The logger instance for diagnostic output.</param>
    /// <param name="options">The background queue configuration.</param>
    /// <exception cref="ArgumentNullException"><paramref name="options" /> is <see langword="null" />.</exception>
    [SuppressMessage(
        "ReSharper",
        "ExceptionNotDocumentedOptional",
        Justification =
            "Potential exceptions originate from underlying implementation details and are not part of the method contract.")]
    public BackgroundTaskQueue(ILogger<BackgroundTaskQueue> logger, IOptions<BackgroundQueueOptions> options)
        : base(logger)
    {
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));

        _databaseChannel = Channel.CreateBounded<BackgroundWorkItem>(
            new BoundedChannelOptions(_options.DatabaseCapacity)
            {
                SingleReader = _options.DatabaseWorkerCount == 1,
                SingleWriter = false,
                FullMode = BoundedChannelFullMode.Wait
            });

        _externalChannel = Channel.CreateBounded<BackgroundWorkItem>(
            new BoundedChannelOptions(_options.ExternalCapacity)
            {
                SingleReader = _options.ExternalWorkerCount == 1,
                SingleWriter = false,
                FullMode = BoundedChannelFullMode.Wait
            });

        _notificationChannel = Channel.CreateBounded<BackgroundWorkItem>(
            new BoundedChannelOptions(_options.NotificationCapacity)
            {
                SingleReader = _options.NotificationWorkerCount == 1,
                SingleWriter = false,
                FullMode = BoundedChannelFullMode.Wait
            });
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentNullException"><paramref name="workItem" /> is <see langword="null" />.</exception>
    /// <exception cref="ObjectDisposedException">
    ///     The exception thrown when this <see cref="CancellationTokenSource" /> has
    ///     been disposed.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     The configured back-pressure timeout's <see cref="TimeSpan.TotalMilliseconds" /> is less than -1 or greater
    ///     than Int32.MaxValue (or UInt32.MaxValue - 1 on some versions of .NET). Note that this upper bound is more
    ///     restrictive than TimeSpan.MaxValue.
    /// </exception>
    public async ValueTask EnqueueAsync(BackgroundWorkItem workItem, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(workItem);

        var channel = ResolveChannel(workItem.Category);

        // Fast path: write immediately when there is free space.
        if (channel.Writer.TryWrite(workItem))
            return;

        // Pool is full: surface the back-pressure and wait up to the configured timeout for free space.
        Logger.LogBackgroundQueueBackpressure(workItem.Category.ToString(),
            ResolveCapacity(workItem.Category).ToString(), workItem.BackgroundTaskType, workItem.RequestType);

        using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeoutCts.CancelAfter(_options.EnqueueTimeout);

        try
        {
            await channel.Writer.WriteAsync(workItem, timeoutCts.Token).ConfigureAwait(false);
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            // The back-pressure timeout elapsed before space became available: drop it and log an error.
            Logger.LogBackgroundQueueFull(workItem.Category.ToString(),
                ResolveCapacity(workItem.Category).ToString(), workItem.BackgroundTaskType, workItem.RequestType);
        }
    }

    /// <inheritdoc />
    /// <exception cref="OperationCanceledException">
    ///     The cancellation token was canceled. This exception is stored into the returned task.
    /// </exception>
    public ValueTask<BackgroundWorkItem> DequeueAsync(BackgroundWorkCategory category,
        CancellationToken cancellationToken)
    {
        return ResolveChannel(category).Reader.ReadAsync(cancellationToken);
    }

    /// <summary>
    ///     Selects the channel backing the given <paramref name="category" />.
    /// </summary>
    private Channel<BackgroundWorkItem> ResolveChannel(BackgroundWorkCategory category)
    {
        return category switch
        {
            BackgroundWorkCategory.External => _externalChannel,
            BackgroundWorkCategory.Notification => _notificationChannel,
            _ => _databaseChannel
        };
    }

    /// <summary>
    ///     Returns the configured capacity of the pool backing the given <paramref name="category" />.
    /// </summary>
    private int ResolveCapacity(BackgroundWorkCategory category)
    {
        return category switch
        {
            BackgroundWorkCategory.External => _options.ExternalCapacity,
            BackgroundWorkCategory.Notification => _options.NotificationCapacity,
            _ => _options.DatabaseCapacity
        };
    }
}