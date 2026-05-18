using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using template.net10.api.Core.Base;
using template.net10.api.Logger;
using template.net10.api.Services.Background.Extensions;
using template.net10.api.Services.Background.Interfaces;

namespace template.net10.api.Services.Background;

/// <summary>
///     Long-running hosted service that processes <see cref="BackgroundWorkItem" /> entries
///     from the <see cref="IBackgroundTaskQueue" /> sequentially. Each work item executes
///     within its own DI scope, ensuring scoped services (e.g. DbContext) are properly isolated.
/// </summary>
[UsedImplicitly]
internal sealed class BackgroundTaskService(
    IBackgroundTaskQueue queue,
    IServiceScopeFactory scopeFactory,
    ILogger<BackgroundTaskService> logger)
    : BackgroundServiceBase(logger), IBackgroundTaskService
{
    /// <summary>
    ///     The queue from which <see cref="BackgroundWorkItem" /> entries are dequeued
    ///     one at a time and executed sequentially by <see cref="ExecuteAsync" />.
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
    [SuppressMessage("ReSharper", "CA2007",
        Justification =
            "ConfigureAwait cant be injected into the AsyncServiceScope created by the IServiceScopeFactory")]
    [SuppressMessage("Roslyn", "CA1031",
        Justification =
            "Catch all exceptions to prevent the background service from crashing. Exceptions are logged and the service continues processing the next work item.")]
    [SuppressMessage("ReSharper", "CatchAllClause",
        Justification =
            "Catch all exceptions to prevent the background service from crashing. Exceptions are logged and the service continues processing the next work item.")]
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            BackgroundWorkItem? workItem = null;
            try
            {
                workItem = await _queue.DequeueAsync(stoppingToken).ConfigureAwait(false);
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
internal sealed record BackgroundWorkItem(
    Func<IServiceProvider, CancellationToken, Task> ExecuteAsync,
    string BackgroundTaskType,
    string RequestType);