using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Text.Json.Serialization;
using template.net10.api.Core.Interfaces;
using template.net10.api.Core.Json;

namespace template.net10.api.Core.Contracts;

/// <summary>
///     Represents a SignalR hub event resource describing an available real-time event endpoint.
/// </summary>
[SuppressMessage("Design",
    "CA1515:Consider making public types internal",
    Justification =
        "Contracts must remain public to allow proper discovery and schema generation by OpenAPI.")]
public sealed record HubEventResource : IPublicApiContract, IEqualityOperators<HubEventResource, HubEventResource, bool>
{
    /// <summary>
    ///     Gets the name of the hub event.
    /// </summary>
    [JsonRequired]
    [JsonConverter(typeof(CamelCaseStringConverter))]
    public required string Name { get; init; }

    /// <summary>
    ///     Gets the description of the hub event.
    /// </summary>
    [JsonRequired]
    public required string Description { get; init; }

    /// <summary>
    ///     Gets the endpoint path associated with this hub event.
    /// </summary>
    [JsonRequired]
    public required string Path { get; init; }

    /// <summary>
    ///     Gets the type of the hub event (e.g., notification, broadcast).
    /// </summary>
    [JsonRequired]
    public required string Type { get; init; }

    /// <summary>
    ///     Gets the collection of field names included in the hub event payload.
    /// </summary>
    [JsonRequired]
    [JsonConverter(typeof(CamelCaseStringEnumerableConverter))]
    public required IEnumerable<string> Fields { get; init; }
}

/// <summary>
///     Represents an informational message resource sent through a SignalR hub connection.
/// </summary>
[SuppressMessage("Design",
    "CA1515:Consider making public types internal",
    Justification =
        "Contracts must remain public to allow proper discovery and schema generation by OpenAPI.")]
public sealed record HubInfoMessageResource : IPublicApiContract,
    IEqualityOperators<HubInfoMessageResource, HubInfoMessageResource, bool>
{
    /// <summary>
    ///     Gets the informational message content.
    /// </summary>
    public required string Message { get; init; } = string.Empty;

    /// <summary>
    ///     Gets the SignalR connection identifier associated with this message.
    /// </summary>
    public required string ConnectionId { get; init; } = string.Empty;
}