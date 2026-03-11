using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace template.net10.api.Core.Extensions;

/// <summary>
///     Provides extension methods for string manipulation including cleaning, diacritic removal, and unwanted character
///     stripping.
/// </summary>
[SuppressMessage(
    "ReSharper",
    "UnusedType.Global",
    Justification = "General-purpose helper type; usage depends on consumer requirements.")]
[SuppressMessage(
    "ReSharper",
    "UnusedMember.Global",
    Justification = "General-purpose helper methods; not all members are used in every scenario.")]
internal static partial class StringExtensions
{
    /// <summary>
    ///     Gets a compiled regular expression that matches control characters, line/paragraph separators, zero-width
    ///     characters, and symbol characters.
    /// </summary>
    /// <returns>A <see cref="Regex" /> instance matching unwanted characters.</returns>
    [GeneratedRegex(@"[\p{C}\p{Zl}\p{Zp}\u200B-\u200D\uFEFF\p{So}]")]
    private static partial Regex UnwantedCharsRegex();

    /// <summary>
    ///     Removes unwanted characters (control chars, zero-width chars, symbols) from the input string after trimming
    ///     whitespace.
    /// </summary>
    /// <param name="input">The string to clean.</param>
    /// <returns>
    ///     The cleaned string with unwanted characters removed, or <see cref="string.Empty" /> if the input is null or
    ///     whitespace.
    /// </returns>
    private static string RemoveUnwantedCharacters(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return string.Empty;

        // Trim spaces and remove unwanted characters using regex
        var trimmedInput = input.AsSpan().Trim();
        return trimmedInput.IsEmpty ? string.Empty : UnwantedCharsRegex().Replace(trimmedInput.ToString(), "");
    }

    /// <summary>
    ///     Removes diacritical marks (accents) from the specified text by decomposing and recomposing Unicode characters.
    /// </summary>
    /// <param name="text">The text from which to remove diacritics.</param>
    /// <returns>The text with all diacritical marks removed.</returns>
    /// <exception cref="ArgumentException">The current instance contains invalid Unicode characters.</exception>
    [SuppressMessage(
        "ReSharper",
        "ExceptionNotDocumentedOptional",
        Justification =
            "Potential exceptions originate from underlying implementation details and are not part of the method contract.")]
    internal static string RemoveDiacritics(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return text;

        return string.Concat(
            text.Normalize(NormalizationForm.FormD)
                .Where(static c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
        ).Normalize(NormalizationForm.FormC);
    }

    extension(string input)
    {
        /// <summary>
        ///     Cleans the string by removing unwanted characters and optionally removing additional specified characters.
        /// </summary>
        /// <param name="additionalCharsToRemove">An optional array of additional characters to remove from the string.</param>
        /// <returns>The cleaned string.</returns>
        [SuppressMessage(
            "ReSharper",
            "ExceptionNotDocumentedOptional",
            Justification =
                "Potential exceptions originate from underlying implementation details and are not part of the method contract.")]
        internal string CleanString(char[]? additionalCharsToRemove = null)
        {
            // Remove unwanted characters first
            var cleaned = RemoveUnwantedCharacters(input);

            // If no additional characters to remove, return the result
            if (additionalCharsToRemove is not { Length: not 0 })
                return cleaned;

            // Remove additional characters
            StringBuilder finalCleaned = new(cleaned.Length);
            foreach (var c in cleaned.Where(c => !additionalCharsToRemove.Contains(c)))
                finalCleaned.Append(c);

            return finalCleaned.ToString();
        }
    }
}