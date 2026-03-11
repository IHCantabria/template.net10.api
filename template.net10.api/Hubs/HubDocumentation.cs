using template.net10.api.Core.Contracts;
using template.net10.api.Hubs.User;
using template.net10.api.Hubs.User.Contracts;
using template.net10.api.Hubs.User.Interfaces;

namespace template.net10.api.Hubs;

/// <summary>
///     Defines the direction of a SignalR hub event for documentation purposes.
/// </summary>
file enum HubEventType
{
    /// <summary>
    ///     Indicates an event invoked by the client on the hub (client-to-server).
    /// </summary>
    ClientCall,

    /// <summary>
    ///     Indicates an event sent from the server to connected clients (server-to-client).
    /// </summary>
    ServerCall
}

/// <summary>
///     Provides structured metadata constants for all SignalR hub events, used for API documentation generation.
/// </summary>
internal static class HubsDocumentation
{
    /// <summary>
    ///     Documentation metadata for user-related SignalR hub events.
    /// </summary>
    internal static class User
    {
        /// <summary>
        ///     Metadata for the client-invoked check connection status hub event.
        /// </summary>
        internal static class CheckConnectionStatus
        {
            /// <summary>
            ///     The hub method name for checking connection status.
            /// </summary>
            internal const string Name = nameof(UserHub.CheckConnectionStatus);

            /// <summary>
            ///     The event direction type (client-to-server call).
            /// </summary>
            internal const string Type = nameof(HubEventType.ClientCall);
        }

        /// <summary>
        ///     Metadata for the server-to-client connection status notification event.
        /// </summary>
        internal static class ConnectionStatus
        {
            /// <summary>
            ///     The hub method name for the connection status notification.
            /// </summary>
            internal const string Name = nameof(IUserHub.ConnectionStatus);

            /// <summary>
            ///     The event direction type (server-to-client call).
            /// </summary>
            internal const string Type = nameof(HubEventType.ServerCall);

            /// <summary>
            ///     The payload field names included in the connection status message.
            /// </summary>
            internal static readonly string[] Fields =
                [nameof(HubInfoMessageResource.ConnectionId), nameof(HubInfoMessageResource.Message)];
        }

        /// <summary>
        ///     Metadata for the server-to-client connection online notification event.
        /// </summary>
        internal static class ConnectionOnline
        {
            /// <summary>
            ///     The hub method name for the connection online notification.
            /// </summary>
            internal const string Name = nameof(IUserHub.ConnectionOnline);

            /// <summary>
            ///     The event direction type (server-to-client call).
            /// </summary>
            internal const string Type = nameof(HubEventType.ServerCall);

            /// <summary>
            ///     The payload field names included in the connection online message.
            /// </summary>
            internal static readonly string[] Fields =
                [nameof(HubInfoMessageResource.ConnectionId), nameof(HubInfoMessageResource.Message)];
        }

        /// <summary>
        ///     Metadata for the server-to-client user created notification event.
        /// </summary>
        internal static class CreatedUser
        {
            /// <summary>
            ///     The hub method name for the user created notification.
            /// </summary>
            internal const string Name = nameof(IUserHub.CreatedUser);

            /// <summary>
            ///     The event direction type (server-to-client call).
            /// </summary>
            internal const string Type = nameof(HubEventType.ServerCall);

            /// <summary>
            ///     The payload field names included in the user created message.
            /// </summary>
            internal static readonly string[] Fields =
            [
                nameof(UserHubCreatedUserMessageResource.Uuid),
                nameof(UserHubCreatedUserMessageResource.Message)
            ];
        }

        /// <summary>
        ///     Metadata for the server-to-client user updated notification event.
        /// </summary>
        internal static class UpdatedUser
        {
            /// <summary>
            ///     The hub method name for the user updated notification.
            /// </summary>
            internal const string Name = nameof(IUserHub.UpdatedUser);

            /// <summary>
            ///     The event direction type (server-to-client call).
            /// </summary>
            internal const string Type = nameof(HubEventType.ServerCall);

            /// <summary>
            ///     The payload field names included in the user updated message.
            /// </summary>
            internal static readonly string[] Fields =
            [
                nameof(UserHubUpdatedUserMessageResource.Uuid),
                nameof(UserHubUpdatedUserMessageResource.Message)
            ];
        }

        /// <summary>
        ///     Metadata for the server-to-client user deleted notification event.
        /// </summary>
        internal static class DeletedUser
        {
            /// <summary>
            ///     The hub method name for the user deleted notification.
            /// </summary>
            internal const string Name = nameof(IUserHub.DeletedUser);

            /// <summary>
            ///     The event direction type (server-to-client call).
            /// </summary>
            internal const string Type = nameof(HubEventType.ServerCall);

            /// <summary>
            ///     The payload field names included in the user deleted message.
            /// </summary>
            internal static readonly string[] Fields =
            [
                nameof(UserHubDeletedUserMessageResource.Uuid),
                nameof(UserHubDeletedUserMessageResource.Message)
            ];
        }
    }
}