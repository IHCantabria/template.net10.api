using System.Diagnostics.CodeAnalysis;
using template.net10.api.Core.Exceptions;

namespace template.net10.api.Core.Geometries;

/// <summary>
///     Exception thrown when a geometry extent (bounding box) does not satisfy validity constraints.
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
public sealed class GeometryExtentNotValidException : CoreException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="GeometryExtentNotValidException"/> class.
    /// </summary>
    public GeometryExtentNotValidException()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="GeometryExtentNotValidException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public GeometryExtentNotValidException(string message) : base(message)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="GeometryExtentNotValidException"/> class with a specified error message and inner exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public GeometryExtentNotValidException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

/// <summary>
///     Exception thrown when a geometry point does not satisfy validity constraints.
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
public sealed class GeometryPointNotValidException : CoreException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="GeometryPointNotValidException"/> class.
    /// </summary>
    public GeometryPointNotValidException()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="GeometryPointNotValidException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public GeometryPointNotValidException(string message) : base(message)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="GeometryPointNotValidException"/> class with a specified error message and inner exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public GeometryPointNotValidException(string message, Exception innerException) : base(message, innerException)
    {
    }
}