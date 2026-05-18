using System.Diagnostics.CodeAnalysis;
using template.net10.api.Contracts;

namespace template.net10.api.Domain.DTOs;

internal sealed partial record AccessTokenDto
{
    /// <summary>
    ///     Implicitly converts an <see cref="AccessTokenDto" /> to an <see cref="AccessTokenResource" />.
    /// </summary>
    /// <param name="dto">The access token DTO to convert.</param>
    /// <returns>A new <see cref="AccessTokenResource" /> populated from the DTO.</returns>
    public static implicit operator AccessTokenResource(AccessTokenDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        return new AccessTokenResource
        {
            AccessToken = dto.AccessToken,
            AccessTokenType = AccessTokenType,
            RefreshToken = dto.RefreshToken,
            RefreshTokenType = RefreshTokenType
        };
    }
}

internal sealed partial record IdTokenDto
{
    /// <summary>
    ///     Implicitly converts an <see cref="IdTokenDto" /> to an <see cref="IdTokenResource" />.
    /// </summary>
    /// <param name="dto">The ID token DTO to convert.</param>
    /// <returns>A new <see cref="IdTokenResource" /> populated from the DTO.</returns>
    public static implicit operator IdTokenResource(IdTokenDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        return new IdTokenResource
        {
            IdToken = dto.IdToken,
            IdTokenType = TokenType
        };
    }
}

public sealed partial record UserDto
{
    /// <summary>
    ///     Implicitly converts a <see cref="UserDto" /> to a <see cref="UserResource" />.
    /// </summary>
    /// <param name="dto">The user DTO to convert.</param>
    /// <returns>A new <see cref="UserResource" /> populated from the DTO.</returns>
    public static implicit operator UserResource(UserDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        return new UserResource
        {
            Uuid = dto.Uuid,
            Username = dto.Username,
            Email = dto.Email,
            IsDisabled = dto.IsDisabled,
            RoleId = dto.RoleId,
            RoleName = dto.RoleName,
            FirstName = dto.FirstName,
            LastName = dto.LastName
        };
    }

    /// <summary>
    ///     Named alternative for the implicit conversion from <see cref="UserDto" /> to <see cref="UserResource" />.
    /// </summary>
    /// <param name="dto">The user DTO to convert.</param>
    /// <returns>A new <see cref="UserResource" /> populated from the DTO.</returns>
    [SuppressMessage(
        "ReSharper",
        "UnusedMember.Global",
        Justification = "Required as a named alternative for the implicit conversion operator.")]
    public static UserResource ToUserResource(UserDto dto)
    {
        return dto;
    }

    /// <summary>
    ///     Converts a read-only list of <see cref="UserDto" /> instances to an enumerable of <see cref="UserResource" />.
    /// </summary>
    /// <param name="dtos">The collection of user DTOs to convert.</param>
    /// <returns>An enumerable of <see cref="UserResource" /> instances.</returns>
    internal static IEnumerable<UserResource> ToCollection(
        IReadOnlyList<UserDto> dtos)
    {
        var resources = new UserResource[dtos.Count];
        for (var i = 0; i < dtos.Count; i++) resources[i] = dtos[i];
        return resources;
    }
}

public sealed partial record UserResetedPasswordDto
{
    /// <summary>
    ///     Implicitly converts a <see cref="UserResetedPasswordDto" /> to a <see cref="UserResetedPasswordResource" />.
    /// </summary>
    /// <param name="dto">The reset-password DTO to convert.</param>
    /// <returns>A new <see cref="UserResetedPasswordResource" /> populated from the DTO.</returns>
    public static implicit operator UserResetedPasswordResource(UserResetedPasswordDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        return new UserResetedPasswordResource
        {
            Uuid = dto.Uuid,
            Password = dto.Password,
            Email = dto.Email
        };
    }

    /// <summary>
    ///     Named alternative for the implicit conversion from <see cref="UserResetedPasswordDto" /> to
    ///     <see cref="UserResetedPasswordResource" />.
    /// </summary>
    /// <param name="dto">The reset-password DTO to convert.</param>
    /// <returns>A new <see cref="UserResetedPasswordResource" /> populated from the DTO.</returns>
    [SuppressMessage(
        "ReSharper",
        "UnusedMember.Global",
        Justification = "Required as a named alternative for the implicit conversion operator.")]
    public static UserResetedPasswordResource ToUserResetedPasswordResource(UserResetedPasswordDto dto)
    {
        return dto;
    }
}