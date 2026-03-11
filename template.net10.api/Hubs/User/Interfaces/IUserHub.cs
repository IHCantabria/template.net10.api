using System.Diagnostics.CodeAnalysis;
using template.net10.api.Core.Contracts;
using template.net10.api.Hubs.User.Contracts;

namespace template.net10.api.Hubs.User.Interfaces;

/// <summary>
///     Defines the server-to-client SignalR hub contract for user-related real-time event notifications.
/// </summary>
[SuppressMessage("ReSharper", "AsyncApostle.AsyncMethodNamingHighlighting",
    Justification =
        "RPC methods should not be suffixed with 'Async' as this would leak an implementation detail to your users.")]
[SuppressMessage("Pragma", "VSTHRD200",
    Justification =
        "RPC methods should not be suffixed with 'Async' as this would leak an implementation detail to your users.")]
[SuppressMessage("Design",
    "CA1515:Consider making public types internal",
    Justification =
        "The interface is part of the public API contract and must remain publicly accessible.")]
public interface IUserHub
{
    /// <summary>
    ///     Notifies connected clients that a new connection has come online.
    /// </summary>
    /// <param name="message">The hub info message containing the connection identifier and status text.</param>
    /// <returns>A task representing the asynchronous notification operation.</returns>
    Task ConnectionOnline(HubInfoMessageResource message);

    /// <summary>
    ///     Notifies connected clients of their current connection status.
    /// </summary>
    /// <param name="message">The hub info message containing the connection identifier and status text.</param>
    /// <returns>A task representing the asynchronous notification operation.</returns>
    Task ConnectionStatus(HubInfoMessageResource message);

    /// <summary>
    ///     Notifies connected clients that a new user has been created.
    /// </summary>
    /// <param name="message">The message containing the UUID and description of the created user.</param>
    /// <returns>A task representing the asynchronous notification operation.</returns>
    Task CreatedUser(UserHubCreatedUserMessageResource message);

    /// <summary>
    ///     Notifies connected clients that a user has been updated.
    /// </summary>
    /// <param name="message">The message containing the UUID and description of the updated user.</param>
    /// <returns>A task representing the asynchronous notification operation.</returns>
    Task UpdatedUser(UserHubUpdatedUserMessageResource message);

    /// <summary>
    ///     Notifies connected clients that a user has been deleted.
    /// </summary>
    /// <param name="message">The message containing the UUID and description of the deleted user.</param>
    /// <returns>A task representing the asynchronous notification operation.</returns>
    Task DeletedUser(UserHubDeletedUserMessageResource message);
}