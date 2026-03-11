using template.net10.api.Hubs.User;

namespace template.net10.api.Hubs.Extensions;

/// <summary>
///     Provides extension methods for <see cref="WebApplication"/> to configure SignalR hub endpoint mappings.
/// </summary>
internal static class WebApplicationExtensions
{
    extension(WebApplication app)
    {
        /// <summary>
        ///     Maps all SignalR hub endpoints to the application pipeline with stateful reconnect support.
        /// </summary>
        internal void ConfigureHubs()
        {
            app.MapHub<UserHub>(ApiRoutes.UsersHub.PathHub, static options => options.AllowStatefulReconnects = true);
        }
    }
}