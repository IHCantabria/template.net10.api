using System.Diagnostics.CodeAnalysis;

namespace template.net10.api.Core.Exceptions;

/// <summary>
///     Base exception class for all API-related errors in the application.
/// </summary>
[SuppressMessage("Design",
    "CA1515:Consider making public types internal",
    Justification =
        "Exception types are part of the public contract and must remain public to be consumed by external callers.")]
public abstract class CoreException : Exception
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="CoreException" /> class.
    /// </summary>
    protected CoreException()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="CoreException" /> class with a specified error message.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    protected CoreException(string message) : base(message)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="CoreException" /> class with a specified error message and inner
    ///     exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    protected CoreException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

/// <summary>
///     Represents an unhandled exception that reaches the global exception handler.
///     Thrown when an unexpected error occurs that has not been caught by any specific
///     exception-handling middleware or domain layer, ensuring a consistent error response
///     is returned to the client.
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
public sealed class GlobalUnhandledException : CoreException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="CoreException" /> class.
    /// </summary>
    public GlobalUnhandledException()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="CoreException" /> class with a specified error message.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    public GlobalUnhandledException(string message) : base(message)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="CoreException" /> class with a specified error message and inner
    ///     exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public GlobalUnhandledException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

/// <summary>
///     Represents the error that occurs when a requested entity cannot be found in the data store.
///     Mapped to an HTTP 404 Not Found response by the global exception handler.
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
public sealed class EntityNotFoundException : CoreException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="CoreException" /> class.
    /// </summary>
    public EntityNotFoundException()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="CoreException" /> class with a specified error message.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    public EntityNotFoundException(string message) : base(message)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="CoreException" /> class with a specified error message and inner
    ///     exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public EntityNotFoundException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

/// <summary>
///     Represents the error that occurs when building or producing a controller <c>ActionResult</c> fails.
///     Used in the result-mapping pipeline to propagate failures encountered while translating
///     domain results into HTTP responses.
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
public sealed class ActionResultException : CoreException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="CoreException" /> class.
    /// </summary>
    public ActionResultException()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="CoreException" /> class with a specified error message.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    public ActionResultException(string message) : base(message)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="CoreException" /> class with a specified error message and inner
    ///     exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public ActionResultException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

/// <summary>
///     Represents a failure that occurs during a database transaction (e.g. commit, rollback or
///     constraint violation). Thrown to signal that the unit-of-work could not be completed and
///     any partial changes have been rolled back.
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
public sealed class DatabaseTransactionException : CoreException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="CoreException" /> class.
    /// </summary>
    public DatabaseTransactionException()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="CoreException" /> class with a specified error message.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    public DatabaseTransactionException(string message) : base(message)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="CoreException" /> class with a specified error message and inner
    ///     exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public DatabaseTransactionException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

/// <summary>
///     Represents a failure that occurs during JSON serialization or deserialization.
///     Thrown when a payload cannot be serialized to or deserialized from JSON, allowing
///     the global exception handler to return a consistent error response.
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
public sealed class JsonSerializerException : CoreException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="CoreException" /> class.
    /// </summary>
    public JsonSerializerException()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="CoreException" /> class with a specified error message.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    public JsonSerializerException(string message) : base(message)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="CoreException" /> class with a specified error message and inner
    ///     exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public JsonSerializerException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

/// <summary>
///     Represents an HTTP 400 Bad Request error.
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
public sealed class BadRequestException : CoreException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="BadRequestException" /> class.
    /// </summary>
    public BadRequestException()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="BadRequestException" /> class with a specified error message.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    public BadRequestException(string message) : base(message)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="BadRequestException" /> class with a specified error message and inner
    ///     exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public BadRequestException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

/// <summary>
///     Represents an HTTP 401 Unauthorized error.
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
public sealed class UnauthorizedException : CoreException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="UnauthorizedException" /> class.
    /// </summary>
    public UnauthorizedException()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="UnauthorizedException" /> class with a specified error message.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    public UnauthorizedException(string message) : base(message)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="UnauthorizedException" /> class with a specified error message and
    ///     inner exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public UnauthorizedException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

/// <summary>
///     Represents an HTTP 403 Forbidden error.
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
public sealed class ForbiddenException : CoreException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ForbiddenException" /> class.
    /// </summary>
    public ForbiddenException()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ForbiddenException" /> class with a specified error message.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    public ForbiddenException(string message) : base(message)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ForbiddenException" /> class with a specified error message and inner
    ///     exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public ForbiddenException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

/// <summary>
///     Represents an HTTP 404 Not Found error.
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
public sealed class NotFoundException : CoreException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="NotFoundException" /> class.
    /// </summary>
    public NotFoundException()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="NotFoundException" /> class with a specified error message.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    public NotFoundException(string message) : base(message)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="NotFoundException" /> class with a specified error message and inner
    ///     exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public NotFoundException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

/// <summary>
///     Represents an HTTP 408 Request Timeout error.
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
public sealed class RequestTimeoutException : CoreException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="RequestTimeoutException" /> class.
    /// </summary>
    public RequestTimeoutException()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="RequestTimeoutException" /> class with a specified error message.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    public RequestTimeoutException(string message) : base(message)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="RequestTimeoutException" /> class with a specified error message and
    ///     inner exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public RequestTimeoutException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

/// <summary>
///     Represents an HTTP 409 Conflict error.
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
public sealed class ConflictException : CoreException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ConflictException" /> class.
    /// </summary>
    public ConflictException()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ConflictException" /> class with a specified error message.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    public ConflictException(string message) : base(message)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ConflictException" /> class with a specified error message and inner
    ///     exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public ConflictException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

/// <summary>
///     Represents an HTTP 410 Gone error indicating the resource is no longer available.
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
public sealed class GoneException : CoreException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="GoneException" /> class.
    /// </summary>
    public GoneException()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="GoneException" /> class with a specified error message.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    public GoneException(string message) : base(message)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="GoneException" /> class with a specified error message and inner
    ///     exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public GoneException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

/// <summary>
///     Represents an HTTP 422 Unprocessable Entity error.
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
public sealed class UnprocessableEntityException : CoreException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="UnprocessableEntityException" /> class.
    /// </summary>
    public UnprocessableEntityException()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="UnprocessableEntityException" /> class with a specified error message.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    public UnprocessableEntityException(string message) : base(message)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="UnprocessableEntityException" /> class with a specified error message
    ///     and inner exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public UnprocessableEntityException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

/// <summary>
///     Represents an HTTP 500 Internal Server Error.
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
public sealed class InternalServerErrorException : CoreException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="InternalServerErrorException" /> class.
    /// </summary>
    public InternalServerErrorException()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="InternalServerErrorException" /> class with a specified error message.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    public InternalServerErrorException(string message) : base(message)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="InternalServerErrorException" /> class with a specified error message
    ///     and inner exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public InternalServerErrorException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

/// <summary>
///     Represents an HTTP 501 Not Implemented error.
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
public sealed class NotImplementedException : CoreException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="NotImplementedException" /> class.
    /// </summary>
    public NotImplementedException()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="NotImplementedException" /> class with a specified error message.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    public NotImplementedException(string message) : base(message)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="NotImplementedException" /> class with a specified error message and
    ///     inner exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public NotImplementedException(string message, Exception innerException) : base(
        message, innerException)
    {
    }
}