using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Text.Json.Serialization;
using template.net10.api.Core.Interfaces;

namespace template.net10.api.Contracts;

/// <summary>
///     Represents the response resource containing access token data.
/// </summary>
[SuppressMessage("Design",
    "CA1515:Consider making public types internal",
    Justification =
        "Contracts must remain public to allow proper discovery and schema generation by OpenAPI.")]
public sealed record AccessTokenResource : IPublicApiContract,
    IEqualityOperators<AccessTokenResource, AccessTokenResource, bool>
{
    /// <summary>
    ///     Gets the JWT access token string.
    /// </summary>
    [JsonRequired]
    public required string AccessToken { get; init; }

    /// <summary>
    ///     Gets the type of the access token (e.g., Bearer).
    /// </summary>
    [JsonRequired]
    public required string AccessTokenType { get; init; }

    /// <summary>
    ///     Gets the refresh token string used to renew the access token.
    /// </summary>
    [JsonRequired]
    public required string RefreshToken { get; init; }

    /// <summary>
    ///     Gets the type of the refresh token.
    /// </summary>
    [JsonRequired]
    public required string RefreshTokenType { get; init; }
}

/// <summary>
///     Represents the response resource containing identity token data.
/// </summary>
[SuppressMessage("Design",
    "CA1515:Consider making public types internal",
    Justification =
        "Contracts must remain public to allow proper discovery and schema generation by OpenAPI.")]
public sealed record IdTokenResource : IPublicApiContract,
    IEqualityOperators<IdTokenResource, IdTokenResource, bool>
{
    /// <summary>
    ///     Gets the identity token string.
    /// </summary>
    [JsonRequired]
    public required string IdToken { get; init; }

    /// <summary>
    ///     Gets the type of the identity token.
    /// </summary>
    [JsonRequired]
    public required string IdTokenType { get; init; }
}

/// <summary>
///     Represents the response resource containing detailed user information.
/// </summary>
[SuppressMessage("Design",
    "CA1515:Consider making public types internal",
    Justification =
        "Contracts must remain public to allow proper discovery and schema generation by OpenAPI.")]
public sealed record UserResource : IPublicApiContract, IEqualityOperators<UserResource, UserResource, bool>
{
    /// <summary>
    ///     Gets the unique identifier of the user.
    /// </summary>
    [JsonRequired]
    public required Guid Uuid { get; init; }

    /// <summary>
    ///     Gets the username of the user.
    /// </summary>
    [JsonRequired]
    public required string Username { get; init; }

    /// <summary>
    ///     Gets the email address of the user.
    /// </summary>
    [JsonRequired]
    [EmailAddress]
    public required string Email { get; init; }

    /// <summary>
    ///     Gets a value indicating whether the user account is disabled.
    /// </summary>
    [JsonRequired]
    public required bool IsDisabled { get; init; }

    /// <summary>
    ///     Gets the identifier of the role assigned to the user.
    /// </summary>
    [JsonRequired]
    public short? RoleId { get; init; }

    /// <summary>
    ///     Gets the display name of the user's role.
    /// </summary>
    [JsonRequired]
    public string? RoleName { get; init; }

    /// <summary>
    ///     Gets the first name of the user.
    /// </summary>
    [JsonRequired]
    public string? FirstName { get; init; }

    /// <summary>
    ///     Gets the last name of the user.
    /// </summary>
    [JsonRequired]
    public string? LastName { get; init; }
}

/// <summary>
///     Represents the response resource returned after creating a new user.
/// </summary>
[SuppressMessage("Design",
    "CA1515:Consider making public types internal",
    Justification =
        "Contracts must remain public to allow proper discovery and schema generation by OpenAPI.")]
public sealed record UserCreatedResource : IPublicApiContract,
    IEqualityOperators<UserCreatedResource, UserCreatedResource, bool>
{
    /// <summary>
    ///     Gets the unique identifier of the newly created user.
    /// </summary>
    [JsonRequired]
    public required Guid Uuid { get; init; }

    /// <summary>
    ///     Gets the username of the created user.
    /// </summary>
    [JsonRequired]
    public required string Username { get; init; }

    /// <summary>
    ///     Gets the email address of the created user.
    /// </summary>
    [JsonRequired]
    public required string Email { get; init; }

    /// <summary>
    ///     Gets a value indicating whether the created user account is disabled.
    /// </summary>
    [JsonRequired]
    public required bool IsDisabled { get; init; }

    /// <summary>
    ///     Gets the role identifier of the created user.
    /// </summary>
    [JsonRequired]
    public short? RoleId { get; init; }

    /// <summary>
    ///     Gets the first name of the created user.
    /// </summary>
    [JsonRequired]
    public string? FirstName { get; init; }

    /// <summary>
    ///     Gets the last name of the created user.
    /// </summary>
    [JsonRequired]
    public string? LastName { get; init; }
}

/// <summary>
///     Represents the response resource returned after a user password reset.
/// </summary>
[SuppressMessage("Design",
    "CA1515:Consider making public types internal",
    Justification =
        "Contracts must remain public to allow proper discovery and schema generation by OpenAPI.")]
public sealed record UserResetedPasswordResource : IPublicApiContract,
    IEqualityOperators<UserResetedPasswordResource, UserResetedPasswordResource, bool>
{
    /// <summary>
    ///     Gets the unique identifier of the user whose password was reset.
    /// </summary>
    [JsonRequired]
    public required Guid Uuid { get; init; }

    /// <summary>
    ///     Gets the new password value after the reset.
    /// </summary>
    [JsonRequired]
    public required string Password { get; init; }

    /// <summary>
    ///     Gets the email address of the user whose password was reset.
    /// </summary>
    [JsonRequired]
    public required string Email { get; init; }
}

/// <summary>
///     Represents the response resource indicating the current state of a user account.
/// </summary>
[SuppressMessage("Design",
    "CA1515:Consider making public types internal",
    Justification =
        "Contracts must remain public to allow proper discovery and schema generation by OpenAPI.")]
public sealed record UserStateResource : IPublicApiContract,
    IEqualityOperators<UserStateResource, UserStateResource, bool>
{
    /// <summary>
    ///     Gets the unique identifier of the user.
    /// </summary>
    [JsonRequired]
    public required Guid Uuid { get; init; }

    /// <summary>
    ///     Gets the username of the user.
    /// </summary>
    [JsonRequired]
    public required string Username { get; init; }

    /// <summary>
    ///     Gets a value indicating whether the user account is disabled.
    /// </summary>
    [JsonRequired]
    public required bool IsDisabled { get; init; }
}