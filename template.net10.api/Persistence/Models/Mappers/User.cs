using System.Diagnostics.CodeAnalysis;
using template.net10.api.Contracts;
using template.net10.api.Core.Interfaces;
using template.net10.api.Domain.DTOs;

namespace template.net10.api.Persistence.Models;

internal partial class User
{
    /// <summary>
    ///     Implicitly converts a <see cref="User" /> entity to a <see cref="UserCreatedResource" /> response contract,
    ///     mapping all relevant fields.
    /// </summary>
    /// <param name="entity">The source <see cref="User" /> entity.</param>
    /// <returns>A <see cref="UserCreatedResource" /> populated from the entity.</returns>
    public static implicit operator UserCreatedResource(User entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        return new UserCreatedResource
        {
            Username = entity.Username,
            Email = entity.Email,
            IsDisabled = entity.IsDisabled,
            RoleId = entity.RoleId,
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            Uuid = entity.Uuid
        };
    }

    /// <summary>
    ///     Explicitly maps a <see cref="User" /> entity to a <see cref="UserCreatedResource" /> by invoking the implicit
    ///     operator.
    /// </summary>
    /// <param name="entity">The source <see cref="User" /> entity.</param>
    /// <returns>A <see cref="UserCreatedResource" /> populated from the entity.</returns>
    public static UserCreatedResource ToUserCreatedResource(
        User entity)
    {
        return entity;
    }

    /// <summary>
    ///     Implicitly converts a <see cref="User" /> entity to a <see cref="UserResetedPasswordDto" />.
    ///     The <c>Password</c> field is intentionally left empty as it is only available before hashing.
    /// </summary>
    /// <param name="entity">The source <see cref="User" /> entity.</param>
    /// <returns>A <see cref="UserResetedPasswordDto" /> with UUID and email populated.</returns>
    public static implicit operator UserResetedPasswordDto(User entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        return new UserResetedPasswordDto
        {
            Uuid = entity.Uuid,
            Email = entity.Email,
            Password = ""
        };
    }

    /// <summary>
    ///     Explicitly maps a <see cref="User" /> entity to a <see cref="UserResetedPasswordDto" /> by invoking the implicit
    ///     operator.
    /// </summary>
    /// <param name="entity">The source <see cref="User" /> entity.</param>
    /// <returns>A <see cref="UserResetedPasswordDto" /> populated from the entity.</returns>
    public static UserResetedPasswordDto ToUserResetedPasswordDto(
        User entity)
    {
        return entity;
    }

    /// <summary>
    ///     Implicitly converts a <see cref="User" /> entity to a <see cref="UserStateResource" />,
    ///     mapping username, disabled flag, and UUID.
    /// </summary>
    /// <param name="entity">The source <see cref="User" /> entity.</param>
    /// <returns>A <see cref="UserStateResource" /> populated from the entity.</returns>
    public static implicit operator UserStateResource(User entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        return new UserStateResource
        {
            Username = entity.Username,
            IsDisabled = entity.IsDisabled,
            Uuid = entity.Uuid
        };
    }

    /// <summary>
    ///     Explicitly maps a <see cref="User" /> entity to a <see cref="UserStateResource" /> by invoking the implicit
    ///     operator.
    /// </summary>
    /// <param name="entity">The source <see cref="User" /> entity.</param>
    /// <returns>A <see cref="UserStateResource" /> populated from the entity.</returns>
    public static UserStateResource ToUserStateResource(
        User entity)
    {
        return entity;
    }
}

/// <summary>
///     Contains EF Core query projections from <see cref="User" /> to various DTO types,
///     used by the generic repository to produce efficient server-side projections.
/// </summary>
internal static class UserProjections
{
    /// <summary>
    ///     Projection from <see cref="User" /> to <see cref="UserIdTokenDto" />,
    ///     used when building identity tokens (includes UUID, email, names, role, and username).
    /// </summary>
    internal static IProjection<User, UserIdTokenDto> UserIdTokenProjection =>
        new Projection<User, UserIdTokenDto>(static p => new UserIdTokenDto
        {
            Uuid = p.Uuid,
            Email = p.Email,
            FirstName = p.FirstName,
            LastName = p.LastName,
            RoleName = p.Role != null ? p.Role.Name : null,
            Username = p.Username
        });

    /// <summary>
    ///     Projection from <see cref="User" /> to <see cref="UserAccessTokenDto" />,
    ///     used when building JWT access tokens (includes UUID, names, role, username, role claims, and user claims).
    /// </summary>
    [SuppressMessage(
        "ReSharper",
        "ExceptionNotDocumentedOptional",
        Justification =
            "Potential exceptions originate from underlying implementation details and are not part of the method contract.")]
    internal static IProjection<User, UserAccessTokenDto> UserAccessTokenProjection =>
        new Projection<User, UserAccessTokenDto>(static p => new UserAccessTokenDto
        {
            Uuid = p.Uuid,
            Email = p.Email,
            FirstName = p.FirstName,
            LastName = p.LastName,
            RoleName = p.Role != null ? p.Role.Name : null,
            Username = p.Username,
            RoleClaims = p.Role != null
                ? p.Role.Claims.Select(static c => new ClaimDto(c.Id, c.Name))
                : Enumerable.Empty<ClaimDto>(),
            UserClaims = p.Claims.Select(static c => new ClaimDto(c.Id, c.Name))
        });

    /// <summary>
    ///     Projection from <see cref="User" /> to <see cref="UserCredentialsDto" />,
    ///     used during login to retrieve the stored password hash and salt for verification.
    /// </summary>
    internal static IProjection<User, UserCredentialsDto> UserCredentialsProjection =>
        new Projection<User, UserCredentialsDto>(static p => new UserCredentialsDto
        {
            PasswordHash = p.PasswordHash,
            PasswordSalt = p.PasswordSalt
        });

    /// <summary>
    ///     Projection from <see cref="User" /> to <see cref="UserDto" />,
    ///     used to retrieve full user information for API responses.
    /// </summary>
    internal static IProjection<User, UserDto> UserProjection =>
        new Projection<User, UserDto>(static p => new UserDto
        {
            Uuid = p.Uuid,
            Email = p.Email,
            FirstName = p.FirstName,
            IsDisabled = p.IsDisabled,
            LastName = p.LastName,
            RoleId = p.RoleId,
            RoleName = p.Role != null ? p.Role.Name : null,
            Username = p.Username
        });
}