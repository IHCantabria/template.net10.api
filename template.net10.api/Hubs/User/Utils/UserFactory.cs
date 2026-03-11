using Microsoft.Extensions.Localization;
using template.net10.api.Core.Contracts;
using template.net10.api.Localize.Resources;

namespace template.net10.api.Hubs.User.Utils;

/// <summary>
///     Provides factory methods to generate SignalR hub event metadata for user-related events.
/// </summary>
internal static class HubUserEvents
{
    /// <summary>
    ///     Builds the collection of all user hub event descriptors for API documentation.
    /// </summary>
    /// <param name="localizer">The string localizer for resolving event description texts.</param>
    /// <param name="fullUrl">The full URL path of the SignalR hub endpoint.</param>
    /// <returns>A collection of <see cref="HubEventResource"/> describing all available user hub events.</returns>
    internal static IEnumerable<HubEventResource> GetEvents(IStringLocalizer<ResourceMain> localizer, string fullUrl)
    {
        return
        [
            new HubEventResource
            {
                Name = HubsDocumentation.User.CheckConnectionStatus.Name,
                Path = fullUrl,
                Type = HubsDocumentation.User.CheckConnectionStatus.Type,
                Description = localizer["GetUserEventsCheckConnectionStatusDescription"],
                Fields = []
            },
            new HubEventResource
            {
                Name = HubsDocumentation.User.ConnectionStatus.Name,
                Path = fullUrl,
                Type = HubsDocumentation.User.ConnectionStatus.Type,
                Description = localizer["GetUserEventsConnectionStatusDescription"],
                Fields = HubsDocumentation.User.ConnectionStatus.Fields
            },
            new HubEventResource
            {
                Name = HubsDocumentation.User.ConnectionOnline.Name,
                Path = fullUrl,
                Type = HubsDocumentation.User.ConnectionOnline.Type,
                Description = localizer["GetUserEventsCheckConnectionStatusDescription"],
                Fields = HubsDocumentation.User.ConnectionOnline.Fields
            },
            new HubEventResource
            {
                Name = HubsDocumentation.User.CreatedUser.Name,
                Path = fullUrl,
                Type = HubsDocumentation.User.CreatedUser.Type,
                Description = localizer["GetUserEventsNewUserDescription"],
                Fields = HubsDocumentation.User.CreatedUser.Fields
            },
            new HubEventResource
            {
                Name = HubsDocumentation.User.UpdatedUser.Name,
                Path = fullUrl,
                Type = HubsDocumentation.User.UpdatedUser.Type,
                Description = localizer["GetUserEventsUpdateUserDescription"],
                Fields = HubsDocumentation.User.UpdatedUser.Fields
            },
            new HubEventResource
            {
                Name = HubsDocumentation.User.DeletedUser.Name,
                Path = fullUrl,
                Type = HubsDocumentation.User.DeletedUser.Type,
                Description = localizer["GetUserEventsDeleteUserDescription"],
                Fields = HubsDocumentation.User.DeletedUser.Fields
            }
        ];
    }
}