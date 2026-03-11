using System.ComponentModel.DataAnnotations;
using Path = System.IO.Path;

namespace template.net10.api.Settings.Attributes;

/// <summary>
///     Validation attribute that ensures a string value is a valid local absolute path:
///     rooted, not a UNC path, and without a trailing directory separator.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
internal sealed class LocalAbsolutePathAttribute : ValidationAttribute
{
    /// <summary>
    ///     Validates that <paramref name="value"/> is a non-null string that satisfies the local absolute path rules.
    /// </summary>
    /// <param name="value">The value to validate.</param>
    /// <param name="validationContext">Provides context about the validation operation.</param>
    /// <returns><see cref="ValidationResult.Success"/> when valid; a <see cref="ValidationResult"/> with an error message otherwise.</returns>
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not string path) return new ValidationResult("Invalid type for local absolute path validation.");

        return IsLocalAbsolutePath(path)
            ? ValidationResult.Success
            : new ValidationResult(GetErrorMessage(path));
    }

    /// <summary>
    ///     Returns <see langword="true"/> when <paramref name="path"/> is rooted, not a UNC path, and does not end with a directory separator.
    /// </summary>
    /// <param name="path">The path string to check.</param>
    /// <returns><see langword="true"/> if the path is a valid local absolute path; otherwise <see langword="false"/>.</returns>
    private static bool IsLocalAbsolutePath(string path)
    {
        // Check if the path is rooted and not a UNC path
        return Path.IsPathRooted(path) && !IsUncPath(path) &&
               !(path is [.., var lastChar] && lastChar == Path.DirectorySeparatorChar);
    }

    /// <summary>
    ///     Returns <see langword="true"/> when <paramref name="path"/> is a UNC network path (e.g. <c>\\server\share</c>).
    /// </summary>
    /// <param name="path">The path string to check.</param>
    /// <returns><see langword="true"/> if the path is a UNC path; otherwise <see langword="false"/>.</returns>
    private static bool IsUncPath(string path)
    {
        // Check if the path is a UNC path
        return Uri.TryCreate(path, UriKind.Absolute, out var uri) && uri.IsUnc;
    }

    /// <summary>
    ///     Builds a user-friendly validation error message describing why <paramref name="path"/> failed.
    /// </summary>
    /// <param name="path">The invalid path string.</param>
    /// <returns>A descriptive error message string.</returns>
    private static string GetErrorMessage(string path)
    {
        return path is [.., var lastChar] && lastChar == Path.DirectorySeparatorChar
            ? "Path must not end with '/'. Please remove the trailing slash."
            : $"Invalid local absolute path: {path}";
    }
}