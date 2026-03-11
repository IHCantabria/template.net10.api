using System.ComponentModel.DataAnnotations;
using System.Numerics;
using Microsoft.Extensions.Options;
using template.net10.api.Settings.Interfaces;

namespace template.net10.api.Settings.Options;

/// <summary>
///     Strongly-typed configuration options for JWT Bearer authentication.
///     Bound from the <c>Security:Jwt</c> configuration section.
/// </summary>
internal sealed record JwtOptions : ISecurityOptions, IEqualityOperators<JwtOptions, JwtOptions, bool>
{
    /// <summary>
    ///     The configuration section key used to bind <see cref="JwtOptions"/>.
    /// </summary>
    public const string Jwt = $"{ISecurityOptions.Security}:{nameof(Jwt)}";

    /// <summary>
    ///     The expected JWT audience claim value, used during token validation.
    /// </summary>
    [Required]
    public required string Audience { get; init; }

    /// <summary>
    ///     The expected JWT issuer claim value, used during token validation.
    /// </summary>
    [Required]
    public required string Issuer { get; init; }

    /// <summary>
    ///     The HMAC-SHA signing secret used to generate and validate JWT tokens.
    ///     Must be kept confidential and loaded from a secure configuration source.
    /// </summary>
    [Required]
    public required string Secret { get; init; }

    /// <summary>
    ///     The lifetime of issued JWT tokens, or <see langword="null"/> to use the provider default.
    /// </summary>
    public required TimeSpan? TokenLifetime { get; init; }
}

/// <summary>
///     Source-generated <see cref="IValidateOptions{TOptions}"/> validator for <see cref="JwtOptions"/>.
/// </summary>
[OptionsValidator]
internal sealed partial class JwtOptionsValidator : IValidateOptions<JwtOptions>;