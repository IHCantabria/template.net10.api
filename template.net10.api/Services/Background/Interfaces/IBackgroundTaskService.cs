namespace template.net10.api.Services.Background.Interfaces;

/// <summary>
///     Marker interface for the long-running hosted background service that processes
///     <see cref="BackgroundWorkItem" /> entries from the <see cref="IBackgroundTaskQueue" />.
///     Implementing classes run as <see cref="Microsoft.Extensions.Hosting.IHostedService" />
///     singletons for the lifetime of the application, executing queued work items sequentially
///     within isolated DI scopes.
/// </summary>
internal interface IBackgroundTaskService;