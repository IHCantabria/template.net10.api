using System.Net;
using FluentValidation;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using template.net10.api.Business.Extensions;
using template.net10.api.Core.Factory;
using template.net10.api.Localize.Resources;

namespace template.net10.api.Business.Factory;

/// <summary>
///     Factory class that creates HTTP result objects from exceptions by wrapping them in ProblemDetails responses.
/// </summary>
internal static class HttpResultFactory
{
    /// <summary>
    ///     Creates an HTTP result for a <see cref="ValidationException"/> by determining the appropriate status code.
    /// </summary>
    /// <param name="vex">The validation exception to process.</param>
    /// <param name="localizer">The string localizer for resolving localized error messages.</param>
    /// <param name="features">The HTTP feature collection for the current request.</param>
    /// <returns>A <see cref="BadRequestResult"/> with the corresponding ProblemDetails set in the feature collection.</returns>
    internal static BadRequestResult CreateValidationResult(
        ValidationException vex, IStringLocalizer<ResourceMain> localizer, IFeatureCollection features)
    {
        var httpStatusCode = HttpResultUtils.GetStatusCode(vex);
        return ManageValidationResultCreation(httpStatusCode, vex, localizer, features);
    }

    /// <summary>
    ///     Routes the validation result creation to the appropriate handler based on the resolved HTTP status code.
    /// </summary>
    /// <param name="httpStatusCode">The HTTP status code resolved from the validation errors.</param>
    /// <param name="vex">The validation exception containing the errors.</param>
    /// <param name="localizer">The string localizer for resolving localized error messages.</param>
    /// <param name="features">The HTTP feature collection for the current request.</param>
    /// <returns>A <see cref="BadRequestResult"/> with the corresponding ProblemDetails set in the feature collection.</returns>
    private static BadRequestResult ManageValidationResultCreation(HttpStatusCode httpStatusCode,
        ValidationException vex,
        IStringLocalizer<ResourceMain> localizer, IFeatureCollection features)
    {
        return httpStatusCode switch
        {
            HttpStatusCode.BadRequest or HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden
                or HttpStatusCode.NotFound or HttpStatusCode.RequestTimeout or HttpStatusCode.Conflict
                or HttpStatusCode.Gone or HttpStatusCode.UnprocessableEntity or HttpStatusCode.InternalServerError
                or HttpStatusCode.NotImplemented => CreateValidationResult(httpStatusCode, vex, localizer, features),
            _ => throw new NotSupportedException(localizer["MapperExceptionStatusCodeNotSupported"])
        };
    }

    /// <summary>
    ///     Creates an HTTP 400 Bad Request result from the specified exception.
    /// </summary>
    /// <param name="exception">The exception that caused the bad request.</param>
    /// <param name="localizer">The string localizer for resolving localized error messages.</param>
    /// <param name="features">The HTTP feature collection for the current request.</param>
    /// <returns>A <see cref="BadRequestResult"/> with ProblemDetails set in the feature collection.</returns>
    internal static BadRequestResult CreateBadRequestResult(Exception exception,
        IStringLocalizer<ResourceMain> localizer,
        IFeatureCollection features)
    {
        var clientProblemDetails = ProblemDetailsFactoryCore.CreateProblemDetailsBadRequest(exception, localizer);
        features.Set(clientProblemDetails);
        return new BadRequestResult();
    }

    /// <summary>
    ///     Creates a validation HTTP result with the specified status code, populating ProblemDetails with validation errors.
    /// </summary>
    /// <param name="statusCode">The HTTP status code to associate with the result.</param>
    /// <param name="exception">The validation exception containing the errors.</param>
    /// <param name="localizer">The string localizer for resolving localized error messages.</param>
    /// <param name="features">The HTTP feature collection for the current request.</param>
    /// <returns>A <see cref="BadRequestResult"/> with ProblemDetails and validation errors set in the feature collection.</returns>
    private static BadRequestResult CreateValidationResult(HttpStatusCode statusCode, ValidationException exception,
        IStringLocalizer<ResourceMain> localizer, IFeatureCollection features)
    {
        var clientProblemDetails =
            ProblemDetailsFactoryCore.CreateProblemDetailsByHttpStatusCode(statusCode, exception, localizer);
        clientProblemDetails.AddErrors(localizer, exception);
        features.Set(clientProblemDetails);
        return new BadRequestResult();
    }

    /// <summary>
    ///     Creates an HTTP 401 Unauthorized result from the specified exception.
    /// </summary>
    /// <param name="exception">The exception that caused the unauthorized response.</param>
    /// <param name="localizer">The string localizer for resolving localized error messages.</param>
    /// <param name="features">The HTTP feature collection for the current request.</param>
    /// <returns>A <see cref="BadRequestResult"/> with ProblemDetails set in the feature collection.</returns>
    internal static BadRequestResult CreateUnauthorizedResult(Exception exception,
        IStringLocalizer<ResourceMain> localizer,
        IFeatureCollection features)
    {
        var clientProblemDetails = ProblemDetailsFactoryCore.CreateProblemDetailsUnauthorized(exception, localizer);
        features.Set(clientProblemDetails);
        return new BadRequestResult();
    }

    /// <summary>
    ///     Creates an HTTP 403 Forbidden result from the specified exception.
    /// </summary>
    /// <param name="exception">The exception that caused the forbidden response.</param>
    /// <param name="localizer">The string localizer for resolving localized error messages.</param>
    /// <param name="features">The HTTP feature collection for the current request.</param>
    /// <returns>A <see cref="BadRequestResult"/> with ProblemDetails set in the feature collection.</returns>
    internal static BadRequestResult CreateForbiddenResult(Exception exception,
        IStringLocalizer<ResourceMain> localizer,
        IFeatureCollection features)
    {
        var clientProblemDetails = ProblemDetailsFactoryCore.CreateProblemDetailsForbidden(exception, localizer);
        features.Set(clientProblemDetails);
        return new BadRequestResult();
    }

    /// <summary>
    ///     Creates an HTTP 404 Not Found result from the specified exception.
    /// </summary>
    /// <param name="exception">The exception that caused the not found response.</param>
    /// <param name="localizer">The string localizer for resolving localized error messages.</param>
    /// <param name="features">The HTTP feature collection for the current request.</param>
    /// <returns>A <see cref="BadRequestResult"/> with ProblemDetails set in the feature collection.</returns>
    internal static BadRequestResult CreateNotFoundResult(Exception exception, IStringLocalizer<ResourceMain> localizer,
        IFeatureCollection features)
    {
        var clientProblemDetails = ProblemDetailsFactoryCore.CreateProblemDetailsNotFound(exception, localizer);
        features.Set(clientProblemDetails);
        return new BadRequestResult();
    }

    /// <summary>
    ///     Creates an HTTP 408 Request Timeout result from the specified exception.
    /// </summary>
    /// <param name="exception">The exception that caused the request timeout response.</param>
    /// <param name="localizer">The string localizer for resolving localized error messages.</param>
    /// <param name="features">The HTTP feature collection for the current request.</param>
    /// <returns>A <see cref="BadRequestResult"/> with ProblemDetails set in the feature collection.</returns>
    internal static BadRequestResult CreateRequestTimeoutResult(Exception exception,
        IStringLocalizer<ResourceMain> localizer, IFeatureCollection features)
    {
        var clientProblemDetails = ProblemDetailsFactoryCore.CreateProblemDetailsRequestTimeout(exception, localizer);
        features.Set(clientProblemDetails);
        return new BadRequestResult();
    }

    /// <summary>
    ///     Creates an HTTP 409 Conflict result from the specified exception.
    /// </summary>
    /// <param name="exception">The exception that caused the conflict response.</param>
    /// <param name="localizer">The string localizer for resolving localized error messages.</param>
    /// <param name="features">The HTTP feature collection for the current request.</param>
    /// <returns>A <see cref="BadRequestResult"/> with ProblemDetails set in the feature collection.</returns>
    internal static BadRequestResult CreateConflictResult(Exception exception, IStringLocalizer<ResourceMain> localizer,
        IFeatureCollection features)
    {
        var clientProblemDetails = ProblemDetailsFactoryCore.CreateProblemDetailsConflict(exception, localizer);
        features.Set(clientProblemDetails);
        return new BadRequestResult();
    }

    /// <summary>
    ///     Creates an HTTP 410 Gone result from the specified exception.
    /// </summary>
    /// <param name="exception">The exception that caused the gone response.</param>
    /// <param name="localizer">The string localizer for resolving localized error messages.</param>
    /// <param name="features">The HTTP feature collection for the current request.</param>
    /// <returns>A <see cref="BadRequestResult"/> with ProblemDetails set in the feature collection.</returns>
    internal static BadRequestResult CreateGoneResult(Exception exception, IStringLocalizer<ResourceMain> localizer,
        IFeatureCollection features)
    {
        var clientProblemDetails = ProblemDetailsFactoryCore.CreateProblemDetailsGone(exception, localizer);
        features.Set(clientProblemDetails);
        return new BadRequestResult();
    }

    /// <summary>
    ///     Creates an HTTP 422 Unprocessable Entity result from the specified exception.
    /// </summary>
    /// <param name="exception">The exception that caused the unprocessable entity response.</param>
    /// <param name="localizer">The string localizer for resolving localized error messages.</param>
    /// <param name="features">The HTTP feature collection for the current request.</param>
    /// <returns>A <see cref="BadRequestResult"/> with ProblemDetails set in the feature collection.</returns>
    internal static BadRequestResult CreateUnprocessableEntityResult(Exception exception,
        IStringLocalizer<ResourceMain> localizer,
        IFeatureCollection features)
    {
        var clientProblemDetails =
            ProblemDetailsFactoryCore.CreateProblemDetailsUnprocessableEntity(exception, localizer);
        features.Set(clientProblemDetails);
        return new BadRequestResult();
    }

    /// <summary>
    ///     Creates an HTTP 500 Internal Server Error result from the specified exception.
    /// </summary>
    /// <param name="exception">The exception that caused the internal server error response.</param>
    /// <param name="localizer">The string localizer for resolving localized error messages.</param>
    /// <param name="features">The HTTP feature collection for the current request.</param>
    /// <returns>A <see cref="BadRequestResult"/> with ProblemDetails set in the feature collection.</returns>
    internal static BadRequestResult CreateInternalServerErrorResult(Exception exception,
        IStringLocalizer<ResourceMain> localizer,
        IFeatureCollection features)
    {
        var clientProblemDetails =
            ProblemDetailsFactoryCore.CreateProblemDetailsInternalServerError(exception, localizer);
        features.Set(clientProblemDetails);
        return new BadRequestResult();
    }

    /// <summary>
    ///     Creates an HTTP 501 Not Implemented result from the specified exception.
    /// </summary>
    /// <param name="exception">The exception that caused the not implemented response.</param>
    /// <param name="localizer">The string localizer for resolving localized error messages.</param>
    /// <param name="features">The HTTP feature collection for the current request.</param>
    /// <returns>A <see cref="BadRequestResult"/> with ProblemDetails set in the feature collection.</returns>
    internal static BadRequestResult CreateNotImplementedResult(Exception exception,
        IStringLocalizer<ResourceMain> localizer,
        IFeatureCollection features)
    {
        var clientProblemDetails =
            ProblemDetailsFactoryCore.CreateProblemDetailsNotImplemented(exception, localizer);
        features.Set(clientProblemDetails);
        return new BadRequestResult();
    }
}