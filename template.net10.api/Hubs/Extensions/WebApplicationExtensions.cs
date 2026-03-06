using template.net10.api.Hubs.User;

namespace template.net10.api.Hubs.Extensions;

/// <summary>
///     ADD DOCUMENTATION
/// </summary>
internal static class WebApplicationExtensions
{
    extension(WebApplication app)
    {
        /// <summary>
        ///     ADD DOCUMENTATION
        /// </summary>
        internal void ConfigureHubs()
        {
            app.MapHub<UserHub>(ApiRoutes.UsersHub.PathHub, static options => options.AllowStatefulReconnects = true);
        }
    }
}