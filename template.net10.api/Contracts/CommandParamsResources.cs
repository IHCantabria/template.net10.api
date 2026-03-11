using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using template.net10.api.Core.Interfaces;

namespace template.net10.api.Contracts;

/// <summary>
///     Represents the API contract for creating a new user.
/// </summary>
[SuppressMessage("Design",
    "CA1515:Consider making public types internal",
    Justification =
        "Contracts must remain public to allow proper discovery and schema generation by OpenAPI.")]
public sealed partial record CommandCreateUserParamsResource : IPublicApiContract,
    IEqualityOperators<CommandCreateUserParamsResource, CommandCreateUserParamsResource, bool>
{
    /// <summary>
    ///     Gets the username for the new user account.
    /// </summary>
    [JsonRequired]
    public required string Username { get; init; }

    /// <summary>
    ///     Gets the email address for the new user account.
    /// </summary>
    [JsonRequired]
    [EmailAddress]
    public required string Email { get; init; }

    /// <summary>
    ///     Gets a value indicating whether the user account should be initially disabled.
    /// </summary>
    [JsonRequired]
    public required bool IsDisabled { get; init; }

    /// <summary>
    ///     Gets the password for the new user account.
    /// </summary>
    [JsonRequired]
    public required string Password { get; init; }

    /// <summary>
    ///     Gets the password confirmation, which must match the password.
    /// </summary>
    [JsonRequired]
    public required string ConfirmPassword { get; init; }

    /// <summary>
    ///     Gets the optional first name of the new user.
    /// </summary>
    public string? FirstName { get; init; }

    /// <summary>
    ///     Gets the optional last name of the new user.
    /// </summary>
    public string? LastName { get; init; }

    /// <summary>
    ///     Gets the role identifier assigned to the new user.
    /// </summary>
    [JsonRequired]
    public required short RoleId { get; init; }
}

/// <summary>
///     Represents the API contract for updating an existing user.
/// </summary>
[SuppressMessage("Design",
    "CA1515:Consider making public types internal",
    Justification =
        "Contracts must remain public to allow proper discovery and schema generation by OpenAPI.")]
public sealed partial record CommandUpdateUserParamsResource : IPublicApiContract,
    IEqualityOperators<CommandUpdateUserParamsResource, CommandUpdateUserParamsResource, bool>
{
    /// <summary>
    ///     Gets the unique identifier of the user to update.
    /// </summary>
    [Required]
    [FromRoute(Name = "user-key")]
    public required Guid Key { get; init; }

    /// <summary>
    ///     Gets the request body containing the updated user properties.
    /// </summary>
    [Required]
    [FromBody]
    public required CommandUpdateUserParamsBodyResource Body { get; init; }
}

/// <summary>
///     Represents the request body for updating user properties.
/// </summary>
[SuppressMessage("Design",
    "CA1515:Consider making public types internal",
    Justification =
        "Contracts must remain public to allow proper discovery and schema generation by OpenAPI.")]
public sealed record CommandUpdateUserParamsBodyResource : IPublicApiContract,
    IEqualityOperators<CommandUpdateUserParamsBodyResource, CommandUpdateUserParamsBodyResource, bool>
{
    /// <summary>
    ///     Gets the updated username, or null to leave unchanged.
    /// </summary>
    public string? Username { get; init; }

    /// <summary>
    ///     Gets a value indicating whether the user account should be disabled.
    /// </summary>
    public bool IsDisabled { get; init; }

    /// <summary>
    ///     Gets the updated email address, or null to leave unchanged.
    /// </summary>
    public string? Email { get; init; }

    /// <summary>
    ///     Gets the updated role identifier, or null to leave unchanged.
    /// </summary>
    public short? RoleId { get; init; }

    /// <summary>
    ///     Gets the updated first name, or null to leave unchanged.
    /// </summary>
    public string? FirstName { get; init; }

    /// <summary>
    ///     Gets the updated last name, or null to leave unchanged.
    /// </summary>
    public string? LastName { get; init; }
}

/// <summary>
///     Represents the API contract for resetting a user's password.
/// </summary>
[SuppressMessage("Design",
    "CA1515:Consider making public types internal",
    Justification =
        "Contracts must remain public to allow proper discovery and schema generation by OpenAPI.")]
public sealed partial record CommandResetUserPasswordParamsResource : IPublicApiContract,
    IEqualityOperators<CommandResetUserPasswordParamsResource, CommandResetUserPasswordParamsResource, bool>
{
    /// <summary>
    ///     Gets the unique identifier of the user whose password will be reset.
    /// </summary>
    [Required]
    [FromRoute(Name = "user-key")]
    public required Guid Key { get; init; }

    /// <summary>
    ///     Gets the request body containing the new password data.
    /// </summary>
    [Required]
    [FromBody]
    public required CommandResetUserPasswordParamsBodyResource Body { get; init; }
}

/// <summary>
///     Represents the request body for resetting a user's password.
/// </summary>
[SuppressMessage("Design",
    "CA1515:Consider making public types internal",
    Justification =
        "Contracts must remain public to allow proper discovery and schema generation by OpenAPI.")]
public sealed record CommandResetUserPasswordParamsBodyResource : IPublicApiContract,
    IEqualityOperators<CommandResetUserPasswordParamsBodyResource, CommandResetUserPasswordParamsBodyResource, bool>
{
    /// <summary>
    ///     Gets the new password for the user account.
    /// </summary>
    [JsonRequired]
    public required string Password { get; init; }

    /// <summary>
    ///     Gets the confirmation of the new password, which must match the password.
    /// </summary>
    [JsonRequired]
    public required string ConfirmPassword { get; init; }
}

/// <summary>
///     Represents the API contract for disabling a user account.
/// </summary>
[SuppressMessage("Design",
    "CA1515:Consider making public types internal",
    Justification =
        "Contracts must remain public to allow proper discovery and schema generation by OpenAPI.")]
public sealed partial record CommandDisableUserParamsResource : IPublicApiContract,
    IEqualityOperators<CommandDisableUserParamsResource, CommandDisableUserParamsResource, bool>
{
    /// <summary>
    ///     Gets the unique identifier of the user to disable.
    /// </summary>
    [Required]
    [FromRoute(Name = "user-key")]
    public required Guid Key { get; init; }
}

/// <summary>
///     Represents the API contract for enabling a user account.
/// </summary>
[SuppressMessage("Design",
    "CA1515:Consider making public types internal",
    Justification =
        "Contracts must remain public to allow proper discovery and schema generation by OpenAPI.")]
public sealed partial record CommandEnableUserParamsResource : IPublicApiContract,
    IEqualityOperators<CommandEnableUserParamsResource, CommandEnableUserParamsResource, bool>
{
    /// <summary>
    ///     Gets the unique identifier of the user to enable.
    /// </summary>
    [Required]
    [FromRoute(Name = "user-key")]
    public required Guid Key { get; init; }
}

/// <summary>
///     Represents the API contract for deleting a user.
/// </summary>
[SuppressMessage("Design",
    "CA1515:Consider making public types internal",
    Justification =
        "Contracts must remain public to allow proper discovery and schema generation by OpenAPI.")]
public sealed partial record CommandDeleteUserParamsResource : IPublicApiContract,
    IEqualityOperators<CommandDeleteUserParamsResource, CommandDeleteUserParamsResource, bool>
{
    /// <summary>
    ///     Gets the unique identifier of the user to delete.
    /// </summary>
    [Required]
    [FromRoute(Name = "user-key")]
    public required Guid Key { get; init; }
}