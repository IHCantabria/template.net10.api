using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Path = System.IO.Path;

namespace template.net10.api.Settings.Attributes;

/// <summary>
///     Validation attribute that ensures a string value is a valid local relative path:
///     not rooted and not a UNC path.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
[SuppressMessage(
    "ReSharper",
    "UnusedType.Global",
    Justification =
        "Demonstration/base attribute provided as reusable validation example; usage depends on consumer scenarios.")]
internal sealed class LocalRelativePathAttribute : ValidationAttribute
{
    /// <summary>
    ///     Validates that <paramref name="value"/> is a non-null string that represents a local relative path.
    /// </summary>
    /// <param name="value">The value to validate.</param>
    /// <param name="validationContext">Provides context about the validation operation.</param>
    /// <returns><see cref="ValidationResult.Success"/> when valid; a <see cref="ValidationResult"/> with an error message otherwise.</returns>
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not string path) return new ValidationResult("Invalid type for relative path validation.");

        return IsRelativePath(path)
            ? ValidationResult.Success
            : new ValidationResult(GetErrorMessage(path));
    }

    /// <summary>
    ///     Returns <see langword="true"/> when <paramref name="path"/> is not rooted and not a UNC path.
    /// </summary>
    /// <param name="path">The path string to check.</param>
    /// <returns><see langword="true"/> if the path is a valid relative path; otherwise <see langword="false"/>.</returns>
    private static bool IsRelativePath(string path)
    {
        // Check if the path is NOT rooted, meaning it is relative
        return !Path.IsPathRooted(path) && !IsUncPath(path);
    }

    /// <summary>
    ///     Returns <see langword="true"/> when <paramref name="path"/> is a UNC network path.
    /// </summary>
    /// <param name="path">The path string to check.</param>
    /// <returns><see langword="true"/> if the path is a UNC path; otherwise <see langword="false"/>.</returns>
    private static bool IsUncPath(string path)
    {
        // Check if the path is a UNC path
        return Uri.TryCreate(path, UriKind.Absolute, out var uri) && uri.IsUnc;
    }

    /// <summary>
    ///     Builds a user-friendly validation error message for <paramref name="path"/>.
    /// </summary>
    /// <param name="path">The invalid path string.</param>
    /// <returns>A descriptive error message string.</returns>
    private static string GetErrorMessage(string path)
    {
        return $"Invalid relative path: {path}. The path must not be absolute or a UNC path.";
    }
}