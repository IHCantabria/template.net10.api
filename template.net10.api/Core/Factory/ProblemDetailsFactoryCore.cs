using System.Diagnostics.CodeAnalysis;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Localization;
using template.net10.api.Localize.Resources;

namespace template.net10.api.Core.Factory;

/// <summary>
///     Provides factory methods for creating RFC 7807 <see cref="ProblemDetails" /> responses mapped to HTTP status codes.
/// </summary>
[SuppressMessage(
    "ReSharper",
    "ClassTooBig",
    Justification =
        "Centralized ProblemDetails factory. The class intentionally groups all HTTP status mappings in one place.")]
internal static class ProblemDetailsFactoryCore
{
    /// <summary>
    ///     Creates a <see cref="ProblemDetails" /> instance appropriate for the specified HTTP status code.
    /// </summary>
    /// <param name="httpStatusCode">The HTTP status code to map to a problem details response.</param>
    /// <param name="ex">The exception that triggered the error.</param>
    /// <param name="localizer">The string localizer for retrieving localized messages.</param>
    /// <returns>A <see cref="ProblemDetails" /> instance with the appropriate status, title, detail, and type.</returns>
    internal static ProblemDetails CreateProblemDetailsByHttpStatusCode(HttpStatusCode httpStatusCode, Exception ex,
        IStringLocalizer<ResourceMain> localizer)
    {
        return httpStatusCode switch
        {
            HttpStatusCode.BadRequest => CreateProblemDetailsBadRequest(ex, localizer),
            HttpStatusCode.Unauthorized => CreateProblemDetailsUnauthorized(ex, localizer),
            HttpStatusCode.Forbidden => CreateProblemDetailsForbidden(ex, localizer),
            HttpStatusCode.NotFound =>
                CreateProblemDetailsNotFound(ex, localizer),
            HttpStatusCode.MethodNotAllowed => CreateProblemDetailsMethodNotAllowed(ex, localizer),
            HttpStatusCode.RequestTimeout => CreateProblemDetailsRequestTimeout(ex, localizer),
            HttpStatusCode.Conflict => CreateProblemDetailsConflict(ex, localizer),
            HttpStatusCode.Gone => CreateProblemDetailsGone(ex, localizer),
            HttpStatusCode.UnsupportedMediaType => CreateProblemDetailsUnsupportedMediaType(ex, localizer),
            HttpStatusCode.UnprocessableEntity => CreateProblemDetailsUnprocessableEntity(ex, localizer),
            HttpStatusCode.TooManyRequests => CreateProblemDetailsTooManyRequest(ex, localizer),
            HttpStatusCode.InternalServerError => CreateProblemDetailsInternalServerError(ex, localizer),
            HttpStatusCode.NotImplemented => CreateProblemDetailsNotImplemented(ex, localizer),
            _ => CreateProblemDetailsInternalServerError(ex, localizer)
        };
    }

    /// <summary>
    ///     Creates a <see cref="ProblemDetails" /> for a bad request caused by model state validation failures.
    /// </summary>
    /// <param name="modelState">The model state dictionary containing validation errors.</param>
    /// <param name="localizer">The string localizer for retrieving localized messages.</param>
    /// <returns>A <see cref="ProblemDetails" /> with validation errors included as extensions.</returns>
    [SuppressMessage(
        "ReSharper",
        "ExceptionNotDocumentedOptional",
        Justification =
            "Potential exceptions originate from underlying implementation details and are not part of the method contract.")]
    internal static ProblemDetails CreateProblemDetailsBadRequestValidationPayload(ModelStateDictionary modelState,
        IStringLocalizer<ResourceMain> localizer)
    {
        var errors = modelState
            .ToDictionary(
                static kvp => kvp.Key,
                static kvp => kvp.Value?.Errors.Select(static e => e.ErrorMessage)
            )
            .Where(static k => k.Value?.Any() == true)
            .ToDictionary();
        return ProblemDetailsFactoryCoreUtils.IsRootFileFail(modelState.Keys)
            ? CreateProblemDetailsBadRequestValidationJsonMalformed(errors, localizer)
            : CreateProblemDetailsBadRequestValidationJsonInvalid(errors, localizer);
    }

    /// <summary>
    ///     Creates a <see cref="ProblemDetails" /> for a bad request caused by malformed JSON in the request body.
    /// </summary>
    /// <param name="errors">The dictionary of validation errors keyed by field name.</param>
    /// <param name="localizer">The string localizer for retrieving localized messages.</param>
    /// <returns>A <see cref="ProblemDetails" /> indicating the JSON payload is malformed.</returns>
    [SuppressMessage(
        "ReSharper",
        "ExceptionNotDocumentedOptional",
        Justification =
            "Potential exceptions originate from underlying implementation details and are not part of the method contract.")]
    private static ProblemDetails CreateProblemDetailsBadRequestValidationJsonMalformed(
        Dictionary<string, IEnumerable<string>?> errors,
        IStringLocalizer localizer)
    {
        var problemDetails = new ProblemDetails
        {
            Title = localizer["ProblemDetailsValidationTitle"],
            Detail = localizer["ProblemDetailsBadRequestValidationJsonMalformedDetail"],
            Type = "https://tools.ietf.org/html/rfc9110#name-400-bad-request",
            Status = StatusCodes.Status400BadRequest
        };
        problemDetails.Extensions.TryAdd("errors", errors);
        problemDetails.Extensions.TryAdd("code",
            localizer["ProblemDetailsBadRequestValidationJsonMalformedCode"].Value);
        return problemDetails;
    }

    /// <summary>
    ///     Creates a <see cref="ProblemDetails" /> for a bad request caused by invalid JSON values that fail validation rules.
    /// </summary>
    /// <param name="errors">The dictionary of validation errors keyed by field name.</param>
    /// <param name="localizer">The string localizer for retrieving localized messages.</param>
    /// <returns>A <see cref="ProblemDetails" /> indicating the JSON payload has invalid field values.</returns>
    [SuppressMessage(
        "ReSharper",
        "ExceptionNotDocumentedOptional",
        Justification =
            "Potential exceptions originate from underlying implementation details and are not part of the method contract.")]
    private static ProblemDetails CreateProblemDetailsBadRequestValidationJsonInvalid(
        Dictionary<string, IEnumerable<string>?> errors,
        IStringLocalizer localizer)
    {
        var problemDetails = new ProblemDetails
        {
            Title = localizer["ProblemDetailsValidationTitle"],
            Detail = localizer["ProblemDetailsBadRequestValidationJsonInvalidDetail"],
            Type = "https://tools.ietf.org/html/rfc9110#name-400-bad-request",
            Status = StatusCodes.Status400BadRequest
        };
        problemDetails.Extensions.TryAdd("errors", errors);
        problemDetails.Extensions.TryAdd("code", localizer["ProblemDetailsBadRequestValidationJsonInvalidCode"].Value);
        return problemDetails;
    }

    /// <summary>
    ///     Creates a <see cref="ProblemDetails" /> for an HTTP 400 Bad Request error.
    /// </summary>
    /// <param name="exception">The exception that caused the bad request.</param>
    /// <param name="localizer">The string localizer for retrieving localized messages.</param>
    /// <returns>A <see cref="ProblemDetails" /> with status 400.</returns>
    [SuppressMessage(
        "ReSharper",
        "ExceptionNotDocumentedOptional",
        Justification =
            "Potential exceptions originate from underlying implementation details and are not part of the method contract.")]
    internal static ProblemDetails CreateProblemDetailsBadRequest(Exception exception,
        IStringLocalizer<ResourceMain> localizer)
    {
        var problemDetails = new ProblemDetails
        {
            Title = localizer["ProblemDetailsBadRequestTitle"],
            Detail = exception.Message,
            Type = "https://tools.ietf.org/html/rfc9110#name-400-bad-request",
            Status = StatusCodes.Status400BadRequest
        };
        problemDetails.Extensions.TryAdd("code", localizer["ProblemDetailsBadRequestCode"].Value);
        return problemDetails;
    }

    /// <summary>
    ///     Creates a <see cref="ProblemDetails" /> for an unsupported HTTP method or protocol version.
    /// </summary>
    /// <param name="localizer">The string localizer for retrieving localized messages.</param>
    /// <returns>A <see cref="ProblemDetails" /> indicating the HTTP method is not supported.</returns>
    [SuppressMessage(
        "ReSharper",
        "ExceptionNotDocumentedOptional",
        Justification =
            "Potential exceptions originate from underlying implementation details and are not part of the method contract.")]
    internal static ProblemDetails CreateProblemDetailsBadRequestHttpNotSupported(
        IStringLocalizer<ResourceMain> localizer)
    {
        var problemDetails = new ProblemDetails
        {
            Title = localizer["ProblemDetailsBadRequestHttpNotSupportedTitle"],
            Detail = localizer["ProblemDetailsBadRequestHttpNotSupportedDetail"],
            Type = "https://tools.ietf.org/html/rfc9110#name-400-bad-request",
            Status = StatusCodes.Status500InternalServerError
        };
        problemDetails.Extensions.TryAdd("code", localizer["ProblemDetailsBadRequestHttpNotSupportedCode"].Value);
        return problemDetails;
    }

    /// <summary>
    ///     Creates a <see cref="ProblemDetails" /> for an HTTP 401 Unauthorized error.
    /// </summary>
    /// <param name="exception">The exception that caused the unauthorized error.</param>
    /// <param name="localizer">The string localizer for retrieving localized messages.</param>
    /// <returns>A <see cref="ProblemDetails" /> with status 401.</returns>
    [SuppressMessage(
        "ReSharper",
        "ExceptionNotDocumentedOptional",
        Justification =
            "Potential exceptions originate from underlying implementation details and are not part of the method contract.")]
    internal static ProblemDetails CreateProblemDetailsUnauthorized(Exception exception,
        IStringLocalizer<ResourceMain> localizer)
    {
        var problemDetails = new ProblemDetails
        {
            Title = localizer["ProblemDetailsUnauthorizedTitle"],
            Detail = exception.Message,
            Type = "https://tools.ietf.org/html/rfc9110#name-401-unauthorized",
            Status = StatusCodes.Status401Unauthorized
        };
        problemDetails.Extensions.TryAdd("code", localizer["ProblemDetailsUnauthorizedCode"].Value);
        return problemDetails;
    }

    /// <summary>
    ///     Creates a <see cref="ProblemDetails" /> for an HTTP 401 Unauthorized error when the authentication token is
    ///     missing.
    /// </summary>
    /// <param name="localizer">The string localizer for retrieving localized messages.</param>
    /// <returns>A <see cref="ProblemDetails" /> with status 401 indicating a missing token.</returns>
    [SuppressMessage(
        "ReSharper",
        "ExceptionNotDocumentedOptional",
        Justification =
            "Potential exceptions originate from underlying implementation details and are not part of the method contract.")]
    internal static ProblemDetails CreateProblemDetailsUnauthorizedMissingToken(
        IStringLocalizer<ResourceMain> localizer)
    {
        var problemDetails = new ProblemDetails
        {
            Title = localizer["ProblemDetailsUnauthorizedMissingTokenTitle"],
            Detail = localizer["ProblemDetailsUnauthorizedMissingTokenDetail"],
            Type = "https://tools.ietf.org/html/rfc9110#name-401-unauthorized",
            Status = StatusCodes.Status401Unauthorized
        };
        problemDetails.Extensions.TryAdd("code", localizer["ProblemDetailsUnauthorizedMissingTokenCode"].Value);
        return problemDetails;
    }

    /// <summary>
    ///     Creates a <see cref="ProblemDetails" /> for an HTTP 401 Unauthorized error when authentication processing fails.
    /// </summary>
    /// <param name="exception">The exception that caused the authentication failure.</param>
    /// <param name="localizer">The string localizer for retrieving localized messages.</param>
    /// <returns>A <see cref="ProblemDetails" /> with status 401 indicating an authentication process failure.</returns>
    [SuppressMessage(
        "ReSharper",
        "ExceptionNotDocumentedOptional",
        Justification =
            "Potential exceptions originate from underlying implementation details and are not part of the method contract.")]
    internal static ProblemDetails CreateProblemDetailsUnauthorizedProcessFail(Exception exception,
        IStringLocalizer<ResourceMain> localizer)
    {
        var problemDetails = new ProblemDetails
        {
            Title = localizer["ProblemDetailsUnauthorizedProcessFailTitle"],
            Detail = exception.Message,
            Type = "https://tools.ietf.org/html/rfc9110#name-401-unauthorized",
            Status = StatusCodes.Status401Unauthorized
        };
        problemDetails.Extensions.TryAdd("code", localizer["ProblemDetailsUnauthorizedProcessFailCode"].Value);
        return problemDetails;
    }

    /// <summary>
    ///     Creates a <see cref="ProblemDetails" /> for an HTTP 403 Forbidden error when access is denied.
    /// </summary>
    /// <param name="localizer">The string localizer for retrieving localized messages.</param>
    /// <returns>A <see cref="ProblemDetails" /> with status 403 indicating forbidden access.</returns>
    [SuppressMessage(
        "ReSharper",
        "ExceptionNotDocumentedOptional",
        Justification =
            "Potential exceptions originate from underlying implementation details and are not part of the method contract.")]
    internal static ProblemDetails CreateProblemDetailsForbiddenAccess(IStringLocalizer<ResourceMain> localizer)
    {
        var problemDetails = new ProblemDetails
        {
            Title = localizer["ProblemDetailsForbiddenAccessTitle"],
            Detail = localizer["ProblemDetailsForbiddenAccessDetail"],
            Type = "https://tools.ietf.org/html/rfc9110#name-403-forbidden",
            Status = StatusCodes.Status403Forbidden
        };
        problemDetails.Extensions.TryAdd("code", localizer["ProblemDetailsForbiddenAccessCode"].Value);
        return problemDetails;
    }

    /// <summary>
    ///     Creates a <see cref="ProblemDetails" /> for an HTTP 403 Forbidden error.
    /// </summary>
    /// <param name="exception">The exception that caused the forbidden error.</param>
    /// <param name="localizer">The string localizer for retrieving localized messages.</param>
    /// <returns>A <see cref="ProblemDetails" /> with status 403.</returns>
    [SuppressMessage(
        "ReSharper",
        "ExceptionNotDocumentedOptional",
        Justification =
            "Potential exceptions originate from underlying implementation details and are not part of the method contract.")]
    internal static ProblemDetails CreateProblemDetailsForbidden(Exception exception,
        IStringLocalizer<ResourceMain> localizer)
    {
        var problemDetails = new ProblemDetails
        {
            Title = localizer["ProblemDetailsForbiddenTitle"],
            Detail = exception.Message,
            Type = "https://tools.ietf.org/html/rfc9110#name-403-forbidden",
            Status = StatusCodes.Status403Forbidden
        };
        problemDetails.Extensions.TryAdd("code", localizer["ProblemDetailsForbiddenCode"].Value);
        return problemDetails;
    }

    /// <summary>
    ///     Creates a <see cref="ProblemDetails" /> for an HTTP 404 Not Found error.
    /// </summary>
    /// <param name="exception">The exception that caused the not found error.</param>
    /// <param name="localizer">The string localizer for retrieving localized messages.</param>
    /// <returns>A <see cref="ProblemDetails" /> with status 404.</returns>
    [SuppressMessage(
        "ReSharper",
        "ExceptionNotDocumentedOptional",
        Justification =
            "Potential exceptions originate from underlying implementation details and are not part of the method contract.")]
    internal static ProblemDetails CreateProblemDetailsNotFound(Exception exception,
        IStringLocalizer<ResourceMain> localizer)
    {
        var problemDetails = new ProblemDetails
        {
            Title = localizer["ProblemDetailsNotFoundTitle"],
            Detail = exception.Message,
            Type = "https://tools.ietf.org/html/rfc9110#name-404-not-found",
            Status = StatusCodes.Status404NotFound
        };
        problemDetails.Extensions.TryAdd("code", localizer["ProblemDetailsNotFoundCode"].Value);
        return problemDetails;
    }

    /// <summary>
    ///     Creates a <see cref="ProblemDetails" /> for an HTTP 405 Method Not Allowed error.
    /// </summary>
    /// <param name="exception">The exception that caused the method not allowed error.</param>
    /// <param name="localizer">The string localizer for retrieving localized messages.</param>
    /// <returns>A <see cref="ProblemDetails" /> with status 405.</returns>
    [SuppressMessage(
        "ReSharper",
        "ExceptionNotDocumentedOptional",
        Justification =
            "Potential exceptions originate from underlying implementation details and are not part of the method contract.")]
    private static ProblemDetails CreateProblemDetailsMethodNotAllowed(Exception exception,
        IStringLocalizer localizer)
    {
        var problemDetails = new ProblemDetails
        {
            Title = localizer["ProblemDetailsMethodNotAllowedTitle"],
            Detail = exception.Message,
            Type = "https://tools.ietf.org/html/rfc9110#name-405-method-not-allowed",
            Status = StatusCodes.Status405MethodNotAllowed
        };
        problemDetails.Extensions.TryAdd("code", localizer["ProblemDetailsMethodNotAllowedCode"].Value);
        return problemDetails;
    }

    /// <summary>
    ///     Creates a <see cref="ProblemDetails" /> for an HTTP 408 Request Timeout error.
    /// </summary>
    /// <param name="exception">The exception that caused the request timeout.</param>
    /// <param name="localizer">The string localizer for retrieving localized messages.</param>
    /// <returns>A <see cref="ProblemDetails" /> with status 408.</returns>
    [SuppressMessage(
        "ReSharper",
        "ExceptionNotDocumentedOptional",
        Justification =
            "Potential exceptions originate from underlying implementation details and are not part of the method contract.")]
    internal static ProblemDetails CreateProblemDetailsRequestTimeout(Exception exception,
        IStringLocalizer<ResourceMain> localizer)
    {
        var problemDetails = new ProblemDetails
        {
            Title = localizer["ProblemDetailsRequestTimeoutTitle"],
            Detail = exception.Message,
            Type = "https://tools.ietf.org/html/rfc9110#name-408-request-timeout",
            Status = StatusCodes.Status408RequestTimeout
        };
        problemDetails.Extensions.TryAdd("code", localizer["ProblemDetailsRequestTimeoutCode"].Value);
        return problemDetails;
    }

    /// <summary>
    ///     Creates a <see cref="ProblemDetails" /> for an HTTP 409 Conflict error.
    /// </summary>
    /// <param name="exception">The exception that caused the conflict.</param>
    /// <param name="localizer">The string localizer for retrieving localized messages.</param>
    /// <returns>A <see cref="ProblemDetails" /> with status 409.</returns>
    [SuppressMessage(
        "ReSharper",
        "ExceptionNotDocumentedOptional",
        Justification =
            "Potential exceptions originate from underlying implementation details and are not part of the method contract.")]
    internal static ProblemDetails CreateProblemDetailsConflict(Exception exception,
        IStringLocalizer<ResourceMain> localizer)
    {
        var problemDetails = new ProblemDetails
        {
            Title = localizer["ProblemDetailsConflictTitle"],
            Detail = exception.Message,
            Type = "https://tools.ietf.org/html/rfc9110#name-409-conflict",
            Status = StatusCodes.Status409Conflict
        };
        problemDetails.Extensions.TryAdd("code", localizer["ProblemDetailsConflictCode"].Value);
        return problemDetails;
    }

    /// <summary>
    ///     Creates a <see cref="ProblemDetails" /> for an HTTP 410 Gone error.
    /// </summary>
    /// <param name="exception">The exception indicating the resource is no longer available.</param>
    /// <param name="localizer">The string localizer for retrieving localized messages.</param>
    /// <returns>A <see cref="ProblemDetails" /> with status 410.</returns>
    [SuppressMessage(
        "ReSharper",
        "ExceptionNotDocumentedOptional",
        Justification =
            "Potential exceptions originate from underlying implementation details and are not part of the method contract.")]
    internal static ProblemDetails CreateProblemDetailsGone(Exception exception,
        IStringLocalizer<ResourceMain> localizer)
    {
        var problemDetails = new ProblemDetails
        {
            Title = localizer["ProblemDetailsGoneTitle"],
            Detail = exception.Message,
            Type = "https://tools.ietf.org/html/rfc9110#name-410-gone",
            Status = StatusCodes.Status410Gone
        };
        problemDetails.Extensions.TryAdd("code", localizer["ProblemDetailsGoneCode"].Value);
        return problemDetails;
    }

    /// <summary>
    ///     Creates a <see cref="ProblemDetails" /> for an HTTP 415 Unsupported Media Type error.
    /// </summary>
    /// <param name="exception">The exception that caused the unsupported media type error.</param>
    /// <param name="localizer">The string localizer for retrieving localized messages.</param>
    /// <returns>A <see cref="ProblemDetails" /> with status 415.</returns>
    private static ProblemDetails CreateProblemDetailsUnsupportedMediaType(Exception exception,
        IStringLocalizer localizer)
    {
        var problemDetails = new ProblemDetails
        {
            Title = localizer["ProblemDetailsUnsupportedMediaTypeTitle"],
            Detail = exception.Message,
            Type = "https://tools.ietf.org/html/rfc9110#name-415-unsupported-media-type",
            Status = StatusCodes.Status415UnsupportedMediaType
        };
        problemDetails.Extensions.TryAdd("code", localizer["ProblemDetailsUnsupportedMediaTypeCode"].Value);
        return problemDetails;
    }

    /// <summary>
    ///     Creates a <see cref="ProblemDetails" /> for an HTTP 422 Unprocessable Entity error.
    /// </summary>
    /// <param name="exception">The exception indicating the entity could not be processed.</param>
    /// <param name="localizer">The string localizer for retrieving localized messages.</param>
    /// <returns>A <see cref="ProblemDetails" /> with status 422.</returns>
    [SuppressMessage(
        "ReSharper",
        "ExceptionNotDocumentedOptional",
        Justification =
            "Potential exceptions originate from underlying implementation details and are not part of the method contract.")]
    internal static ProblemDetails CreateProblemDetailsUnprocessableEntity(Exception exception,
        IStringLocalizer<ResourceMain> localizer)
    {
        var problemDetails = new ProblemDetails
        {
            Title = localizer["ProblemDetailsUnprocessableEntityTitle"],
            Detail = exception.Message,
            Type = "https://tools.ietf.org/html/rfc9110#name-422-unprocessable-content",
            Status = StatusCodes.Status422UnprocessableEntity
        };
        problemDetails.Extensions.TryAdd("code", localizer["ProblemDetailsUnprocessableEntityCode"].Value);
        return problemDetails;
    }

    /// <summary>
    ///     Creates a <see cref="ProblemDetails" /> for an HTTP 429 Too Many Requests error.
    /// </summary>
    /// <param name="exception">The exception indicating rate limiting was triggered.</param>
    /// <param name="localizer">The string localizer for retrieving localized messages.</param>
    /// <returns>A <see cref="ProblemDetails" /> with status 429.</returns>
    [SuppressMessage(
        "ReSharper",
        "ExceptionNotDocumentedOptional",
        Justification =
            "Potential exceptions originate from underlying implementation details and are not part of the method contract.")]
    private static ProblemDetails CreateProblemDetailsTooManyRequest(Exception exception,
        IStringLocalizer localizer)
    {
        var problemDetails = new ProblemDetails
        {
            Title = localizer["ProblemDetailsTooManyRequestTitle"],
            Detail = exception.Message,
            Type = "https://tools.ietf.org/html/rfc6585#section-4",
            Status = StatusCodes.Status429TooManyRequests
        };
        problemDetails.Extensions.TryAdd("code", localizer["ProblemDetailsTooManyRequestCode"].Value);
        return problemDetails;
    }

    /// <summary>
    ///     Creates a <see cref="ProblemDetails" /> for an HTTP 500 Internal Server Error.
    /// </summary>
    /// <param name="exception">The exception that caused the internal server error.</param>
    /// <param name="localizer">The string localizer for retrieving localized messages.</param>
    /// <returns>A <see cref="ProblemDetails" /> with status 500.</returns>
    [SuppressMessage(
        "ReSharper",
        "ExceptionNotDocumentedOptional",
        Justification =
            "Potential exceptions originate from underlying implementation details and are not part of the method contract.")]
    internal static ProblemDetails CreateProblemDetailsInternalServerError(Exception exception,
        IStringLocalizer<ResourceMain> localizer)
    {
        var problemDetails = new ProblemDetails
        {
            Title = localizer["ProblemDetailsInternalServerErrorTitle"],
            Detail = exception.Message,
            Type = "https://tools.ietf.org/html/rfc9110#name-500-internal-server-error",
            Status = StatusCodes.Status500InternalServerError
        };
        problemDetails.Extensions.TryAdd("code", localizer["ProblemDetailsInternalServerErrorCode"].Value);
        return problemDetails;
    }

    /// <summary>
    ///     Creates a <see cref="ProblemDetails" /> for an HTTP 501 Not Implemented error.
    /// </summary>
    /// <param name="exception">The exception indicating the operation is not implemented.</param>
    /// <param name="localizer">The string localizer for retrieving localized messages.</param>
    /// <returns>A <see cref="ProblemDetails" /> with status 501.</returns>
    [SuppressMessage(
        "ReSharper",
        "ExceptionNotDocumentedOptional",
        Justification =
            "Potential exceptions originate from underlying implementation details and are not part of the method contract.")]
    internal static ProblemDetails CreateProblemDetailsNotImplemented(Exception exception,
        IStringLocalizer<ResourceMain> localizer)
    {
        var problemDetails = new ProblemDetails
        {
            Title = localizer["ProblemDetailsNotImplementedTitle"],
            Detail = exception.Message,
            Type = "https://tools.ietf.org/html/rfc9110#name-501-not-implemented",
            Status = StatusCodes.Status501NotImplemented
        };
        problemDetails.Extensions.TryAdd("code", localizer["ProblemDetailsNotImplementedCode"].Value);
        return problemDetails;
    }
}

/// <summary>
///     Provides utility methods used internally by <see cref="ProblemDetailsFactoryCore" />.
/// </summary>
file static class ProblemDetailsFactoryCoreUtils
{
    /// <summary>
    ///     Determines whether the validation failure originates from the root JSON document (indicated by a "$" key).
    /// </summary>
    /// <param name="keys">The collection of model state keys to inspect.</param>
    /// <returns>
    ///     <see langword="true" /> if the root "$" key is present, indicating a malformed JSON document; otherwise,
    ///     <see langword="false" />.
    /// </returns>
    [SuppressMessage(
        "ReSharper",
        "ExceptionNotDocumentedOptional",
        Justification =
            "Potential exceptions originate from underlying implementation details and are not part of the method contract.")]
    internal static bool IsRootFileFail(ModelStateDictionary.KeyEnumerable keys)
    {
        return keys.Contains("$");
    }
}