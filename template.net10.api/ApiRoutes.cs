namespace template.net10.api;

/// <summary>
///     Defines all API route constants used across controllers and hubs in the application.
/// </summary>
internal static class ApiRoutes
{
    /// <summary>
    ///     Base path prefix for all SignalR hub endpoints.
    /// </summary>
    internal const string HubsAccess = "/hubs";

    /// <summary>
    ///     Route constants for the Identity controller handling authentication operations.
    /// </summary>
    internal static class IdentityController
    {
        /// <summary>
        ///     Base route segment identifying the identity controller.
        /// </summary>
        private const string ControllerIdentity = "identity";

        /// <summary>
        ///     Route path for the identity controller.
        /// </summary>
        internal const string PathController = ControllerIdentity;

        /// <summary>
        ///     Route for the user login endpoint.
        /// </summary>
        internal const string Login = "login";

        /// <summary>
        ///     Route for the access token verification endpoint.
        /// </summary>
        internal const string Access = "access";
    }

    /// <summary>
    ///     Route constants for the Users controller handling user management operations.
    /// </summary>
    internal static class UsersController
    {
        /// <summary>
        ///     Base route segment identifying the users controller.
        /// </summary>
        private const string ControllerIdentity = "users";

        /// <summary>
        ///     Route path for the users controller.
        /// </summary>
        internal const string PathController = ControllerIdentity;

        /// <summary>
        ///     Route for the create user endpoint (POST to controller root).
        /// </summary>
        internal const string CreateUser = "";

        /// <summary>
        ///     Route for updating a specific user, identified by the user key path parameter.
        /// </summary>
        internal const string UpdateUser = "{user-key}";

        /// <summary>
        ///     Route for disabling a specific user, identified by the user key path parameter.
        /// </summary>
        internal const string DisableUser = "{user-key}/disable";

        /// <summary>
        ///     Route for enabling a specific user, identified by the user key path parameter.
        /// </summary>
        internal const string EnableUser = "{user-key}/enable";

        /// <summary>
        ///     Route for resetting a specific user's password, identified by the user key path parameter.
        /// </summary>
        internal const string ResetUserPassword = "{user-key}/reset-password";

        /// <summary>
        ///     Route for deleting a specific user, identified by the user key path parameter.
        /// </summary>
        internal const string DeleteUser = "{user-key}";

        /// <summary>
        ///     Route for retrieving all users (GET to controller root).
        /// </summary>
        internal const string GetUsers = "";

        /// <summary>
        ///     Route for retrieving a specific user, identified by the user key path parameter.
        /// </summary>
        internal const string GetUser = "{user-key}";

        /// <summary>
        ///     Route for the user events endpoint used by SignalR hub subscriptions.
        /// </summary>
        internal const string Hubs = "events";
    }

    /// <summary>
    ///     Route constants for the Users SignalR hub providing real-time user event notifications.
    /// </summary>
    internal static class UsersHub
    {
        /// <summary>
        ///     Name segment identifying the users hub.
        /// </summary>
        private const string HubName = "users";

        /// <summary>
        ///     Full path for the users SignalR hub endpoint.
        /// </summary>
        internal const string PathHub = $"{HubsAccess}/{HubName}";
    }

    /// <summary>
    ///     Route constants for the Health Check controller.
    /// </summary>
    internal static class HealthController
    {
        /// <summary>
        ///     Base route segment for the health controller (mapped to API root).
        /// </summary>
        private const string ControllerIdentity = "";

        /// <summary>
        ///     Route path for the health controller.
        /// </summary>
        internal const string PathController = ControllerIdentity;

        /// <summary>
        ///     Route for the health check endpoint (GET to controller root).
        /// </summary>
        internal const string HealthCheck = "";
    }

    /// <summary>
    ///     Route constants for the System controller providing application metadata.
    /// </summary>
    internal static class SystemController
    {
        /// <summary>
        ///     Base route segment identifying the systems controller.
        /// </summary>
        private const string ControllerIdentity = "systems";

        /// <summary>
        ///     Route path for the systems controller.
        /// </summary>
        internal const string PathController = ControllerIdentity;

        /// <summary>
        ///     Route for retrieving the catalog of application error codes.
        /// </summary>
        internal const string GetErrorCodes = "error-codes";

        /// <summary>
        ///     Route for retrieving the current application version.
        /// </summary>
        internal const string GetVersion = "version";
    }
}