using System.Diagnostics.CodeAnalysis;
using template.net10.api.Core.Attributes;

namespace template.net10.api.Contracts.Interfaces;

/// <summary>
///     Defines the contract for a file-based API resource.
/// </summary>
[SuppressMessage("Design",
    "CA1515:Consider making public types internal",
    Justification =
        "The interface is part of the public API contract and must remain publicly accessible.")]
[PublicApiContract]
public interface IFileContract
{
    /// <summary>
    ///     Gets the raw file content as a byte sequence.
    /// </summary>
    IEnumerable<byte> Data { get; init; }

    /// <summary>
    ///     Gets the name of the file.
    /// </summary>
    string FileName { get; init; }

    /// <summary>
    ///     Gets the MIME content type of the file.
    /// </summary>
    string ContentType { get; init; }
}