using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using template.net10.api.Core.Exceptions;

namespace template.net10.api.Business.Exceptions;

/// <summary>
///     Abstract base class for business-layer exceptions.
/// </summary>
[SuppressMessage("Design",
    "CA1515:Consider making public types internal",
    Justification =
        "Exception types are part of the public contract and must remain public to be consumed by external callers.")]
[UsedImplicitly]
public abstract class BusinessException : CoreException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="BusinessException"/> class.
    /// </summary>
    protected BusinessException()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="BusinessException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The error message that describes the exception.</param>
    protected BusinessException(string message) : base(message)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="BusinessException"/> class with a specified error message and a reference to the inner exception.
    /// </summary>
    /// <param name="message">The error message that describes the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    protected BusinessException(string message, Exception innerException) : base(message, innerException)
    {
    }
}