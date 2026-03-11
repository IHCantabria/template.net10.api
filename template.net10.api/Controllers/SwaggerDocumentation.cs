namespace template.net10.api.Controllers;

/// <summary>
///     Provides constant strings used for Swagger/OpenAPI documentation across all API endpoints.
/// </summary>
internal static class SwaggerDocumentation
{
    /// <summary>
    ///     Swagger documentation constants for the Identity controller endpoints.
    /// </summary>
    internal static class Identity
    {
        /// <summary>
        ///     Swagger tag description for the Identity controller.
        /// </summary>
        internal const string ControllerDescription = "Identity Controller";

        /// <summary>
        ///     Swagger documentation constants for the Login operation.
        /// </summary>
        internal static class Login
        {
            /// <summary>
            ///     Operation summary for the Login endpoint.
            /// </summary>
            internal const string Summary = "Login User.";

            /// <summary>
            ///     Operation description for the Login endpoint.
            /// </summary>
            internal const string Description = "Log in the system with credentials.";

            /// <summary>
            ///     Operation identifier for the Login endpoint.
            /// </summary>
            internal const string Id = "LoginAsync";

            /// <summary>
            ///     Response description for a successful (200 OK) Login request.
            /// </summary>
            internal const string Ok = "Return the id token.";

            /// <summary>
            ///     Response description for a bad request (400) on the Login endpoint.
            /// </summary>
            internal const string BadRequest =
                "Unable to log in the system due to a client query error.";

            /// <summary>
            ///     Response description for a forbidden (403) response on the Login endpoint.
            /// </summary>
            internal const string Forbidden =
                "Unable to log in the system using the credentials provided, invalid password.";

            /// <summary>
            ///     Response description for a not found (404) response on the Login endpoint.
            /// </summary>
            internal const string NotFound =
                "Unable to log in the system using the credentials provided, the user is not present in the system.";

            /// <summary>
            ///     Response description for an unprocessable entity (422) response on the Login endpoint.
            /// </summary>
            internal const string UnprocessableEntity =
                "Unable to log in the system due to a data incompatibility in the client payload. Please review the payload and fix the errors before retry the request.";
        }

        /// <summary>
        ///     Swagger documentation constants for the Access operation.
        /// </summary>
        internal static class Access
        {
            /// <summary>
            ///     Operation summary for the Access endpoint.
            /// </summary>
            internal const string Summary = "Access User.";

            /// <summary>
            ///     Operation description for the Access endpoint.
            /// </summary>
            internal const string Description = "Request access in the system with id Token.";

            /// <summary>
            ///     Operation identifier for the Access endpoint.
            /// </summary>
            internal const string Id = "AccessAsync";

            /// <summary>
            ///     Response description for a successful (200 OK) Access request.
            /// </summary>
            internal const string Ok = "Return the access token.";

            /// <summary>
            ///     Response description for a bad request (400) on the Access endpoint.
            /// </summary>
            internal const string BadRequest =
                "Unable to get the required access token due to a client query error.";

            /// <summary>
            ///     Response description for a forbidden (403) response on the Access endpoint.
            /// </summary>
            internal const string Forbidden =
                "Unable to get the required access token, you dont have the required authentication level.";

            /// <summary>
            ///     Response description for a not found (404) response on the Access endpoint.
            /// </summary>
            internal const string NotFound =
                "Unable to get the required access token, the scope is not present in the system.";

            /// <summary>
            ///     Response description for an unprocessable entity (422) response on the Access endpoint.
            /// </summary>
            internal const string UnprocessableEntity =
                "Unable to get the required access token due to a data incompatibility in the client payload. Please review the payload and fix the errors before retry the request.";
        }
    }

    /// <summary>
    ///     Swagger documentation constants for the Users controller endpoints.
    /// </summary>
    internal static class Users
    {
        /// <summary>
        ///     Swagger tag description for the Users controller.
        /// </summary>
        internal const string ControllerDescription = "Users Controller";

        /// <summary>
        ///     Swagger documentation constants for the Create User operation.
        /// </summary>
        internal static class CreateUser
        {
            /// <summary>
            ///     Operation summary for the Create User endpoint.
            /// </summary>
            internal const string Summary = "Create User.";

            /// <summary>
            ///     Operation description for the Create User endpoint.
            /// </summary>
            internal const string Description = "Create a new User in the system.";

            /// <summary>
            ///     Operation identifier for the Create User endpoint.
            /// </summary>
            internal const string Id = "CreateUserAsync";

            /// <summary>
            ///     Response description for a successful creation (201 Created) on the Create User endpoint.
            /// </summary>
            internal const string Created = "Return the user created in the system.";

            /// <summary>
            ///     Response description for a bad request (400) on the Create User endpoint.
            /// </summary>
            internal const string BadRequest =
                "Unable to create the user due to a client payload error. Please review the payload and fix the errors before retry the request.";

            /// <summary>
            ///     Response description for a not found (404) response on the Create User endpoint.
            /// </summary>
            internal const string NotFound =
                "Unable to create the user due to mistmaching data in the client payload. Please review the payload and fix the errors before retry the request.";

            /// <summary>
            ///     Response description for an unprocessable entity (422) response on the Create User endpoint.
            /// </summary>
            internal const string UnprocessableEntity =
                "Unable to create the user due to a data incompatibility in the client payload. Please review the payload and fix the errors before retry the request.";

            /// <summary>
            ///     Response description for a conflict (409) response on the Create User endpoint.
            /// </summary>
            internal const string Conflict =
                "Unable to create the user due to a conflict with it's current state. Please review the payload and fix the errors before retry the request.";
        }

        /// <summary>
        ///     Swagger documentation constants for the Update User operation.
        /// </summary>
        internal static class UpdateUser
        {
            /// <summary>
            ///     Operation summary for the Update User endpoint.
            /// </summary>
            internal const string Summary = "Update User.";

            /// <summary>
            ///     Operation description for the Update User endpoint.
            /// </summary>
            internal const string Description = "Update a User in the system.";

            /// <summary>
            ///     Operation identifier for the Update User endpoint.
            /// </summary>
            internal const string Id = "UpdateUserAsync";

            /// <summary>
            ///     Response description for a successful (200 OK) Update User request.
            /// </summary>
            internal const string Ok = "Return the user updated in the system.";

            /// <summary>
            ///     Response description for a bad request (400) on the Update User endpoint.
            /// </summary>
            internal const string BadRequest =
                "Unable to update the user due to a client payload error. Please review the payload and fix the errors before retry the request.";

            /// <summary>
            ///     Response description for a not found (404) response on the Update User endpoint.
            /// </summary>
            internal const string NotFound =
                "Unable to update the user due to mistmaching data in the client payload. Please review the payload and fix the errors before retry the request.";

            /// <summary>
            ///     Response description for an unprocessable entity (422) response on the Update User endpoint.
            /// </summary>
            internal const string UnprocessableEntity =
                "Unable to update the user due to a data incompatibility in the client payload. Please review the payload and fix the errors before retry the request.";

            /// <summary>
            ///     Response description for a conflict (409) response on the Update User endpoint.
            /// </summary>
            internal const string Conflict =
                "Unable to update the user due to a conflict with it's current state. Please review the payload and fix the errors before retry the request.";
        }

        /// <summary>
        ///     Swagger documentation constants for the Delete User operation.
        /// </summary>
        internal static class DeleteUser
        {
            /// <summary>
            ///     Operation summary for the Delete User endpoint.
            /// </summary>
            internal const string Summary = "Delete User.";

            /// <summary>
            ///     Operation description for the Delete User endpoint.
            /// </summary>
            internal const string Description = "Delete a User in the system.";

            /// <summary>
            ///     Operation identifier for the Delete User endpoint.
            /// </summary>
            internal const string Id = "DeleteUserAsync";

            /// <summary>
            ///     Response description for a successful (200 OK) Delete User request.
            /// </summary>
            internal const string Ok = "Return the user deleted in the system.";

            /// <summary>
            ///     Response description for a bad request (400) on the Delete User endpoint.
            /// </summary>
            internal const string BadRequest =
                "Unable to delete the user due to a client payload error. Please review the payload and fix the errors before retry the request.";

            /// <summary>
            ///     Response description for a not found (404) response on the Delete User endpoint.
            /// </summary>
            internal const string NotFound =
                "Unable to delete the user due to mistmaching data in the client payload. Please review the payload and fix the errors before retry the request.";

            /// <summary>
            ///     Response description for an unprocessable entity (422) response on the Delete User endpoint.
            /// </summary>
            internal const string UnprocessableEntity =
                "Unable to delete the user due to a data incompatibility in the client payload. Please review the payload and fix the errors before retry the request.";

            /// <summary>
            ///     Response description for a conflict (409) response on the Delete User endpoint.
            /// </summary>
            internal const string Conflict =
                "Unable to delete the user due to a conflict with it's current state. Please review the payload and fix the errors before retry the request.";
        }

        /// <summary>
        ///     Swagger documentation constants for the Disable User operation.
        /// </summary>
        internal static class DisableUser
        {
            /// <summary>
            ///     Operation summary for the Disable User endpoint.
            /// </summary>
            internal const string Summary = "Disable User.";

            /// <summary>
            ///     Operation description for the Disable User endpoint.
            /// </summary>
            internal const string Description = "Disable a User in the system.";

            /// <summary>
            ///     Operation identifier for the Disable User endpoint.
            /// </summary>
            internal const string Id = "DisableUserAsync";

            /// <summary>
            ///     Response description for a successful (200 OK) Disable User request.
            /// </summary>
            internal const string Ok = "Return the user disabled in the system.";

            /// <summary>
            ///     Response description for a bad request (400) on the Disable User endpoint.
            /// </summary>
            internal const string BadRequest =
                "Unable to disable the user due to a client payload error. Please review the payload and fix the errors before retry the request.";

            /// <summary>
            ///     Response description for a not found (404) response on the Disable User endpoint.
            /// </summary>
            internal const string NotFound =
                "Unable to disable the user due to mistmaching data in the client payload. Please review the payload and fix the errors before retry the request.";

            /// <summary>
            ///     Response description for an unprocessable entity (422) response on the Disable User endpoint.
            /// </summary>
            internal const string UnprocessableEntity =
                "Unable to disable the user due to a data incompatibility in the client payload. Please review the payload and fix the errors before retry the request.";

            /// <summary>
            ///     Response description for a conflict (409) response on the Disable User endpoint.
            /// </summary>
            internal const string Conflict =
                "Unable to disable the user due to a conflict with it's current state. Please review the payload and fix the errors before retry the request.";
        }

        /// <summary>
        ///     Swagger documentation constants for the Enable User operation.
        /// </summary>
        internal static class EnableUser
        {
            /// <summary>
            ///     Operation summary for the Enable User endpoint.
            /// </summary>
            internal const string Summary = "Enable User.";

            /// <summary>
            ///     Operation description for the Enable User endpoint.
            /// </summary>
            internal const string Description = "Enable a User in the system.";

            /// <summary>
            ///     Operation identifier for the Enable User endpoint.
            /// </summary>
            internal const string Id = "EnableUserAsync";

            /// <summary>
            ///     Response description for a successful (200 OK) Enable User request.
            /// </summary>
            internal const string Ok = "Return the user enabled in the system.";

            /// <summary>
            ///     Response description for a bad request (400) on the Enable User endpoint.
            /// </summary>
            internal const string BadRequest =
                "Unable to enable the user due to a client payload error. Please review the payload and fix the errors before retry the request.";

            /// <summary>
            ///     Response description for a not found (404) response on the Enable User endpoint.
            /// </summary>
            internal const string NotFound =
                "Unable to enable the user due to mistmaching data in the client payload. Please review the payload and fix the errors before retry the request.";

            /// <summary>
            ///     Response description for an unprocessable entity (422) response on the Enable User endpoint.
            /// </summary>
            internal const string UnprocessableEntity =
                "Unable to enable the user due to a data incompatibility in the client payload. Please review the payload and fix the errors before retry the request.";

            /// <summary>
            ///     Response description for a conflict (409) response on the Enable User endpoint.
            /// </summary>
            internal const string Conflict =
                "Unable to enable the user due to a conflict with it's current state. Please review the payload and fix the errors before retry the request.";
        }

        /// <summary>
        ///     Swagger documentation constants for the Reset User Password operation.
        /// </summary>
        internal static class ResetUserPassword
        {
            /// <summary>
            ///     Operation summary for the Reset User Password endpoint.
            /// </summary>
            internal const string Summary = "Reset User Password.";

            /// <summary>
            ///     Operation description for the Reset User Password endpoint.
            /// </summary>
            internal const string Description = "Reset a User password in the system.";

            /// <summary>
            ///     Operation identifier for the Reset User Password endpoint.
            /// </summary>
            internal const string Id = "ResetUserPasswordAsync";

            /// <summary>
            ///     Response description for a successful (200 OK) Reset User Password request.
            /// </summary>
            internal const string Ok = "Return the new user credentials in the system.";

            /// <summary>
            ///     Response description for a bad request (400) on the Reset User Password endpoint.
            /// </summary>
            internal const string BadRequest =
                "Unable to reset the user password due to a client payload error. Please review the payload and fix the errors before retry the request.";

            /// <summary>
            ///     Response description for a not found (404) response on the Reset User Password endpoint.
            /// </summary>
            internal const string NotFound =
                "Unable to reset the user password due to mistmaching data in the client payload. Please review the payload and fix the errors before retry the request.";

            /// <summary>
            ///     Response description for an unprocessable entity (422) response on the Reset User Password endpoint.
            /// </summary>
            internal const string UnprocessableEntity =
                "Unable to reset the user password due to a data incompatibility in the client payload. Please review the payload and fix the errors before retry the request.";
        }

        /// <summary>
        ///     Swagger documentation constants for the Get Users operation.
        /// </summary>
        internal static class GetUsers
        {
            /// <summary>
            ///     Operation summary for the Get Users endpoint.
            /// </summary>
            internal const string Summary = "Get Users.";

            /// <summary>
            ///     Operation description for the Get Users endpoint.
            /// </summary>
            internal const string Description = "Get the users availables in the system.";

            /// <summary>
            ///     Operation identifier for the Get Users endpoint.
            /// </summary>
            internal const string Id = "GetUsersAsync";

            /// <summary>
            ///     Response description for a successful (200 OK) Get Users request.
            /// </summary>
            internal const string Ok = "Return the uses availables in the system.";
        }

        /// <summary>
        ///     Swagger documentation constants for the Get User operation.
        /// </summary>
        internal static class GetUser
        {
            /// <summary>
            ///     Operation summary for the Get User endpoint.
            /// </summary>
            internal const string Summary = "Get User.";

            /// <summary>
            ///     Operation description for the Get User endpoint.
            /// </summary>
            internal const string Description = "Get data about a user based on its Key(uuid).";

            /// <summary>
            ///     Operation identifier for the Get User endpoint.
            /// </summary>
            internal const string Id = "GetUserInfoAsync";

            /// <summary>
            ///     Response description for a successful (200 OK) Get User request.
            /// </summary>
            internal const string Ok = "Return the user with the specified key in the system.";

            /// <summary>
            ///     Response description for a bad request (400) on the Get User endpoint.
            /// </summary>
            internal const string BadRequest =
                "Unable to get the user due to a client payload error. Please review the payload and fix the errors before retry the request.";

            /// <summary>
            ///     Response description for a not found (404) response on the Get User endpoint.
            /// </summary>
            internal const string NotFound =
                "Unable to get the user due to mistmaching data in the client query. The User key is not present in the system.";
        }

        /// <summary>
        ///     Swagger documentation constants for the Get User Events operation.
        /// </summary>
        internal static class GetUserEvents
        {
            /// <summary>
            ///     Operation summary for the Get User Events endpoint.
            /// </summary>
            internal const string Summary = "Get the User's Events.";

            /// <summary>
            ///     Operation description for the Get User Events endpoint.
            /// </summary>
            internal const string Description = "Get the user's events in the system.";

            /// <summary>
            ///     Operation identifier for the Get User Events endpoint.
            /// </summary>
            internal const string Id = "GetUserEventsAsync";

            /// <summary>
            ///     Response description for a successful (200 OK) Get User Events request.
            /// </summary>
            internal const string Ok = "Return the user's events in the system.";
        }
    }

    /// <summary>
    ///     Swagger documentation constants for the System controller endpoints.
    /// </summary>
    internal static class System
    {
        /// <summary>
        ///     Swagger tag description for the System controller.
        /// </summary>
        internal const string ControllerDescription = "System Controller";

        /// <summary>
        ///     Swagger documentation constants for the Get Error Codes operation.
        /// </summary>
        internal static class GetErrorCodes
        {
            /// <summary>
            ///     Operation summary for the Get Error Codes endpoint.
            /// </summary>
            internal const string Summary = "Get Error Codes.";

            /// <summary>
            ///     Operation description for the Get Error Codes endpoint.
            /// </summary>
            internal const string Description = "Get the error codes and their description managed for the system.";

            /// <summary>
            ///     Operation identifier for the Get Error Codes endpoint.
            /// </summary>
            internal const string Id = "GetErrorCodesAsync";

            /// <summary>
            ///     Response description for a successful (200 OK) Get Error Codes request.
            /// </summary>
            internal const string Ok = "Return the error codes and their description managed for the system.";
        }

        /// <summary>
        ///     Swagger documentation constants for the Get Version operation.
        /// </summary>
        internal static class GetVersion
        {
            /// <summary>
            ///     Operation summary for the Get Version endpoint.
            /// </summary>
            internal const string Summary = "Get Version";

            /// <summary>
            ///     Operation description for the Get Version endpoint.
            /// </summary>
            internal const string Description = "Get the current system version.";

            /// <summary>
            ///     Operation identifier for the Get Version endpoint.
            /// </summary>
            internal const string Id = "GetVersionAsync";

            /// <summary>
            ///     Response description for a successful (200 OK) Get Version request.
            /// </summary>
            internal const string Ok = "Return the current system version.";
        }
    }

    /// <summary>
    ///     Swagger documentation constants for global API response filter descriptions.
    /// </summary>
    internal static class Filter
    {
        /// <summary>
        ///     Description for an HTTP 401 Unauthorized response triggered by authentication failure.
        /// </summary>
        internal const string AuthorizationErrorDescription =
            "Unable to execute the requested operation due to a authorization error. Please, log in the system with your user and get a valid access_token before trying again. if the error persist contact with IH-IT.";

        /// <summary>
        ///     Description for an HTTP 403 Forbidden response triggered by insufficient privileges.
        /// </summary>
        internal const string ForbiddenErrorDescription =
            "Unable to execute the requested operation due to a forbidden error. Your current user don't have privileges to execute the requested operation.";

        /// <summary>
        ///     Description for an HTTP 500 Internal Server Error response.
        /// </summary>
        internal const string InternalServerErrorDescription =
            "Unable to execute the requested operation due to a server error. Please, try again after a couple of mins. if the error persist contact with IH-IT.";

        /// <summary>
        ///     Description for an HTTP 408 Request Timeout response.
        /// </summary>
        internal const string RequestTimeoutErrorDescription =
            "Unable to execute the requested operation due to a request timeout issue, please retry the request.";
    }
}