using System.Numerics;
using template.net10.api.Core.Authorization.DTOs.Base;
using template.net10.api.Core.Interfaces;

namespace template.net10.api.Domain.DTOs;

/// <summary>
///     Represents the access token data returned after successful authentication.
/// </summary>
internal sealed partial record AccessTokenDto : IDto,
    IEqualityOperators<AccessTokenDto, AccessTokenDto, bool>
{
    /// <summary>
    ///     Gets the signed JWT access token string.
    /// </summary>
    internal required string AccessToken { get; init; }

    /// <summary>
    ///     Gets the access token type identifier. Always returns "Bearer".
    /// </summary>
    private static string AccessTokenType => "Bearer";

    /// <summary>
    ///     Gets the signed JWT refresh token string.
    /// </summary>
    internal required string RefreshToken { get; init; }

    /// <summary>
    ///     Gets the refresh token type identifier. Always returns "Guid".
    /// </summary>
    private static string RefreshTokenType => "Guid";
}

/// <summary>
///     Represents the ID token data returned after successful authentication.
/// </summary>
internal sealed partial record IdTokenDto : IDto, IEqualityOperators<IdTokenDto, IdTokenDto, bool>
{
    /// <summary>
    ///     Gets the signed JWT ID token string.
    /// </summary>
    internal required string IdToken { get; init; }

    /// <summary>
    ///     Gets the ID token type identifier. Always returns "Bearer".
    /// </summary>
    private static string TokenType => "Bearer";
}

/// <summary>
///     Represents a user data transfer object exposed through the API.
/// </summary>
public sealed partial record UserDto : IDto, IEqualityOperators<UserDto, UserDto, bool>
{
    /// <summary>
    ///     Gets the unique identifier (UUID) of the user.
    /// </summary>
    public required Guid Uuid { get; init; }

    /// <summary>
    ///     Gets the username.
    /// </summary>
    public required string Username { get; init; }

    /// <summary>
    ///     Gets the user's email address.
    /// </summary>
    public required string Email { get; init; }

    /// <summary>
    ///     Gets a value indicating whether the user account is disabled.
    /// </summary>
    public required bool IsDisabled { get; init; }

    /// <summary>
    ///     Gets the optional role identifier assigned to the user.
    /// </summary>
    public required short? RoleId { get; init; }

    /// <summary>
    ///     Gets the optional role name assigned to the user.
    /// </summary>
    public required string? RoleName { get; init; }

    /// <summary>
    ///     Gets the optional first name of the user.
    /// </summary>
    public required string? FirstName { get; init; }

    /// <summary>
    ///     Gets the optional last name of the user.
    /// </summary>
    public required string? LastName { get; init; }
}

/// <summary>
///     Represents a user DTO containing the result of a password reset operation.
/// </summary>
public sealed partial record UserResetedPasswordDto : IDto,
    IEqualityOperators<UserResetedPasswordDto, UserResetedPasswordDto, bool>
{
    /// <summary>
    ///     Gets the unique identifier (UUID) of the user whose password was reset.
    /// </summary>
    public required Guid Uuid { get; init; }

    /// <summary>
    ///     Gets the email address of the user whose password was reset.
    /// </summary>
    public required string Email { get; init; }

    /// <summary>
    ///     Gets or sets the new password generated during the reset.
    /// </summary>
    public required string Password { get; set; } = "";
}

/// <summary>
///     Represents the stored password credentials (hash and salt) for a user.
/// </summary>
internal sealed record UserCredentialsDto : IDto, IEqualityOperators<UserCredentialsDto, UserCredentialsDto, bool>
{
    /// <summary>
    ///     Gets the stored password hash.
    /// </summary>
    internal required string PasswordHash { get; init; }

    /// <summary>
    ///     Gets the stored password salt.
    /// </summary>
    internal required string PasswordSalt { get; init; }
}

/// <summary>
///     Represents the data required to create a new user in the persistence layer.
/// </summary>
internal sealed partial record CreateUserDto : IDto,
    IEqualityOperators<CreateUserDto, CreateUserDto, bool>
{
    /// <summary>
    ///     Gets the generated unique identifier (UUID) for the new user.
    /// </summary>
    internal required Guid Uuid { get; init; }

    /// <summary>
    ///     Gets the username for the new user.
    /// </summary>
    internal required string Username { get; init; }

    /// <summary>
    ///     Gets the email address for the new user.
    /// </summary>
    internal required string Email { get; init; }

    /// <summary>
    ///     Gets a value indicating whether the new user account is disabled.
    /// </summary>
    internal required bool IsDisabled { get; init; }

    /// <summary>
    ///     Gets the hashed password for the new user.
    /// </summary>
    internal required string PasswordHash { get; init; }

    /// <summary>
    ///     Gets the password salt used during hashing.
    /// </summary>
    internal required string PasswordSalt { get; init; }

    /// <summary>
    ///     Gets the optional role identifier to assign to the new user.
    /// </summary>
    internal required short? RoleId { get; init; }

    /// <summary>
    ///     Gets the optional first name of the new user.
    /// </summary>
    internal string? FirstName { get; init; }

    /// <summary>
    ///     Gets the optional last name of the new user.
    /// </summary>
    internal string? LastName { get; init; }
}

/// <summary>
///     Represents the input data for hashing a new user password.
/// </summary>
internal sealed record CreateUserPasswordDto : IDto,
    IEqualityOperators<CreateUserPasswordDto, CreateUserPasswordDto, bool>
{
    /// <summary>
    ///     Gets the plain-text password to hash.
    /// </summary>
    internal required string Password { get; init; }

    /// <summary>
    ///     Gets the application-level secret pepper to append during hashing.
    /// </summary>
    internal required string Pepper { get; init; }
}

/// <summary>
///     Represents the input data for verifying a user's password against a stored hash.
/// </summary>
internal sealed record VerifyUserPasswordDto : IDto,
    IEqualityOperators<VerifyUserPasswordDto, VerifyUserPasswordDto, bool>
{
    /// <summary>
    ///     Gets the plain-text password to verify.
    /// </summary>
    internal required string Password { get; init; }

    /// <summary>
    ///     Gets the application-level secret pepper used during the original hashing.
    /// </summary>
    internal required string Pepper { get; init; }

    /// <summary>
    ///     Gets the stored salt that was used during the original hashing.
    /// </summary>
    internal required string Salt { get; init; }

    /// <summary>
    ///     Gets the stored password hash to compare against.
    /// </summary>
    internal required string Hash { get; init; }
}

/// <summary>
///     Represents user data used to generate an ID token, inheriting base ID token claims.
/// </summary>
internal sealed record UserIdTokenDto : UserIdTokenBaseDto,
    IEqualityOperators<UserIdTokenDto, UserIdTokenDto, bool>;

/// <summary>
///     Represents user data used to generate an access token, inheriting base access token claims.
/// </summary>
internal sealed record UserAccessTokenDto : UserAccessTokenBaseDto,
    IEqualityOperators<UserAccessTokenDto, UserAccessTokenDto, bool>;