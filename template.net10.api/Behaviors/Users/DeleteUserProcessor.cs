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
///     ADD DOCUMENTATION
/// </summary>
internal sealed class DeleteUserProcessor(
    IHubContext<UserHub, IUserHub> hubContext,
    IStringLocalizer<ResourceMain> localizer)
    : IRequestPostProcessor<CommandDeleteUser, LanguageExt.Common.Result<User>>

{
    /// <summary>
    ///     ADD DOCUMENTATION
    /// </summary>
    private readonly IHubContext<UserHub, IUserHub> _hubContext =
        hubContext ?? throw new ArgumentNullException(nameof(hubContext));

    /// <summary>
    ///     ADD DOCUMENTATION
    /// </summary>
    private readonly IStringLocalizer<ResourceMain> _localizer =
        localizer ?? throw new ArgumentNullException(nameof(localizer));

    /// <summary>
    ///     ADD DOCUMENTATION
    /// </summary>
    /// <exception cref="ResultSuccessInvalidOperationException">
    ///     Result is not a success! Use ExtractException method instead and Check the
    ///     state of Result with IsSuccess or IsFaulted before use this method or ExtractException method
    /// </exception>
    public async Task Process(CommandDeleteUser request, LanguageExt.Common.Result<User> response,
        CancellationToken cancellationToken)
    {
        if (response.IsFaulted) return;

        await SendEventNotificationAsync(response.ExtractData()).ConfigureAwait(false);
    }

    /// <summary>
    ///     ADD DOCUMENTATION
    /// </summary>
    private Task SendEventNotificationAsync(User data)
    {
        return _hubContext.Clients.All.DeletedUser(new UserHubDeletedUserMessageResource
        {
            Message = _localizer["UserHubDeletedUserMsg"],
            Uuid = data.Uuid.ToString(CultureInfo.InvariantCulture.ToString())
        });
    }
}