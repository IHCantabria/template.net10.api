using System.Numerics;
using JetBrains.Annotations;
using template.net10.api.Core.Interfaces;

namespace template.net10.api.Core.Authorization.DTOs.Base;

/// <summary>
///     Base DTO representing user information extracted from an ID token.
/// </summary>
internal abstract record UserIdTokenBaseDto : IDto, IEqualityOperators<UserIdTokenBaseDto, UserIdTokenBaseDto, bool>
{
    /// <summary>
    ///     Gets the unique identifier of the user.
    /// </summary>
    internal required Guid Uuid { get; init; }

    /// <summary>
    ///     Gets the username of the user.
    /// </summary>
    internal required string Username { get; init; }

    /// <summary>
    ///     Gets the first name of the user.
    /// </summary>
    internal required string? FirstName { get; init; }

    /// <summary>
    ///     Gets the last name of the user.
    /// </summary>
    internal required string? LastName { get; init; }

    /// <summary>
    ///     Gets the email address of the user.
    /// </summary>
    internal required string Email { get; init; }

    /// <summary>
    ///     Gets the role name assigned to the user.
    /// </summary>
    internal required string? RoleName { get; init; }
}

/// <summary>
///     Base DTO representing user information extracted from an access token.
/// </summary>
internal abstract record UserAccessTokenBaseDto : IDto,
    IEqualityOperators<UserAccessTokenBaseDto, UserAccessTokenBaseDto, bool>
{
    /// <summary>
    ///     Gets the unique identifier of the user.
    /// </summary>
    internal required Guid Uuid { get; init; }

    /// <summary>
    ///     Gets the username of the user.
    /// </summary>
    internal required string Username { get; init; }

    /// <summary>
    ///     Gets the first name of the user.
    /// </summary>
    internal required string? FirstName { get; init; }

    /// <summary>
    ///     Gets the last name of the user.
    /// </summary>
    internal required string? LastName { get; init; }

    /// <summary>
    ///     Gets the email address of the user.
    /// </summary>
    internal required string Email { get; init; }

    /// <summary>
    ///     Gets the role name assigned to the user.
    /// </summary>
    internal required string? RoleName { get; init; }

    /// <summary>
    ///     Gets the collection of claims associated with the user's role.
    /// </summary>
    internal required IEnumerable<ClaimBaseDto> RoleClaims { get; init; } = [];

    /// <summary>
    ///     Gets the collection of claims directly assigned to the user.
    /// </summary>
    internal required IEnumerable<ClaimBaseDto> UserClaims { get; init; } = [];
}

/// <summary>
///     Base DTO representing a claim with an identifier and a name.
/// </summary>
internal abstract record ClaimBaseDto([UsedImplicitly] short Id, [UsedImplicitly] string Name)
    : IDto, IEqualityOperators<ClaimBaseDto, ClaimBaseDto, bool>;