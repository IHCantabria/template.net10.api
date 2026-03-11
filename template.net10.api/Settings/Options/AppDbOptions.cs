using System.ComponentModel.DataAnnotations;
using System.Numerics;
using System.Web;
using Microsoft.Extensions.Options;
using template.net10.api.Settings.Interfaces;

namespace template.net10.api.Settings.Options;

/// <summary>
///     Strongly-typed configuration options for the application database connection.
///     Bound from the <c>Connections:AppDb</c> configuration section.
/// </summary>
internal sealed record AppDbOptions : IConnectionsOptions,
    IEqualityOperators<AppDbOptions, AppDbOptions, bool>
{
    /// <summary>
    ///     The configuration section key used to bind <see cref="AppDbOptions"/>.
    /// </summary>
    public const string AppDb = $"{IConnectionsOptions.Connections}:{nameof(AppDb)}";

    /// <summary>
    ///     The raw (possibly HTML-encoded) database connection string.
    /// </summary>
    [Required]
    public required string ConnectionString { get; init; }

    /// <summary>
    ///     The default schema to use for database operations, or <see langword="null"/> to use the provider default.
    /// </summary>
    public required string? Schema { get; init; }

    /// <summary>
    ///     The HTML-decoded version of <see cref="ConnectionString"/>, ready for use by EF Core.
    /// </summary>
    public string DecodedConnectionString => HttpUtility.HtmlDecode(ConnectionString);
}

/// <summary>
///     Source-generated <see cref="IValidateOptions{TOptions}"/> validator for <see cref="AppDbOptions"/>.
/// </summary>
[OptionsValidator]
internal sealed partial class AppDbOptionsValidator : IValidateOptions<AppDbOptions>;