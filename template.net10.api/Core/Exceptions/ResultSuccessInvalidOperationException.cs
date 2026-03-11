using System.Diagnostics.CodeAnalysis;

namespace template.net10.api.Core.Exceptions;

/// <summary>
///     Represents an invalid operation error thrown when attempting to access an exception from a successful <see cref="LanguageExt.Common.Result{T}"/>.
/// </summary>
[SuppressMessage("Design",
    "CA1515:Consider making public types internal",
    Justification =
        "Exception types are part of the public contract and must remain public to be consumed by external callers.")]
[SuppressMessage("ReSharper",
    "MemberCanBeInternal",
    Justification =
        "Exception types are part of the public contract and must remain public to be consumed by external callers.")]
[SuppressMessage("ReSharper",
    "ClassNeverInstantiated.Global",
    Justification =
        "Exception type is part of the public API; instantiation depends on usage by consumers or higher-level layers.")]
[SuppressMessage(
    "ReSharper",
    "UnusedMember.Global",
    Justification = "Standard exception constructor required by CA1032 and RCS1194.")]
public sealed class ResultSuccessInvalidOperationException : CoreException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ResultSuccessInvalidOperationException"/> class.
    /// </summary>
    public ResultSuccessInvalidOperationException()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ResultSuccessInvalidOperationException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    public ResultSuccessInvalidOperationException(string message) : base(message)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ResultSuccessInvalidOperationException"/> class with a specified error message and inner exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public ResultSuccessInvalidOperationException(string message, Exception innerException) : base(message,
        innerException)
    {
    }
}