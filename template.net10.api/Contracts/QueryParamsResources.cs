using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using template.net10.api.Core.Interfaces;

namespace template.net10.api.Contracts;

/// <summary>
///     Represents the API contract for retrieving a user by their unique identifier.
/// </summary>
[SuppressMessage("Design",
    "CA1515:Consider making public types internal",
    Justification =
        "Contracts must remain public to allow proper discovery and schema generation by OpenAPI.")]
public sealed partial record QueryGetUserParamsResource : IPublicApiContract,
    IEqualityOperators<QueryGetUserParamsResource, QueryGetUserParamsResource, bool>
{
    /// <summary>
    ///     Gets the unique identifier of the user to retrieve.
    /// </summary>
    [Required]
    [FromRoute(Name = "user-key")]
    public required Guid Key { get; init; }
}

/// <summary>
///     Represents the API contract for authenticating a user via login.
/// </summary>
[SuppressMessage("Design",
    "CA1515:Consider making public types internal",
    Justification =
        "Contracts must remain public to allow proper discovery and schema generation by OpenAPI.")]
public sealed partial record QueryLoginUserParamsResource : IPublicApiContract,
    IEqualityOperators<QueryLoginUserParamsResource, QueryLoginUserParamsResource, bool>
{
    /// <summary>
    ///     Gets the email address used for authentication.
    /// </summary>
    [JsonRequired]
    [EmailAddress]
    public required string Email { get; init; }

    /// <summary>
    ///     Gets the password used for authentication.
    /// </summary>
    [JsonRequired]
    public required string Password { get; init; }
}