using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using template.net10.api.Core.Interfaces;
using template.net10.api.Domain.Interfaces;

namespace template.net10.api.Domain.DTOs;

/// <summary>
///     Parameters for querying a single user by unique identifier.
/// </summary>
[SuppressMessage(
    "Design",
    "CA1515:Consider making public types internal",
    Justification = "Public visibility is required as this type is part of the application request contract.")]
public sealed record QueryGetUserParamsDto : IDto,
    IEqualityOperators<QueryGetUserParamsDto, QueryGetUserParamsDto, bool>
{
    /// <summary>
    ///     Gets the unique identifier (UUID) of the user to retrieve.
    /// </summary>
    public required Guid Key { get; init; }
}

/// <summary>
///     Parameters for authenticating a user via email and password.
/// </summary>
[SuppressMessage(
    "Design",
    "CA1515:Consider making public types internal",
    Justification = "Public visibility is required as this type is part of the application request contract.")]
public sealed record QueryLoginUserParamsDto : IDto,
    IEqualityOperators<QueryLoginUserParamsDto, QueryLoginUserParamsDto, bool>
{
    /// <summary>
    ///     Gets the email address used for authentication.
    /// </summary>
    public required string Email { get; init; }

    /// <summary>
    ///     Gets the plain-text password used for authentication.
    /// </summary>
    public required string Password { get; init; }
}

/// <summary>
///     Parameters for retrieving the currently authenticated user's access information.
/// </summary>
[SuppressMessage(
    "Design",
    "CA1515:Consider making public types internal",
    Justification = "Public visibility is required as this type is part of the application request contract.")]
public sealed record QueryAccessUserParamsDto : IDto, IIdentity,
    IEqualityOperators<QueryAccessUserParamsDto, QueryAccessUserParamsDto, bool>
{
    /// <inheritdoc cref="IIdentity.Identity" />
    public required IdentityDto Identity { get; set; }
}