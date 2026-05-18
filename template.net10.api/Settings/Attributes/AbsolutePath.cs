using System.ComponentModel.DataAnnotations;
using Path = System.IO.Path;

namespace template.net10.api.Settings.Attributes;

/// <summary>
///     Validation attribute that ensures a string value is a valid absolute path
///     (local or UNC network path) without a trailing directory separator.
///     <list type="bullet">
///         <item>Local: <c>C:\Data\Files</c>, <c>T:\Storage</c></item>
///         <item>UNC:   <c>\\192.168.1.10\Share\Folder</c>, <c>//192.168.1.10/Share/Folder</c></item>
///     </list>
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
internal sealed class AbsolutePathAttribute : ValidationAttribute
{
    /// <summary>
    ///     Validates that <paramref name="value" /> is a non-null string that satisfies the absolute path rules.
    /// </summary>
    /// <param name="value">The value to validate.</param>
    /// <param name="validationContext">Provides context about the validation operation.</param>
    /// <returns>
    ///     <see cref="ValidationResult.Success" /> when valid; a <see cref="ValidationResult" /> with an error message
    ///     otherwise.
    /// </returns>
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not string path) return new ValidationResult("Invalid type for absolute path validation.");

        return IsAbsolutePath(path)
            ? ValidationResult.Success
            : new ValidationResult(GetErrorMessage(path));
    }

    /// <summary>
    ///     Returns <see langword="true" /> when <paramref name="path" /> is rooted (local or UNC)
    ///     and does not end with a directory separator.
    /// </summary>
    /// <param name="path">The path string to check.</param>
    /// <returns><see langword="true" /> if the path is a valid absolute path; otherwise <see langword="false" />.</returns>
    private static bool IsAbsolutePath(string path)
    {
        return Path.IsPathRooted(path) && !EndsWithDirectorySeparator(path);
    }

    /// <summary>
    ///     Returns <see langword="true" /> when <paramref name="path" /> ends with either
    ///     <see cref="Path.DirectorySeparatorChar" /> or <see cref="Path.AltDirectorySeparatorChar" />.
    /// </summary>
    private static bool EndsWithDirectorySeparator(string path)
    {
        return path is [.., var lastChar] &&
               (lastChar == Path.DirectorySeparatorChar || lastChar == Path.AltDirectorySeparatorChar);
    }

    /// <summary>
    ///     Builds a user-friendly validation error message describing why <paramref name="path" /> failed.
    /// </summary>
    /// <param name="path">The invalid path string.</param>
    /// <returns>A descriptive error message string.</returns>
    private static string GetErrorMessage(string path)
    {
        return EndsWithDirectorySeparator(path)
            ? $"Path must not end with a directory separator ('{Path.DirectorySeparatorChar}' or '{Path.AltDirectorySeparatorChar}'). Please remove the trailing separator."
            : $"Invalid absolute path (local or UNC): {path}";
    }
}