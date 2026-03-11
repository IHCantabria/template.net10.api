using System.Diagnostics.CodeAnalysis;

namespace template.net10.api.Core.File;

/// <summary>
///     Provides utility methods for reading and converting uploaded files to byte arrays.
/// </summary>
[SuppressMessage(
    "ReSharper",
    "UnusedType.Global",
    Justification = "General-purpose helper type; usage depends on consumer requirements.")]
[SuppressMessage(
    "ReSharper",
    "UnusedMember.Global",
    Justification = "General-purpose helper methods; not all members are used in every scenario.")]
internal static class FileReader
{
    /// <summary>
    ///     Asynchronously converts an <see cref="IFormFile" /> to a byte array using a buffered read strategy.
    /// </summary>
    /// <param name="file">The uploaded file to convert.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <param name="bufferSize">The size of the read buffer in bytes. Defaults to 65536.</param>
    /// <returns>A <see cref="LanguageExt.Common.Result{A}" /> containing the file contents as a byte array.</returns>
    [SuppressMessage(
        "ReSharper",
        "ExceptionNotDocumented",
        Justification =
            "Potential exceptions originate from underlying implementation details and are not part of the method contract.")]
    [SuppressMessage(
        "ReSharper",
        "ExceptionNotDocumentedOptional",
        Justification =
            "Potential exceptions originate from underlying implementation details and are not part of the method contract.")]
    [SuppressMessage("ReSharper", "CA2007",
        Justification =
            "ConfigureAwait cant be injected in the Buffer Memory Stream")]
    internal static async Task<LanguageExt.Common.Result<byte[]>> ConvertFileToByteArrayAsync(IFormFile file,
        CancellationToken cancellationToken,
        int bufferSize = 65536)
    {
        await using var memoryStream = new MemoryStream();
        var stream = file.OpenReadStream();
        await using (stream)
        {
            var buffer = new byte[bufferSize];
            int bytesRead;

            while ((bytesRead = await stream.ReadAsync(buffer.AsMemory(0, bufferSize), cancellationToken)
                       .ConfigureAwait(false)) >
                   0)
                await memoryStream.WriteAsync(buffer.AsMemory(0, bytesRead), cancellationToken)
                    .ConfigureAwait(false);
        }

        return memoryStream.ToArray();
    }

    /// <summary>
    ///     Synchronously converts an <see cref="IFormFile" /> to a byte array using a buffered read strategy.
    /// </summary>
    /// <param name="file">The uploaded file to convert.</param>
    /// <param name="bufferSize">The size of the read buffer in bytes. Defaults to 65536.</param>
    /// <returns>A <see cref="LanguageExt.Common.Result{A}" /> containing the file contents as a byte array.</returns>
    [SuppressMessage(
        "ReSharper",
        "ExceptionNotDocumented",
        Justification =
            "Potential exceptions originate from underlying implementation details and are not part of the method contract.")]
    [SuppressMessage(
        "ReSharper",
        "ExceptionNotDocumentedOptional",
        Justification =
            "Potential exceptions originate from underlying implementation details and are not part of the method contract.")]
    internal static LanguageExt.Common.Result<byte[]> ConvertFileToByteArray(IFormFile file,
        int bufferSize = 65536)
    {
        using var memoryStream = new MemoryStream();
        var stream = file.OpenReadStream();
        using (stream)
        {
            var buffer = new byte[bufferSize];
            int bytesRead;

            while ((bytesRead = stream.Read(buffer.AsMemory(0, bufferSize).Span)) > 0)
                memoryStream.Write(buffer.AsMemory(0, bytesRead).Span);
        }

        return memoryStream.ToArray();
    }
}