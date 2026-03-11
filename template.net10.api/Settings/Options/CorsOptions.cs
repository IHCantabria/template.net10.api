using System.ComponentModel.DataAnnotations;
using System.Numerics;
using Microsoft.Extensions.Options;

namespace template.net10.api.Settings.Options;

/// <summary>
///     Strongly-typed configuration options for CORS policy, specifying the policy name and allowed origins.
///     Bound from the <c>Cors</c> configuration section.
/// </summary>
internal sealed record CorsOptions : IEqualityOperators<CorsOptions, CorsOptions, bool>
{
    /// <summary>
    ///     The configuration section key used to bind <see cref="CorsOptions"/>.
    /// </summary>
    public const string Cors = nameof(Cors);

    /// <summary>
    ///     The name of the CORS policy registered in the DI container.
    /// </summary>
    [Required]
    public required string CorsPolicy { get; init; }

    /// <summary>
    ///     A semicolon-separated list of allowed origins, or <see langword="null"/> to allow none.
    /// </summary>
    public required string? AllowedOrigins { get; init; }

    /// <summary>
    ///     The parsed collection of allowed origins from <see cref="AllowedOrigins"/>.
    ///     Returns an empty enumerable when <see cref="AllowedOrigins"/> is null or empty.
    /// </summary>
    public IEnumerable<string> ArrayAllowedOrigins =>
        string.IsNullOrEmpty(AllowedOrigins) ? [] : AllowedOrigins.Split(';');
}

/// <summary>
///     Source-generated <see cref="IValidateOptions{TOptions}"/> validator for <see cref="CorsOptions"/>.
/// </summary>
[OptionsValidator]
internal sealed partial class CorsOptionsValidator : IValidateOptions<CorsOptions>;