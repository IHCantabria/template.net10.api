using System.Globalization;
using MediatR.Pipeline;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Localization;
using template.net10.api.Core.Exceptions;
using template.net10.api.Core.Extensions;
using template.net10.api.Features.Commands;
using template.net10.api.Hubs.User;
using template.net10.api.Hubs.User.Contracts;
using template.net10.api.Hubs.User.Interfaces;
using template.net10.api.Localize.Resources;
using template.net10.api.Persistence.Models;

namespace template.net10.api.Behaviors.Users;

/// <summary>
///     MediatR post-processor that sends a SignalR notification to all clients after a user is successfully updated.
/// </summary>
internal sealed class UpdateUserProcessor(
    IHubContext<UserHub, IUserHub> hubContext,
    IStringLocalizer<ResourceMain> localizer)
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
    ///     Processes the response of a <see cref="CommandUpdateUser"/> request and sends a notification if the operation succeeded.
    /// </summary>
    /// <param name="request">The update user command that was executed.</param>
    /// <param name="response">The result containing the updated <see cref="User"/> or a fault.</param>
    /// <param name="cancellationToken">A token to observe for cancellation.</param>
    /// <exception cref="ResultSuccessInvalidOperationException">
    ///     Result is not a success! Use ExtractException method instead and Check the
    ///     state of Result with IsSuccess or IsFaulted before use this method or ExtractException method
    /// </exception>
    public async Task Process(CommandUpdateUser request, LanguageExt.Common.Result<User> response,
        CancellationToken cancellationToken)
    {
        if (response.IsFaulted) return;

        await SendEventNotificationAsync(response.ExtractData()).ConfigureAwait(false);
    }

    /// <summary>
    ///     Sends a SignalR notification to all connected clients informing them that a user was updated.
    /// </summary>
    /// <param name="data">The updated <see cref="User"/> entity.</param>
    /// <returns>A task representing the asynchronous notification operation.</returns>
    private Task SendEventNotificationAsync(User data)
    {
        return _hubContext.Clients.All.UpdatedUser(new UserHubUpdatedUserMessageResource
        {
            Message = _localizer["UserHubUpdatedUserMsg"],
            Uuid = data.Uuid.ToString(CultureInfo.InvariantCulture.ToString())
        });
    }
}