using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Numerics;
using System.Text.Json.Serialization;
using FluentValidation;
using FluentValidation.Results;

namespace template.net10.api.Business.Factory;

/// <summary>
///     Provides utility methods for processing validation exceptions and building problem details error collections.
/// </summary>
internal static class HttpResultUtils
{
    /// <summary>
    ///     Creates a collection of <see cref="ProblemDetailsValidationError" /> from the specified validation exception.
    /// </summary>
    /// <param name="validationException">The validation exception containing the errors to convert.</param>
    /// <returns>A list of <see cref="ProblemDetailsValidationError" /> representing the validation failures.</returns>
    internal static List<ProblemDetailsValidationError> CreateErrorsCollection(ValidationException validationException)
    {
        var groupedErrors = GroupErrors(validationException.Errors);
        return ConvertToDictionary(groupedErrors);
    }

    /// <summary>
    ///     Determines the appropriate <see cref="HttpStatusCode" /> for the given validation exception.
    ///     Returns the single status code if all errors share the same code; otherwise returns
    ///     <see cref="HttpStatusCode.UnprocessableEntity" />.
    /// </summary>
    /// <param name="validationException">The validation exception to evaluate.</param>
    /// <returns>The resolved <see cref="HttpStatusCode" /> for the validation errors.</returns>
    [SuppressMessage(
        "ReSharper",
        "ExceptionNotDocumentedOptional",
        Justification =
            "Potential exceptions originate from underlying implementation details and are not part of the method contract.")]
    internal static HttpStatusCode GetStatusCode(ValidationException validationException)
    {
        var errorGroups = GroupErrors(validationException.Errors);
        var errorCodes = GetHttpStatusCodes(errorGroups).ToList();

        return errorCodes.Count == 1 ? errorCodes[0] : HttpStatusCode.UnprocessableEntity;
    }

    /// <summary>
    ///     Groups validation failures by their property name.
    /// </summary>
    /// <param name="errors">The validation failures to group.</param>
    /// <returns>An enumerable of grouped validation failures keyed by property name.</returns>
    private static IEnumerable<IGrouping<string, ValidationFailure>> GroupErrors(IEnumerable<ValidationFailure> errors)
    {
        return errors.GroupBy(static error => error.PropertyName);
    }

    /// <summary>
    ///     Extracts distinct HTTP status codes from the custom state of grouped validation failures.
    /// </summary>
    /// <param name="groups">The grouped validation failures to extract status codes from.</param>
    /// <returns>An enumerable of distinct <see cref="HttpStatusCode" /> values found in the validation failures.</returns>
    private static IEnumerable<HttpStatusCode> GetHttpStatusCodes(
        IEnumerable<IGrouping<string, ValidationFailure>> groups)
    {
        return groups.SelectMany(static g => g.Select(static vf => vf.CustomState).OfType<HttpStatusCode>()).Distinct();
    }

    /// <summary>
    ///     Converts grouped validation failures into a flat list of <see cref="ProblemDetailsValidationError" /> instances.
    /// </summary>
    /// <param name="groupedErrors">The grouped validation failures to convert.</param>
    /// <returns>A list of <see cref="ProblemDetailsValidationError" /> representing all validation errors.</returns>
    private static List<ProblemDetailsValidationError> ConvertToDictionary(
        IEnumerable<IGrouping<string, ValidationFailure>> groupedErrors)
    {
        var errorsList = new List<ProblemDetailsValidationError>();
        //Must be Serial
        foreach (var errors in groupedErrors.Select(static group => group.Select(error =>
                     new ProblemDetailsValidationError
                     {
                         Detail = error.ErrorMessage, Code = error.ErrorCode,
                         Value = error.AttemptedValue?.ToString(), Pointer = group.Key
                     })))
            errorsList.AddRange(errors);

        return errorsList;
    }
}

/// <summary>
///     Represents a single validation error entry in a ProblemDetails response, following RFC 9457 conventions.
/// </summary>
internal sealed record
    ProblemDetailsValidationError : IEqualityOperators<ProblemDetailsValidationError, ProblemDetailsValidationError,
    bool>
{
    /// <summary>
    ///     Gets the human-readable description of the validation error.
    /// </summary>
    [JsonRequired]
    public required string Detail { get; init; }

    /// <summary>
    ///     Gets the property name or JSON pointer identifying the source of the error.
    /// </summary>
    [JsonRequired]
    public required string Pointer { get; init; }

    /// <summary>
    ///     Gets the attempted value that caused the validation error, or <c>null</c> if unavailable.
    /// </summary>
    [JsonRequired]
    public required string? Value { get; init; }

    /// <summary>
    ///     Gets the machine-readable error code identifying the type of validation failure.
    /// </summary>
    [JsonRequired]
    public required string Code { get; init; }
}