using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Microsoft.Extensions.Options;
using template.net10.api.Settings.Attributes;

namespace template.net10.api.Settings.Options;

/// <summary>
///     Strongly-typed configuration options for local file storage, defining root paths for temporary
///     and permanent file operations. Bound from the <c>FileStorage</c> configuration section.
/// </summary>
[SuppressMessage(
    "Performance",
    "IDE0051:Remove unused private members",
    Justification = "Instantiated via configuration binding (IOptions) when the feature is enabled.")]
[SuppressMessage(
    "Performance",
    "UnusedAutoPropertyAccessor.Global",
    Justification = "Instantiated via configuration binding (IOptions) when the feature is enabled.")]
internal sealed record FileStorageOptions : IEqualityOperators<FileStorageOptions, FileStorageOptions, bool>
{
    /// <summary>
    ///     The configuration section key used to bind <see cref="FileStorageOptions" />.
    /// </summary>
    public const string FileStorage = nameof(FileStorage);

    /// <summary>
    ///     The absolute path to the root directory used for temporary file and directory creation.
    ///     Validated by <see cref="LocalAbsolutePathAttribute" />.
    /// </summary>
    [LocalAbsolutePath]
    public required string RootTempPath { get; init; }

    /// <summary>
    ///     The absolute path to the root directory used for permanent file storage.
    ///     Validated by <see cref="LocalAbsolutePathAttribute" />.
    /// </summary>
    [LocalAbsolutePath]
    public required string RootFilePath { get; init; }
}

/// <summary>
///     Source-generated <see cref="IValidateOptions{TOptions}" /> validator for <see cref="FileStorageOptions" />.
/// </summary>
[OptionsValidator]
[SuppressMessage(
    "ReSharper",
    "UnusedType.Global",
    Justification =
        "Validator for options instantiated via configuration binding (IOptions) when the feature is enabled.")]
internal sealed partial class FileStorageOptionsValidator : IValidateOptions<FileStorageOptions>;