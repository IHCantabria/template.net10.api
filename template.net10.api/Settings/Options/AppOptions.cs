using System.ComponentModel.DataAnnotations;
using System.Numerics;
using Microsoft.Extensions.Options;

namespace template.net10.api.Settings.Options;

/// <summary>
///     Strongly-typed configuration options that describe the running application environment.
/// </summary>
internal sealed record AppOptions : IEqualityOperators<AppOptions, AppOptions, bool>
{
    /// <summary>
    ///     The name of the current deployment environment (e.g. <c>local</c>, <c>dev</c>, <c>prod</c>).
    ///     See <see cref="Envs"/> for valid values.
    /// </summary>
    [Required]
    public required string Env { get; set; }
}

/// <summary>
///     Source-generated <see cref="IValidateOptions{TOptions}"/> validator for <see cref="AppOptions"/>.
/// </summary>
[OptionsValidator]
internal sealed partial class AppOptionsValidator : IValidateOptions<AppOptions>;