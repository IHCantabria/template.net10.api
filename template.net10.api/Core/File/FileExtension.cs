using System.Diagnostics.CodeAnalysis;
using Path = System.IO.Path;

namespace template.net10.api.Core.File;

/// <summary>
///     Utility methods for extracting file extensions, including support for
///     compound extensions such as <c>.cog.tiff</c> or <c>.tar.gz</c>.
/// </summary>
[SuppressMessage(
    "ReSharper",
    "UnusedType.Global",
    Justification = "General-purpose helper type; usage depends on consumer requirements.")]
[SuppressMessage(
    "ReSharper",
    "UnusedMember.Global",
    Justification = "General-purpose helper methods; not all members are used in every scenario.")]
internal static class FileExtension
{
    /// <summary>
    ///     Returns the extension of <paramref name="fileName" />, preferring a compound
    ///     extension from <paramref name="compoundExtensions" /> when the file name ends
    ///     with one of those entries; otherwise falls back to Path.GetExtension" />.
    ///     <para>
    ///         Matching is case-insensitive. Entries in <paramref name="compoundExtensions" />
    ///         must include the leading dot of the first segment (e.g. <c>.cog.tiff</c>).
    ///         The method strips any directory prefix from <paramref name="fileName" /> before
    ///         matching as a second line of defence against path traversal values.
    ///     </para>
    /// </summary>
    /// <param name="fileName">
    ///     The file name (with or without directory components) whose extension is to be resolved.
    /// </param>
    /// <param name="compoundExtensions">
    ///     Ordered list of known compound extensions to check before the standard fallback.
    ///     Pass an empty collection to apply standard single-extension behaviour.
    /// </param>
    /// <returns>
    ///     The matched compound extension (e.g. <c>.cog.tiff</c>), the standard single extension
    ///     (e.g. <c>.tiff</c>), or <see cref="string.Empty" /> when <paramref name="fileName" />
    ///     has no extension.
    /// </returns>
    [SuppressMessage(
        "ReSharper",
        "ExceptionNotDocumentedOptional",
        Justification =
            "Potential exceptions originate from underlying implementation details and are not part of the method contract.")]
    internal static string GetFileExtension(string fileName, IEnumerable<string> compoundExtensions)
    {
        var safeName = Path.GetFileName(fileName);

        var matchingCompound = compoundExtensions.FirstOrDefault(compound =>
            safeName.EndsWith(compound, StringComparison.OrdinalIgnoreCase));

        return matchingCompound ?? Path.GetExtension(safeName);
    }
}