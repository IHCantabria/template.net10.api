using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Microsoft.Extensions.Options;

namespace template.net10.api.Settings.Options;

/// <summary>
///     Strongly-typed configuration options describing the project version, used in API documentation and telemetry.
/// </summary>
[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)]
internal sealed record ProjectOptions : IEqualityOperators<ProjectOptions, ProjectOptions, bool>
{
    /// <summary>
    ///     The current version of the project (e.g. <c>1.0.0</c>), surfaced in OpenAPI metadata.
    /// </summary>
    [Required]
    [MinLength(1)]
    public required string Version { get; init; }
}

/// <summary>
///     Source-generated <see cref="IValidateOptions{TOptions}" /> validator for <see cref="ProjectOptions" />.
/// </summary>
[OptionsValidator]
internal sealed partial class ProjectOptionsValidator : IValidateOptions<ProjectOptions>;