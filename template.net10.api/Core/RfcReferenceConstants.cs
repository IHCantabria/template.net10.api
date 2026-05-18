namespace template.net10.api.Core;

/// <summary>
///     Contains constants for RFC references used across the application, providing standardized links to relevant RFC
///     documentation for HTTP status codes and other protocol-related information.
/// </summary>
internal static class RfcReferenceConstants
{
    /// <summary>
    ///     Bad Request (400) RFC reference link, pointing to the relevant section in RFC 9110 that defines the semantics of
    ///     the 400 Bad Request status code.
    /// </summary>
    internal const string BadRequest = "https://tools.ietf.org/html/rfc9110#name-400-bad-request";

    /// <summary>
    ///     Unauthorized (401) RFC reference link, pointing to the relevant section in RFC 9110 that defines the semantics of
    ///     the 401 Unauthorized status code.
    /// </summary>
    internal const string Unauthorized = "https://tools.ietf.org/html/rfc9110#name-401-unauthorized";

    /// <summary>
    ///     Forbidden (403) RFC reference link, pointing to the relevant section in RFC 9110 that defines the semantics of the
    ///     403 Forbidden status code.
    /// </summary>
    internal const string Forbidden = "https://tools.ietf.org/html/rfc9110#name-403-forbidden";

    /// <summary>
    ///     Not Found (404) RFC reference link, pointing to the relevant section in RFC 9110 that defines the semantics of the
    ///     404 Not Found status code.
    /// </summary>
    internal const string NotFound = "https://tools.ietf.org/html/rfc9110#name-404-not-found";

    /// <summary>
    ///     Method Not Allowed (405) RFC reference link, pointing to the relevant section in RFC 9110 that defines the
    ///     semantics of the 405 Method Not Allowed status code.
    /// </summary>
    internal const string MethodNotAllowed =
        "https://tools.ietf.org/html/rfc9110#name-405-method-not-allowed";

    /// <summary>
    ///     Request Timeout (408) RFC reference link, pointing to the relevant section in RFC 9110 that defines the semantics
    ///     of the 408 Request Timeout status code.
    /// </summary>
    internal const string RequestTimeout = "https://tools.ietf.org/html/rfc9110#name-408-request-timeout";

    /// <summary>
    ///     Conflict (409) RFC reference link, pointing to the relevant section in RFC 9110 that defines the semantics of the
    ///     409 Conflict status code.
    /// </summary>
    internal const string Conflict = "https://tools.ietf.org/html/rfc9110#name-409-conflict";

    /// <summary>
    ///     Gone (410) RFC reference link, pointing to the relevant section in RFC 9110 that defines the semantics of the 410
    ///     Gone status code.
    /// </summary>
    internal const string Gone = "https://tools.ietf.org/html/rfc9110#name-410-gone";

    /// <summary>
    ///     Content Too Large (413) RFC reference link, pointing to the relevant section in RFC 9110 that defines the
    ///     semantics of the 413 Content Too Large status code.
    /// </summary>
    internal const string RequestEntityTooLarge =
        "https://tools.ietf.org/html/rfc9110#name-413-content-too-large";

    /// <summary>
    ///     Unsupported Media Type (415) RFC reference link, pointing to the relevant section in RFC 9110 that defines the
    ///     semantics of the 415 Unsupported Media Type status code.
    /// </summary>
    internal const string UnsupportedMediaType =
        "https://tools.ietf.org/html/rfc9110#name-415-unsupported-media-type";

    /// <summary>
    ///     Unprocessable Entity (422) RFC reference link, pointing to the relevant section in RFC 9110 that defines the
    ///     semantics of the 422 Unprocessable Entity status code.
    /// </summary>
    internal const string UnprocessableEntity = "https://tools.ietf.org/html/rfc9110#name-422-unprocessable-content";

    /// <summary>
    ///     Too Many Requests (429) RFC reference link, pointing to the relevant section in RFC 6585 that defines the semantics
    ///     of the 429 Too Many Requests status code.
    /// </summary>
    internal const string TooManyRequests = "https://tools.ietf.org/html/rfc6585#section-4";

    /// <summary>
    ///     Internal Server Error (500) RFC reference link, pointing to the relevant section in RFC 9110 that defines the
    ///     semantics of the 500 Internal Server Error status code.
    /// </summary>
    internal const string InternalServerError = "https://tools.ietf.org/html/rfc9110#name-500-internal-server-error";

    /// <summary>
    ///     Not Implemented (501) RFC reference link, pointing to the relevant section in RFC 9110 that defines the semantics
    ///     of the 501 Not Implemented status code.
    /// </summary>
    internal const string NotImplemented = "https://tools.ietf.org/html/rfc9110#name-501-not-implemented";
}