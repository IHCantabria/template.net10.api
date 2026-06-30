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
///     after a user is successfully updated.
/// </summary>
internal sealed class UpdateUserProcessor(
    IBackgroundTaskQueue queue,
    IHubContext<UserHub, IUserHub> hubContext,
    IStringLocalizer<ResourceMain> localizer,
    ILogger<UpdateUserProcessor> logger)
    : IRequestPostProcessor<CommandUpdateUser, LanguageExt.Common.Result<User>>
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
    private readonly ILogger<UpdateUserProcessor> _logger =
        logger ?? throw new ArgumentNullException(nameof(logger));

    /// <summary>
    ///     Background task queue for fire-and-forget post-process work.
    /// </summary>
    private readonly IBackgroundTaskQueue _queue =
        queue ?? throw new ArgumentNullException(nameof(queue));

    /// <inheritdoc />
    public async Task Process(CommandUpdateUser request, LanguageExt.Common.Result<User> response,
        CancellationToken cancellationToken)
    {
        if (response.IsFaulted) return;

        var start = _logger.PrepareLogHandlingPostProcess(nameof(UpdateUserProcessor), nameof(CommandUpdateUser));
        await ExecutePostProcessAsync(response).ConfigureAwait(false);
        _logger.PrepareLogHandledPostProcess(nameof(UpdateUserProcessor), nameof(CommandUpdateUser), start);
    }

    /// <summary>
    ///     Extracts the UUID of the updated user from <paramref name="response" /> and enqueues a
    ///     fire-and-forget <see cref="BackgroundWorkItem" /> that broadcasts a SignalR notification to all
    ///     connected clients via <see cref="SendNotificationAsync" />.
    /// </summary>
    /// <param name="response">
    ///     The successful result of the <see cref="CommandUpdateUser" /> command containing the updated
    ///     <see cref="User" />.
    /// </param>
    private async Task ExecutePostProcessAsync(LanguageExt.Common.Result<User> response)
    {
        var uuid = response.ExtractData().Uuid;
        await _queue.EnqueueAsync(new BackgroundWorkItem(
            (_, _) => SendNotificationAsync(uuid),
            nameof(UpdateUserProcessor),
            nameof(CommandUpdateUser),
            BackgroundWorkCategory.Notification), CancellationToken.None).ConfigureAwait(false);
    }

    /// <summary>
    ///     Sends a SignalR notification to all connected clients informing them that a user was updated.
    /// </summary>
    private Task SendNotificationAsync(Guid uuid)
    {
        return _hubContext.Clients.All.UpdatedUser(new UserHubUpdatedUserMessageResource
        {
            Message = _localizer["UserHubUpdatedUserMsg"],
            Uuid = uuid.ToString("D")
        });
    }
}