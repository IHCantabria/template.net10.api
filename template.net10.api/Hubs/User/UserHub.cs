using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Localization;
using template.net10.api.Business;
using template.net10.api.Core.Contracts;
using template.net10.api.Hubs.User.Interfaces;
using template.net10.api.Localize.Resources;

namespace template.net10.api.Hubs.User;

/// <summary>
///     SignalR hub that handles real-time user event notifications, requiring user read authorization.
/// </summary>
[SuppressMessage("ReSharper", "AsyncApostle.AsyncMethodNamingHighlighting",
    Justification =
        "RPC methods should not be suffixed with 'Async' as this would leak an implementation detail to your users.")]
[SuppressMessage("Pragma", "VSTHRD200",
    Justification =
        "RPC methods should not be suffixed with 'Async' as this would leak an implementation detail to your users.")]
[SuppressMessage(
    "Design",
    "CA1515:Consider making public types internal",
    Justification =
        "Public visibility is required because this Hub is part of the application messaging contract (MediatR).")]
[MustDisposeResource]
[Authorize(Policy = PoliciesConstants.UserReadPolicy)]
public sealed class UserHub(IStringLocalizer<ResourceMain> localizer) : Hub<IUserHub>
{
    /// <summary>
    ///     String localizer for producing localized hub notification messages.
    /// </summary>
    private readonly IStringLocalizer<ResourceMain> _localizer =
        localizer ?? throw new ArgumentNullException(nameof(localizer));

    /// <summary>
    ///     Called when a new client connects to the hub. Sends a connection online notification to the caller.
    /// </summary>
    /// <returns>A task representing the asynchronous connection handling operation.</returns>
    public override async Task OnConnectedAsync()
    {
        await Clients.Caller.ConnectionOnline(new HubInfoMessageResource
        {
            Message = _localizer["UserHubConnectionOnlineMsg"],
            ConnectionId = Context.ConnectionId
        }).ConfigureAwait(false);

        await base.OnConnectedAsync().ConfigureAwait(false);
    }

    /// <summary>
    ///     Client-invokable method to check the current connection status. Returns the status to the caller.
    /// </summary>
    /// <returns>A task representing the asynchronous status check operation.</returns>
    public Task CheckConnectionStatus()
    {
        return Clients.Caller.ConnectionStatus(new HubInfoMessageResource
        {
            Message = _localizer["UserHubConnectionStatusMsg"],
            ConnectionId = Context.ConnectionId
        });
    }
}