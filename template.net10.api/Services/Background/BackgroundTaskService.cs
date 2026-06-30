using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using Microsoft.Extensions.Options;
using template.net10.api.Core.Base;
using template.net10.api.Logger;
using template.net10.api.Services.Background.Extensions;
using template.net10.api.Services.Background.Interfaces;
using template.net10.api.Settings.Options;

namespace template.net10.api.Services.Background;

/// <summary>
///     Long-running hosted service that processes <see cref="BackgroundWorkItem" /> entries from the
///     <see cref="IBackgroundTaskQueue" />. It runs a configurable number of concurrent workers per pool
///     (<see cref="BackgroundWorkCategory" />): database work is drained by
///     <see cref="BackgroundQueueOptions.DatabaseWorkerCount" />
///     workers, external (network) work by <see cref="BackgroundQueueOptions.ExternalWorkerCount" /> workers and
///     notification (SignalR/WebSocket) work by <see cref="BackgroundQueueOptions.NotificationWorkerCount" /> workers,
///     so that slow network or push calls cannot head-of-line block fast database work. Each work item executes
///     within its own DI scope, ensuring scoped services (e.g. DbContext) are properly isolated.
/// </summary>
[UsedImplicitly]
internal sealed class BackgroundTaskService(
    IBackgroundTaskQueue queue,
    IServiceScopeFactory scopeFactory,
    IOptions<BackgroundQueueOptions> options,
    ILogger<BackgroundTaskService> logger)
    : BackgroundServiceBase(logger), IBackgroundTaskService
{
    /// <summary>
    ///     The background queue configuration controlling the per-pool degree of parallelism.
    /// </summary>
    private readonly BackgroundQueueOptions _options =
        options?.Value ?? throw new ArgumentNullException(nameof(options));

    /// <summary>
    ///     The queue from which <see cref="BackgroundWorkItem" /> entries are dequeued and executed by the workers.
    /// </summary>
    private readonly IBackgroundTaskQueue _queue =
        queue ?? throw new ArgumentNullException(nameof(queue));

    /// <summary>
    ///     Factory used to create a new DI scope for each <see cref="BackgroundWorkItem" />,
    ///     ensuring that scoped dependencies (e.g. <c>DbContext</c>) are properly isolated
    ///     between work items and are disposed after each execution.
    /// </summary>
    private readonly IServiceScopeFactory _scopeFactory =
        scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));

    /// <inheritdoc />
    [SuppressMessage(
        "ReSharper",
        "ExceptionNotDocumentedOptional",
        Justification =
            "Potential exceptions originate from underlying implementation details and are not part of the method contract.")]
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var workers = new List<Task>(
            _options.DatabaseWorkerCount + _options.ExternalWorkerCount + _options.NotificationWorkerCount);

        for (var i = 0; i < _options.DatabaseWorkerCount; i++)
            workers.Add(RunWorkerAsync(BackgroundWorkCategory.Database, stoppingToken));

        for (var i = 0; i < _options.ExternalWorkerCount; i++)
            workers.Add(RunWorkerAsync(BackgroundWorkCategory.External, stoppingToken));

        for (var i = 0; i < _options.NotificationWorkerCount; i++)
            workers.Add(RunWorkerAsync(BackgroundWorkCategory.Notification, stoppingToken));

        return Task.WhenAll(workers);
    }

    /// <summary>
    ///     Continuously dequeues and executes work items of the given <paramref name="category" /> until the
    ///     application is stopping. Each item runs in its own DI scope; failures are logged and the worker
    ///     continues with the next item.
    /// </summary>
    [SuppressMessage("ReSharper", "CA2007",
        Justification =
            "ConfigureAwait cant be injected into the AsyncServiceScope created by the IServiceScopeFactory")]
    [SuppressMessage("Roslyn", "CA1031",
        Justification =
            "Catch all exceptions to prevent the background service from crashing. Exceptions are logged and the service continues processing the next work item.")]
    [SuppressMessage("ReSharper", "CatchAllClause",
        Justification =
            "Catch all exceptions to prevent the background service from crashing. Exceptions are logged and the service continues processing the next work item.")]
    private async Task RunWorkerAsync(BackgroundWorkCategory category, CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            BackgroundWorkItem? workItem = null;
            try
            {
                workItem = await _queue.DequeueAsync(category, stoppingToken).ConfigureAwait(false);
                var start = Logger.PrepareLogBackgroundTaskStarted(workItem.BackgroundTaskType, workItem.RequestType);
                await using var scope = _scopeFactory.CreateAsyncScope();
                await workItem.ExecuteAsync(scope.ServiceProvider, stoppingToken).ConfigureAwait(false);
                Logger.PrepareLogBackgroundTaskCompleted(workItem.BackgroundTaskType, workItem.RequestType, start);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                Logger.LogBackgroundTaskException(workItem?.BackgroundTaskType ?? "unknown",
                    workItem?.RequestType ?? "unknown", ex);
            }
        }
    }
}

/// <summary>
///     Represents a unit of work to be executed asynchronously by the
///     <see cref="BackgroundTaskService" />, decoupled from the HTTP request pipeline.
/// </summary>
/// <param name="ExecuteAsync">
///     The asynchronous delegate to execute. Receives a scoped <see cref="IServiceProvider" />
///     for resolving scoped dependencies and a <see cref="CancellationToken" /> for graceful shutdown.
/// </param>
/// <param name="BackgroundTaskType">The background task type name (for structured logging).</param>
/// <param name="RequestType">The originating MediatR request type name (for structured logging).</param>
/// <param name="Category">
///     The pool that processes this work item. Defaults to
///     <see cref="BackgroundWorkCategory.Database" />.
/// </param>
internal sealed record BackgroundWorkItem(
    Func<IServiceProvider, CancellationToken, Task> ExecuteAsync,
    string BackgroundTaskType,
    string RequestType,
    BackgroundWorkCategory Category = BackgroundWorkCategory.Database);