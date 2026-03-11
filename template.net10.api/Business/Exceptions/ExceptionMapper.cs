using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using template.net10.api.Business.Factory;
using template.net10.api.Core.Exceptions;
using template.net10.api.Localize.Resources;
using NotImplementedException = template.net10.api.Core.Exceptions.NotImplementedException;

namespace template.net10.api.Business.Exceptions;

/// <summary>
///     Defines the supported exception types mapped to their corresponding HTTP status codes.
/// </summary>
internal enum ExceptionType
{
    /// <summary>
    ///     Represents an HTTP 400 Bad Request error.
    /// </summary>
    BadRequest = 400,


    /// <summary>
    ///     Represents an HTTP 401 Unauthorized error.
    /// </summary>
    Unauthorized = 401,

    /// <summary>
    ///     Represents an HTTP 403 Forbidden error.
    /// </summary>
    Forbidden = 403,

    /// <summary>
    ///     Represents an HTTP 404 Not Found error.
    /// </summary>
    NotFound = 404,

    /// <summary>
    ///     Represents an HTTP 408 Request Timeout error.
    /// </summary>
    RequestTimeout = 408,

    /// <summary>
    ///     Represents an HTTP 409 Conflict error.
    /// </summary>
    Conflict = 409,

    /// <summary>
    ///     Represents an HTTP 410 Gone error.
    /// </summary>
    Gone = 410,

    /// <summary>
    ///     Represents an HTTP 422 Unprocessable Entity error.
    /// </summary>
    UnprocessableEntity = 422,

    /// <summary>
    ///     Represents an HTTP 500 Internal Server Error.
    /// </summary>
    InternalServerError = 500,

    /// <summary>
    ///     Represents an HTTP 501 Not Implemented error.
    /// </summary>
    NotImplemented = 501,

    /// <summary>
    ///     Represents a validation error (custom code 601, not a standard HTTP status).
    /// </summary>
    Validation = 601,

    /// <summary>
    ///     Represents an unsupported exception type (custom code 602, not a standard HTTP status).
    /// </summary>
    NotSupported = 602

    // Add more exception types as needed
}

/// <summary>
///     Maps domain and business exceptions to their corresponding <see cref="IActionResult" /> HTTP responses.
/// </summary>
internal static class ExceptionMapper
{
    /// <summary>
    ///     Lookup dictionary that maps each <see cref="ExceptionType" /> to a factory delegate that produces the corresponding
    ///     <see cref="IActionResult" />.
    /// </summary>
    private static readonly Dictionary<ExceptionType,
            Func<Exception, IStringLocalizer<ResourceMain>, IFeatureCollection, IActionResult>>
        ActionResultHandlers = new()
        {
            {
                ExceptionType.BadRequest,
                HttpResultFactory.CreateBadRequestResult
            },
            {
                ExceptionType.Unauthorized,
                HttpResultFactory.CreateUnauthorizedResult
            },
            {
                ExceptionType.Forbidden,
                HttpResultFactory.CreateForbiddenResult
            },
            {
                ExceptionType.NotFound,
                HttpResultFactory.CreateNotFoundResult
            },
            {
                ExceptionType.Conflict,
                HttpResultFactory.CreateConflictResult
            },
            {
                ExceptionType.RequestTimeout,
                HttpResultFactory.CreateRequestTimeoutResult
            },
            { ExceptionType.Gone, HttpResultFactory.CreateGoneResult },
            {
                ExceptionType.Validation, static (ex, localizer, features) =>
                    HttpResultFactory.CreateValidationResult((ValidationException)ex, localizer, features)
            },
            {
                ExceptionType.UnprocessableEntity,
                HttpResultFactory.CreateUnprocessableEntityResult
            },
            {
                ExceptionType.InternalServerError,
                HttpResultFactory.CreateInternalServerErrorResult
            },
            {
                ExceptionType.NotImplemented,
                HttpResultFactory.CreateNotImplementedResult
            }
            // Add more exception type mappings as needed
        };

    /// <summary>
    ///     Maps the specified exception to the corresponding <see cref="IActionResult" /> based on its runtime type.
    /// </summary>
    /// <param name="ex">The exception to map.</param>
    /// <param name="localizer">The string localizer for resolving localized error messages.</param>
    /// <param name="features">The HTTP feature collection for the current request.</param>
    /// <returns>An <see cref="IActionResult" /> representing the mapped HTTP error response.</returns>
    /// <exception cref="Exception">A delegate callback throws an exception.</exception>
    /// <exception cref="NotSupportedException">
    ///     Thrown when the exception type is not mapped to a known handler; the exception
    ///     type is not supported by the mapper.
    /// </exception>
    [SuppressMessage(
        "ReSharper",
        "ExceptionNotDocumentedOptional",
        Justification =
            "Potential exceptions originate from underlying implementation details and are not part of the method contract.")]
    internal static IActionResult MapExceptionToResult(Exception ex, IStringLocalizer<ResourceMain> localizer,
        IFeatureCollection features)
    {
        var exceptionType = GetExceptionType(ex);
        if (exceptionType is ExceptionType.NotSupported)
            throw new NotSupportedException(localizer["MapperExceptionResultNotSupported", exceptionType]);

        return ActionResultHandlers.TryGetValue(exceptionType, out var handler)
            ? handler(ex, localizer, features)
            : throw new NotSupportedException(localizer["MapperExceptionResultNotSupported", exceptionType]);
    }

    /// <summary>
    ///     Resolves the <see cref="ExceptionType" /> corresponding to the runtime type of the given exception.
    /// </summary>
    /// <param name="exception">The exception whose type is to be resolved.</param>
    /// <returns>
    ///     The <see cref="ExceptionType" /> that matches the exception, or <see cref="ExceptionType.NotSupported" /> if
    ///     no match is found.
    /// </returns>
    private static ExceptionType GetExceptionType(Exception exception)
    {
        return exception switch
        {
            BadRequestException => ExceptionType.BadRequest,
            UnauthorizedException => ExceptionType.Unauthorized,
            ForbiddenException => ExceptionType.Forbidden,
            NotFoundException => ExceptionType.NotFound,
            ConflictException => ExceptionType.Conflict,
            RequestTimeoutException => ExceptionType.RequestTimeout,
            GoneException => ExceptionType.Gone,
            ValidationException => ExceptionType.Validation,
            UnprocessableEntityException => ExceptionType.UnprocessableEntity,
            InternalServerErrorException => ExceptionType.InternalServerError,
            NotImplementedException => ExceptionType.NotImplemented,
            _ => ExceptionType.NotSupported
        };
    }
}