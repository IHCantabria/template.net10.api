using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using template.net10.api.Business.Factory;
using template.net10.api.Localize.Resources;

namespace template.net10.api.Business.Extensions;

/// <summary>
///     Provides extension methods for <see cref="ProblemDetails" /> to enrich error responses with validation details.
/// </summary>
internal static class ProblemDetailsExtensions
{
    extension(ProblemDetails problemDetails)
    {
        /// <summary>
        ///     Populates the <see cref="ProblemDetails" /> instance with validation error information extracted from a
        ///     <see cref="ValidationException" />. For a single error, sets the detail, value, pointer, and code directly.
        ///     For multiple errors, sets a generic title and attaches the full error collection.
        /// </summary>
        /// <param name="localizer">The string localizer used to resolve localized fallback messages.</param>
        /// <param name="vex">The validation exception containing the errors to populate into the problem details.</param>
        [SuppressMessage(
            "ReSharper",
            "ExceptionNotDocumentedOptional",
            Justification =
                "Potential exceptions originate from underlying implementation details and are not part of the method contract.")]
        internal void AddErrors(IStringLocalizer<ResourceMain> localizer,
            ValidationException vex)
        {
            var errorsCollection = HttpResultUtils.CreateErrorsCollection(vex);
            problemDetails.Detail = localizer["ProblemDetailsValidationDetail"];
            if (errorsCollection.Count is 1)
            {
                var singleError = errorsCollection[0];
                problemDetails.Detail = singleError.Detail;
                problemDetails.Extensions["value"] = singleError.Value;
                problemDetails.Extensions["pointer"] = singleError.Pointer;
                problemDetails.Extensions["code"] = singleError.Code;
            }
            else
            {
                problemDetails.Title = localizer["ProblemDetailsValidationTitle"];
                problemDetails.Extensions["errors"] = errorsCollection;
                problemDetails.Extensions["code"] = localizer["ProblemDetailsValidationCode"];
            }
        }
    }
}