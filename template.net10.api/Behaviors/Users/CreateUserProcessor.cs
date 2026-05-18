using MediatR.Pipeline;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Localization;
using template.net10.api.Behaviors.Extensions;
using template.net10.api.Core.Extensions;
using template.net10.api.Features.Commands;
using template.net10.api.Hubs.User;
using template.net10.api.Hubs.User.Contracts;
using template.net10.api.Hubs.User.Interfaces;
using template.net10.api.Localize.Resources;
using template.net10.api.Persistence.Models;
using template.net10.api.Services.Background;
using template.net10.api.Services.Background.Interfaces;

namespace template.net10.api.Behaviors.Users;

/// <summary>
///     MediatR post-processor that enqueues a fire-and-forget SignalR notification
///     after a user is successfully created.
/// </summary>
internal sealed class CreateUserProcessor(
    IBackgroundTaskQueue queue,
    IHubContext<UserHub, IUserHub> hubContext,
    IStringLocalizer<ResourceMain> localizer,
    ILogger<CreateUserProcessor> logger)
    : IRequestPostProcessor<CommandCreateUser, LanguageExt.Common.Result<User>>
{
    /// <summary>
    ///     SignalR hub context for broadcasting user-related events to connected clients.
    /// </summary>
    private readonly IHubContext<UserHub, IUserHub> _hubContext =
        hubContext ?? throw new ArgumentNullException(nameof(hubContext));

    /// <summary>
    ///     String localizer for producing localized notification messages.
    /// </summary>
    private readonly IStringLocalizer<ResourceMain> _localizer =
        localizer ?? throw new ArgumentNullException(nameof(localizer));

    /// <summary>
    ///     Logger instance scoped to this post-processor.
    /// </summary>
    private readonly ILogger<CreateUserProcessor> _logger =
        logger ?? throw new ArgumentNullException(nameof(logger));

    /// <summary>
    ///     Background task queue for fire-and-forget post-process work.
    /// </summary>
    private readonly IBackgroundTaskQueue _queue =
        queue ?? throw new ArgumentNullException(nameof(queue));

    /// <inheritdoc />
    public Task Process(CommandCreateUser request, LanguageExt.Common.Result<User> response,
        CancellationToken cancellationToken)
    {
        if (response.IsFaulted) return Task.CompletedTask;

        var start = _logger.PrepareLogHandlingPostProcess(nameof(CreateUserProcessor), nameof(CommandCreateUser));
        ExecutePostProcess(response);
        _logger.PrepareLogHandledPostProcess(nameof(CreateUserProcessor), nameof(CommandCreateUser), start);
        return Task.CompletedTask;
    }

    /// <summary>
    ///     Extracts the UUID of the newly created user from <paramref name="response" /> and enqueues a
    ///     fire-and-forget <see cref="BackgroundWorkItem" /> that broadcasts a SignalR notification to all
    ///     connected clients via <see cref="SendNotificationAsync" />.
    /// </summary>
    /// <param name="response">
    ///     The successful result of the <see cref="CommandCreateUser" /> command containing the created
    ///     <see cref="User" />.
    /// </param>
    private void ExecutePostProcess(LanguageExt.Common.Result<User> response)
    {
        var uuid = response.ExtractData().Uuid;
        _queue.Enqueue(new BackgroundWorkItem(
            (_, _) => SendNotificationAsync(uuid),
            nameof(CreateUserProcessor),
            nameof(CommandCreateUser)));
    }

    /// <summary>
    ///     Sends a SignalR notification to all connected clients informing them that a new user was created.
    /// </summary>
    private Task SendNotificationAsync(Guid uuid)
    {
        return _hubContext.Clients.All.CreatedUser(new UserHubCreatedUserMessageResource
        {
            Message = _localizer["UserHubCreatedUserMsg"],
            Uuid = uuid.ToString("D")
        });
    }
}