using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Text.Json.Serialization;
using template.net10.api.Core.Interfaces;

namespace template.net10.api.Core.Contracts;

/// <summary>
///     Represents an error code resource containing a key-description pair for API error responses.
/// </summary>
[SuppressMessage("Design",
    "CA1515:Consider making public types internal",
    Justification =
        "Contracts must remain public to allow proper discovery and schema generation by OpenAPI.")]
public sealed partial record ErrorCodeResource : IPublicApiContract,
    IEqualityOperators<ErrorCodeResource, ErrorCodeResource, bool>
{
    /// <summary>
    ///     Gets the unique key that identifies this error code.
    /// </summary>
    [JsonRequired]
    public required string Key { get; init; }

    /// <summary>
    ///     Gets the human-readable description of this error code.
    /// </summary>
    [JsonRequired]
    public required string Description { get; init; }
}