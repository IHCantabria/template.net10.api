using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using template.net10.api.Core.Interfaces;
using template.net10.api.Domain.Interfaces;

namespace template.net10.api.Domain.DTOs;

/// <summary>
///     Parameters for the create-user command.
/// </summary>
[SuppressMessage(
    "Design",
    "CA1515:Consider making public types internal",
    Justification = "Public visibility is required as this type is part of the application request contract.")]
public sealed record CommandCreateUserParamsDto : IDto, IIdentity,
    IEqualityOperators<CommandCreateUserParamsDto, CommandCreateUserParamsDto, bool>
{
    /// <summary>
    ///     Gets the username for the new user.
    /// </summary>
    public required string Username { get; init; }

    /// <summary>
    ///     Gets the email address for the new user.
    /// </summary>
    public required string Email { get; init; }

    /// <summary>
    ///     Gets a value indicating whether the new user account is disabled.
    /// </summary>
    public required bool IsDisabled { get; init; }

    /// <summary>
    ///     Gets the password for the new user.
    /// </summary>
    public required string Password { get; init; }

    /// <summary>
    ///     Gets the password confirmation that must match <see cref="Password"/>.
    /// </summary>
    public required string ConfirmPassword { get; init; }

    /// <summary>
    ///     Gets the optional role identifier to assign to the new user.
    /// </summary>
    public required short? RoleId { get; init; }

    /// <summary>
    ///     Gets the optional first name of the new user.
    /// </summary>
    public required string? FirstName { get; init; }

    /// <summary>
    ///     Gets the optional last name of the new user.
    /// </summary>
    public required string? LastName { get; init; }

    /// <inheritdoc cref="IIdentity.Identity" />
    public required IdentityDto Identity { get; set; }
}

/// <summary>
///     Parameters for the reset-user-password command.
/// </summary>
[SuppressMessage(
    "Design",
    "CA1515:Consider making public types internal",
    Justification = "Public visibility is required as this type is part of the application request contract.")]
public sealed record CommandResetUserPasswordParamsDto : IDto, IIdentity,
    IEqualityOperators<CommandResetUserPasswordParamsDto, CommandResetUserPasswordParamsDto, bool>
{
    /// <summary>
    ///     Gets the unique identifier (UUID) of the user whose password is being reset.
    /// </summary>
    public required Guid Key { get; init; }

    /// <summary>
    ///     Gets the new password for the user.
    /// </summary>
    public required string Password { get; init; }

    /// <summary>
    ///     Gets the password confirmation that must match <see cref="Password"/>.
    /// </summary>
    public required string ConfirmPassword { get; init; }

    /// <inheritdoc cref="IIdentity.Identity" />
    public required IdentityDto Identity { get; set; }
}

/// <summary>
///     Parameters for the update-user command.
/// </summary>
[SuppressMessage(
    "Design",
    "CA1515:Consider making public types internal",
    Justification = "Public visibility is required as this type is part of the application request contract.")]
public sealed record CommandUpdateUserParamsDto : IDto, IIdentity,
    IEqualityOperators<CommandUpdateUserParamsDto, CommandUpdateUserParamsDto, bool>
{
    /// <summary>
    ///     Gets the unique identifier (UUID) of the user to update.
    /// </summary>
    public required Guid Key { get; init; }

    /// <summary>
    ///     Gets the optional new username for the user.
    /// </summary>
    public required string? Username { get; init; }

    /// <summary>
    ///     Gets the optional new disabled status for the user.
    /// </summary>
    public required bool? IsDisabled { get; init; }

    /// <summary>
    ///     Gets the optional new email address for the user.
    /// </summary>
    public required string? Email { get; init; }

    /// <summary>
    ///     Gets the optional new role identifier for the user.
    /// </summary>
    public required short? RoleId { get; init; }

    /// <summary>
    ///     Gets the optional new first name for the user.
    /// </summary>
    public required string? FirstName { get; init; }

    /// <summary>
    ///     Gets the optional new last name for the user.
    /// </summary>
    public required string? LastName { get; init; }

    /// <inheritdoc cref="IIdentity.Identity" />
    public required IdentityDto Identity { get; set; }
}

/// <summary>
///     Parameters for the disable-user command.
/// </summary>
[SuppressMessage(
    "Design",
    "CA1515:Consider making public types internal",
    Justification = "Public visibility is required as this type is part of the application request contract.")]
public sealed record CommandDisableUserParamsDto : IDto, IIdentity,
    IEqualityOperators<CommandDisableUserParamsDto, CommandDisableUserParamsDto, bool>
{
    /// <summary>
    ///     Gets the unique identifier (UUID) of the user to disable.
    /// </summary>
    public required Guid Key { get; init; }

    /// <inheritdoc cref="IIdentity.Identity" />
    public required IdentityDto Identity { get; set; }
}

/// <summary>
///     Parameters for the enable-user command.
/// </summary>
[SuppressMessage(
    "Design",
    "CA1515:Consider making public types internal",
    Justification = "Public visibility is required as this type is part of the application request contract.")]
public sealed record CommandEnableUserParamsDto : IDto, IIdentity,
    IEqualityOperators<CommandEnableUserParamsDto, CommandEnableUserParamsDto, bool>
{
    /// <summary>
    ///     Gets the unique identifier (UUID) of the user to enable.
    /// </summary>
    public required Guid Key { get; init; }

    /// <inheritdoc cref="IIdentity.Identity" />
    public required IdentityDto Identity { get; set; }
}

/// <summary>
///     Parameters for the delete-user command.
/// </summary>
[SuppressMessage(
    "Design",
    "CA1515:Consider making public types internal",
    Justification = "Public visibility is required as this type is part of the application request contract.")]
public sealed record CommandDeleteUserParamsDto : IDto, IIdentity,
    IEqualityOperators<CommandDeleteUserParamsDto, CommandDeleteUserParamsDto, bool>
{
    /// <summary>
    ///     Gets the unique identifier (UUID) of the user to delete.
    /// </summary>
    public required Guid Key { get; init; }

    /// <inheritdoc cref="IIdentity.Identity" />
    public required IdentityDto Identity { get; set; }
}