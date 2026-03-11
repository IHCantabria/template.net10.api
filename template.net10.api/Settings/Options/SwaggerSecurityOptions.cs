using System.ComponentModel.DataAnnotations;
using System.Numerics;
using Microsoft.Extensions.Options;

namespace template.net10.api.Settings.Options;

/// <summary>
///     Strongly-typed configuration options for the Swagger JWT Bearer security scheme definition.
///     Bound from the <c>SwaggerSecurity</c> configuration section.
/// </summary>
internal sealed record SwaggerSecurityOptions : IEqualityOperators<SwaggerSecurityOptions, SwaggerSecurityOptions, bool>
{
    /// <summary>
    ///     The configuration section key used to bind <see cref="SwaggerSecurityOptions"/>.
    /// </summary>
    public const string SwaggerSecurity = nameof(SwaggerSecurity);

    /// <summary>
    ///     Human-readable description of the security scheme shown in the Swagger UI.
    /// </summary>
    [Required]
    public required string Description { get; init; }

    /// <summary>
    ///     The name of the HTTP header or query parameter that carries the token (e.g. <c>Authorization</c>).
    /// </summary>
    [Required]
    public required string Name { get; init; }

    /// <summary>
    ///     The unique identifier used to reference this security scheme in OpenAPI operation definitions.
    /// </summary>
    [Required]
    public required string SchemeId { get; init; }

    /// <summary>
    ///     The HTTP authentication scheme name as per RFC 7235 (e.g. <c>Bearer</c>).
    /// </summary>
    [Required]
    public required string SchemeName { get; init; }

    /// <summary>
    ///     The hint for the bearer token format shown in the Swagger UI (e.g. <c>JWT</c>).
    /// </summary>
    [Required]
    public required string BearerFormat { get; init; }
}

/// <summary>
///     Source-generated <see cref="IValidateOptions{TOptions}"/> validator for <see cref="SwaggerSecurityOptions"/>.
/// </summary>
[OptionsValidator]
internal sealed partial class SwaggerSecurityOptionsValidator : IValidateOptions<SwaggerSecurityOptions>;