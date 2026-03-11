using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using template.net10.api.Logger;

namespace template.net10.api.Communications;

/// <summary>
///     Abstract base class for HTTP web clients. Provides shared infrastructure such as
///     <see cref="IHttpClientFactory"/> and <see cref="ILogger"/> for derived client implementations.
/// </summary>
[SuppressMessage(
    "ReSharper",
    "UnusedType.Global",
    Justification = "Type is instantiated via dependency injection and/or reflection.")]
[SuppressMessage(
    "ReSharper",
    "NotAccessedField.Global",
    Justification = "Field is accessed by derived classes.")]
[SuppressMessage(
    "ReSharper",
    "MemberCanBePrivate.Global",
    Justification = "Members are intended to be accessed by derived classes.")]
[SuppressMessage(
    "ReSharper",
    "UnusedMember.Global",
    Justification = "Method is intended for reuse by derived web clients.")]
internal abstract class WebClientBase
{
    /// <summary>
    ///     Factory for creating <see cref="HttpClient"/> instances for outbound HTTP calls.
    /// </summary>
    protected readonly IHttpClientFactory HttpClientFactory;

    /// <summary>
    ///     Logger instance used for diagnostic and trace logging in derived web clients.
    /// </summary>
    protected readonly ILogger Logger;

    /// <summary>
    ///     Initializes a new instance of the <see cref="WebClientBase"/> class with the required dependencies.
    /// </summary>
    /// <param name="httpClientFactory">The HTTP client factory used to create <see cref="HttpClient"/> instances.</param>
    /// <param name="logger">The logger instance for diagnostic output.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="httpClientFactory"/> or <paramref name="logger"/> is <see langword="null"/>.</exception>
    protected WebClientBase(IHttpClientFactory httpClientFactory, ILogger<WebClientBase> logger)
    {
        HttpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        if (Logger.IsEnabled(LogLevel.Debug))
            Logger.LogWebClientBaseInjected(logger.GetType().ToString());
    }

    /// <summary>
    ///     Creates an <see cref="HttpRequestMessage"/> for the specified HTTP method and URI.
    /// </summary>
    /// <param name="method">The HTTP method (GET, POST, PUT, DELETE, etc.).</param>
    /// <param name="path">The target URI for the request.</param>
    /// <returns>A new <see cref="HttpRequestMessage"/> configured with the given method and URI.</returns>
    [MustDisposeResource]
    internal static HttpRequestMessage CreateRequest(HttpMethod method, Uri path)
    {
        return new HttpRequestMessage(method, path);
    }
}