using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Microsoft.Extensions.Options;

namespace template.net10.api.Settings.Options;

/// <summary>
///     Strongly-typed configuration options for Swagger/OpenAPI documentation generation and UI.
///     Declared <see langword="public"/> to allow proper discovery and schema generation by the OpenAPI toolchain.
///     Bound from the <c>Swagger</c> configuration section.
/// </summary>
[SuppressMessage("Design",
    "CA1515:Consider making public types internal",
    Justification =
        "Options must remain public to allow proper discovery and schema generation by OpenAPI.")]
public sealed record SwaggerOptions : IEqualityOperators<SwaggerOptions, SwaggerOptions, bool>
{
    /// <summary>
    ///     The configuration section key used to bind <see cref="SwaggerOptions"/>.
    /// </summary>
    public const string Swagger = nameof(Swagger);

    /// <summary>
    ///     The route template for the generated OpenAPI JSON document (e.g. <c>/swagger/{documentName}/swagger.json</c>).
    /// </summary>
    [Required]
    public required string JsonRoute { get; init; }

    /// <summary>
    ///     A brief, one-line description of the API surfaced in the OpenAPI info object.
    /// </summary>
    [Required]
    public required string ShortDescription { get; init; }

    /// <summary>
    ///     The URL path where the Swagger UI is served (e.g. <c>/swagger</c>).
    /// </summary>
    [Required]
    public required string UiEndpoint { get; init; }

    /// <summary>
    ///     The HTML page title rendered in the browser tab when the Swagger UI is open.
    /// </summary>
    [Required]
    public required string DocumentTitle { get; init; }

    /// <summary>
    ///     The API title displayed at the top of the Swagger UI and in the OpenAPI info object.
    /// </summary>
    [Required]
    public required string Title { get; init; }

    /// <summary>
    ///     The API version string exposed in the OpenAPI document (e.g. <c>v1</c>).
    /// </summary>
    [Required]
    public required string VersionSwagger { get; init; }

    /// <summary>
    ///     A multi-line description of the API providing extended context in the OpenAPI info object.
    /// </summary>
    [Required]
    public required string LongDescription { get; init; }

    /// <summary>
    ///     The SPDX license identifier or license name associated with the API (e.g. <c>MIT</c>).
    /// </summary>
    [Required]
    public required string License { get; init; }

    /// <summary>
    ///     The public base URL of the API server, added to the OpenAPI <c>servers</c> list.
    /// </summary>
    [Required]
    public required Uri ServerUrl { get; init; }
}

/// <summary>
///     Source-generated <see cref="IValidateOptions{TOptions}"/> validator for <see cref="SwaggerOptions"/>.
/// </summary>
[OptionsValidator]
internal sealed partial class SwaggerOptionsValidator : IValidateOptions<SwaggerOptions>;