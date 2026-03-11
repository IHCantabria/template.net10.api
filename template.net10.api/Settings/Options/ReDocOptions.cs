using System.ComponentModel.DataAnnotations;
using System.Numerics;
using Microsoft.Extensions.Options;

namespace template.net10.api.Settings.Options;

/// <summary>
///     Strongly-typed configuration options for ReDoc API documentation UI.
///     Bound from the <c>ReDoc</c> configuration section.
/// </summary>
internal sealed record ReDocOptions : IEqualityOperators<ReDocOptions, ReDocOptions, bool>
{
    /// <summary>
    ///     The configuration section key used to bind <see cref="ReDocOptions"/>.
    /// </summary>
    public const string ReDoc = nameof(ReDoc);

    /// <summary>
    ///     The HTML page title displayed in the browser tab when the ReDoc UI is open.
    /// </summary>
    [Required]
    public required string DocumentTitle { get; init; }

    /// <summary>
    ///     The URL of the OpenAPI JSON specification consumed by the ReDoc UI.
    /// </summary>
    [Required]
    public required Uri SpecUrl { get; init; }

    /// <summary>
    ///     The URL path prefix under which the ReDoc UI is served (e.g. <c>redoc</c>).
    /// </summary>
    [Required]
    public required string RoutePrefix { get; init; }
}

/// <summary>
///     Source-generated <see cref="IValidateOptions{TOptions}"/> validator for <see cref="ReDocOptions"/>.
/// </summary>
[OptionsValidator]
internal sealed partial class ReDocOptionsValidator : IValidateOptions<ReDocOptions>;