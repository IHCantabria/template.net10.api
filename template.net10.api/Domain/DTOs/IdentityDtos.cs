using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using template.net10.api.Core.Authorization;
using template.net10.api.Core.Interfaces;

namespace template.net10.api.Domain.DTOs;

/// <summary>
///     Represents the authenticated user's identity information extracted from the request context.
/// </summary>
[SuppressMessage(
    "Design",
    "CA1515:Consider making public types internal",
    Justification = "Public visibility is required as this type is part of the application request contract.")]
public sealed record IdentityDto : IDto, IEqualityOperators<IdentityDto, IdentityDto, bool>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="IdentityDto"/> record with all identity values set to <see langword="null"/>.
    /// </summary>
    [SetsRequiredMembers]
    internal IdentityDto()
    {
        UserUuid = null;
        UserRoleName = null;
        UserIdentifier = null;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="IdentityDto"/> record with the specified identity values.
    /// </summary>
    /// <param name="userUuid">The unique identifier of the authenticated user.</param>
    /// <param name="userRoleName">The role name assigned to the user.</param>
    /// <param name="userIdentifier">The user's subject identifier from the token.</param>
    [SetsRequiredMembers]
    internal IdentityDto(Guid? userUuid, string? userRoleName, string? userIdentifier)
    {
        UserUuid = userUuid;
        UserRoleName = userRoleName;
        UserIdentifier = userIdentifier;
    }

    /// <summary>
    ///     Gets the unique identifier (UUID) of the authenticated user.
    /// </summary>
    public required Guid? UserUuid { get; init; }

    /// <summary>
    ///     Gets the subject identifier of the authenticated user from the token.
    /// </summary>
    public required string? UserIdentifier { get; init; }

    /// <summary>
    ///     Gets the role name assigned to the authenticated user.
    /// </summary>
    public required string? UserRoleName { get; init; }

    /// <summary>
    ///     Determines whether the identity has a non-null user identifier.
    /// </summary>
    /// <returns><see langword="true"/> if <see cref="UserIdentifier"/> is not <see langword="null"/>; otherwise, <see langword="false"/>.</returns>
    private bool HasUserIdentifier()
    {
        return UserIdentifier is not null;
    }

    /// <summary>
    ///     Determines whether the current identity corresponds to the Genie (system) user.
    /// </summary>
    /// <returns><see langword="true"/> if the user identifier matches the Genie identifier constant; otherwise, <see langword="false"/>.</returns>
    internal bool IsGenie()
    {
        return HasUserIdentifier() && UserIdentifier == GenieIdentityConstants.Identifier;
    }

    /// <summary>
    ///     Determines whether the identity has an assigned role.
    /// </summary>
    /// <returns><see langword="true"/> if <see cref="UserRoleName"/> is not <see langword="null"/>; otherwise, <see langword="false"/>.</returns>
    internal bool HasRole()
    {
        return UserRoleName is not null;
    }
}