using System.ComponentModel.DataAnnotations;
using System.Numerics;
using Microsoft.Extensions.Options;

namespace template.net10.api.Settings.Options;

/// <summary>
///     Strongly-typed configuration options for the API definition, including its public name and base address.
///     Bound from the <c>Api</c> configuration section.
/// </summary>
internal sealed record ApiOptions : IEqualityOperators<ApiOptions, ApiOptions, bool>
{
    /// <summary>
    ///     The configuration section key used to bind <see cref="ApiOptions"/>.
    /// </summary>
    public const string Api = nameof(Api);

    /// <summary>
    ///     The human-readable name of the API, used in OpenAPI documentation and logging.
    /// </summary>
    [Required]
    public required string Name { get; init; }

    /// <summary>
    ///     The public base address (URL) where the API is hosted.
    /// </summary>
    [Required]
    public required string Address { get; init; }
}

/// <summary>
///     Source-generated <see cref="IValidateOptions{TOptions}"/> validator for <see cref="ApiOptions"/>.
/// </summary>
[OptionsValidator]
internal sealed partial class ApiOptionsValidator : IValidateOptions<ApiOptions>;