using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using template.net10.api.Core.Interfaces;

namespace template.net10.api.Hubs.User.Contracts;

/// <summary>
///     SignalR hub message resource for user creation event notifications.
/// </summary>
[SuppressMessage("Design",
    "CA1515:Consider making public types internal",
    Justification =
        "Contracts must remain public to allow proper discovery and schema generation by OpenAPI.")]
public sealed record UserHubCreatedUserMessageResource : IPublicApiContract,
    IEqualityOperators<UserHubCreatedUserMessageResource, UserHubCreatedUserMessageResource, bool>
{
    /// <summary>
    ///     The notification message describing the user creation event.
    /// </summary>
    public required string Message { get; init; } = string.Empty;

    /// <summary>
    ///     The unique identifier of the created user.
    /// </summary>
    public required string Uuid { get; init; } = string.Empty;
}

/// <summary>
///     SignalR hub message resource for user update event notifications.
/// </summary>
[SuppressMessage("Design",
    "CA1515:Consider making public types internal",
    Justification =
        "Contracts must remain public to allow proper discovery and schema generation by OpenAPI.")]
public sealed record UserHubUpdatedUserMessageResource : IPublicApiContract,
    IEqualityOperators<UserHubUpdatedUserMessageResource, UserHubUpdatedUserMessageResource, bool>
{
    /// <summary>
    ///     The notification message describing the user update event.
    /// </summary>
    public required string Message { get; init; } = string.Empty;

    /// <summary>
    ///     The unique identifier of the updated user.
    /// </summary>
    public required string Uuid { get; init; } = string.Empty;
}

/// <summary>
///     SignalR hub message resource for user deletion event notifications.
/// </summary>
[SuppressMessage("Design",
    "CA1515:Consider making public types internal",
    Justification =
        "Contracts must remain public to allow proper discovery and schema generation by OpenAPI.")]
public sealed record UserHubDeletedUserMessageResource : IPublicApiContract,
    IEqualityOperators<UserHubDeletedUserMessageResource, UserHubDeletedUserMessageResource, bool>
{
    /// <summary>
    ///     The notification message describing the user deletion event.
    /// </summary>
    public required string Message { get; init; } = string.Empty;

    /// <summary>
    ///     The unique identifier of the deleted user.
    /// </summary>
    public required string Uuid { get; init; } = string.Empty;
}