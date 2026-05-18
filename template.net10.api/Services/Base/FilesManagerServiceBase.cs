using System.Diagnostics.CodeAnalysis;
using LanguageExt;
using Microsoft.Extensions.Options;
using template.net10.api.Core.Attributes;
using template.net10.api.Core.Base;
using template.net10.api.Settings.Options;
using Path = System.IO.Path;

namespace template.net10.api.Services.Base;

/// <summary>
///     Base service for managing temporary files and directories on the local file system.
///     Provides helpers for creating, referencing, and cleaning up temporary storage paths
///     based on <see cref="FileStorageOptions" /> configuration.
/// </summary>
[ServiceLifetime(ServiceLifetime.Transient)]
[SuppressMessage(
    "ReSharper",
    "UnusedType.Global",
    Justification = "Base service/template intended for reuse; it may be referenced indirectly by derived services.")]
[SuppressMessage(
    "ReSharper",
    "UnusedMember.Global",
    Justification =
        "Protected helper members are intended for derived services; not all are used in every implementation.")]
internal class FilesManagerServiceBase : ServiceBase
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="FilesManagerServiceBase" /> class,
    ///     binding the <see cref="FileStorageOptions" /> configuration and registering the logger
    ///     for derived file-management services.
    /// </summary>
    /// <param name="config">
    ///     The <see cref="IOptions{TOptions}" /> wrapper for <see cref="FileStorageOptions" />, providing
    ///     the root paths used for temporary and permanent file storage operations.
    /// </param>
    /// <param name="logger">The logger instance for diagnostic output in derived services.</param>
    /// <exception cref="ArgumentNullException">
    ///     <paramref name="config" /> value is <see langword="null" />, or
    ///     <paramref name="logger" /> is <see langword="null" />.
    /// </exception>
    protected FilesManagerServiceBase(IOptions<FileStorageOptions> config, ILogger<FilesManagerServiceBase> logger) :
        base(logger)
    {
        Config = config.Value ?? throw new ArgumentNullException(nameof(config));
    }

    /// <summary>
    ///     File storage configuration containing root paths used for temporary file operations.
    /// </summary>
    private FileStorageOptions Config { get; }

    /// <summary>
    ///     Creates a new unique temporary directory under the configured root temp path.
    /// </summary>
    /// <returns>
    ///     A <see cref="LanguageExt.Try{A}" /> wrapping the full path of the newly created temporary directory.
    /// </returns>
    /// <exception cref="DirectoryNotFoundException">The specified path is invalid (for example, it is on an unmapped drive).</exception>
    /// <exception cref="UnauthorizedAccessException">The caller does not have the required permission.</exception>
    /// <exception cref="PathTooLongException">The specified path, file name, or both exceed the system-defined maximum length.</exception>
    [SuppressMessage(
        "ReSharper",
        "ExceptionNotDocumentedOptional",
        Justification =
            "Potential exceptions originate from underlying implementation details and are not part of the method contract.")]
    protected Try<string> CreateTempDirectory()
    {
        return () =>
        {
            var tempDirectory = Path.GetFullPath(Path.Combine(Config.RootTempPath, Path.GetRandomFileName()));
            Directory.CreateDirectory(tempDirectory);
            return tempDirectory;
        };
    }

    /// <summary>
    ///     Creates a new unique directory under the configured root file path.
    /// </summary>
    /// <param name="path">The partial path of the directory to create under Root File Path.</param>
    /// <returns>
    ///     A <see cref="LanguageExt.Try{A}" /> wrapping the full path of the newly created file directory.
    /// </returns>
    /// <exception cref="DirectoryNotFoundException">The specified path is invalid (for example, it is on an unmapped drive).</exception>
    /// <exception cref="UnauthorizedAccessException">The caller does not have the required permission.</exception>
    /// <exception cref="PathTooLongException">The specified path, file name, or both exceed the system-defined maximum length.</exception>
    [SuppressMessage(
        "ReSharper",
        "ExceptionNotDocumentedOptional",
        Justification =
            "Potential exceptions originate from underlying implementation details and are not part of the method contract.")]
    protected Try<string> CreateDirectory(string path)
    {
        return () =>
        {
            var directory = Path.GetFullPath(Path.Combine(Config.RootFilePath, path));
            Directory.CreateDirectory(directory);
            return directory;
        };
    }

    /// <summary>
    ///     Generates a unique temporary file path inside the given <paramref name="tempDirectory" /> without creating the
    ///     file.
    /// </summary>
    /// <param name="tempDirectory">The directory in which the temporary file path will be defined.</param>
    /// <returns>A <see cref="LanguageExt.Try{A}" /> wrapping the generated file path.</returns>
    [SuppressMessage(
        "ReSharper",
        "ExceptionNotDocumentedOptional",
        Justification =
            "Potential exceptions originate from underlying implementation details and are not part of the method contract.")]
    protected static Try<string> DefineTempFile(string tempDirectory)
    {
        return () => Path.Combine(tempDirectory, Path.GetRandomFileName());
    }

    /// <summary>
    ///     Deletes the specified directory and all its contents if it exists; no-op if it does not.
    /// </summary>
    /// <param name="filepDirectory">The full path of the directory to delete.</param>
    /// <returns>
    ///     A <see cref="LanguageExt.Try{A}" /> wrapping <see langword="true" /> when deletion succeeds or the directory
    ///     does not exist.
    /// </returns>
    /// <exception cref="UnauthorizedAccessException">The caller does not have the required permission.</exception>
    /// <exception cref="PathTooLongException">The specified path, file name, or both exceed the system-defined maximum length.</exception>
    [SuppressMessage(
        "ReSharper",
        "ExceptionNotDocumentedOptional",
        Justification =
            "Potential exceptions originate from underlying implementation details and are not part of the method contract.")]
    protected static Try<bool> TryDeleteFileDirectory(string filepDirectory)
    {
        return () =>
        {
            if (Directory.Exists(filepDirectory))
                Directory.Delete(filepDirectory, true);
            return true;
        };
    }
}