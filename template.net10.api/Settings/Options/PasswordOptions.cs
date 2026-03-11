using System.ComponentModel.DataAnnotations;
using System.Numerics;
using Microsoft.Extensions.Options;
using template.net10.api.Settings.Interfaces;

namespace template.net10.api.Settings.Options;

/// <summary>
///     Strongly-typed configuration options for password hashing, containing the per-application pepper secret.
///     Bound from the <c>Security:Password</c> configuration section.
/// </summary>
internal sealed record PasswordOptions : IEqualityOperators<PasswordOptions, PasswordOptions, bool>
{
    /// <summary>
    ///     The configuration section key used to bind <see cref="PasswordOptions"/>.
    /// </summary>
    public const string Password = $"{ISecurityOptions.Security}:{nameof(Password)}";

    /// <summary>
    ///     The server-side pepper appended to passwords before hashing, providing an additional layer
    ///     of protection against credential database leaks. Must be kept secret.
    /// </summary>
    [Required]
    public required string Pepper { get; init; }
}

/// <summary>
///     Source-generated <see cref="IValidateOptions{TOptions}"/> validator for <see cref="PasswordOptions"/>.
/// </summary>
[OptionsValidator]
internal sealed partial class PasswordOptionsValidator : IValidateOptions<PasswordOptions>;