using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using template.net10.api.Core.Exceptions;
using template.net10.api.Core.Extensions;
using template.net10.api.Core.Factory;
using template.net10.api.Domain.Factory;
using template.net10.api.Localize.Resources;
using template.net10.api.Settings.Options;

namespace template.net10.api.Settings.Events;

/// <summary>
///     Custom <see cref="JwtBearerEvents" /> implementation that enriches the JWT authentication pipeline
///     with structured <see cref="Microsoft.AspNetCore.Mvc.ProblemDetails" /> responses for authentication failures,
///     missing tokens, forbidden access, and SignalR/dev token extraction.
/// </summary>
internal sealed class AppJwtBearerEvents(
    IOptions<JwtOptions> jwtConfig,
    IOptions<AppOptions> appConfig,
    IStringLocalizer<ResourceMain> localizer)
    : JwtBearerEvents
{
    /// <summary>
    ///     Application-level options used to detect the dev-token request header.
    /// </summary>
    private readonly AppOptions _appConfig = appConfig.Value ?? throw new ArgumentNullException(nameof(appConfig));

    /// <summary>
    ///     JWT configuration options used to validate token parameters and detect SignalR access tokens.
    /// </summary>
    private readonly JwtOptions _jwtConfig = jwtConfig.Value ?? throw new ArgumentNullException(nameof(jwtConfig));

    /// <summary>
    ///     String localizer used to produce localized error messages in problem details responses.
    /// </summary>
    private readonly IStringLocalizer<ResourceMain> _localizer =
        localizer ?? throw new ArgumentNullException(nameof(localizer));

    /// <summary>
    ///     Called when JWT token authentication fails. Stores a localized
    ///     <see cref="Microsoft.AspNetCore.Mvc.ProblemDetails" />
    ///     describing the failure in the HTTP context features for downstream middleware.
    /// </summary>
    /// <param name="context">Context carrying the authentication failure exception.</param>
    /// <exception cref="ArgumentNullException"><paramref name="context" /> is <see langword="null" />.</exception>
    public override Task AuthenticationFailed(AuthenticationFailedContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        var clientProblemDetails =
            ProblemDetailsFactoryCore.CreateProblemDetailsUnauthorizedProcessFail(context.Exception, _localizer);
        context.HttpContext.Features.Set(clientProblemDetails);
        return base.AuthenticationFailed(context);
    }

    /// <summary>
    ///     Called when a 401 challenge is issued. Sets the WWW-Authenticate <c>Bearer</c> error and description
    ///     from stored problem details and propagates them to the response.
    /// </summary>
    /// <param name="context">Context for the challenge operation.</param>
    /// <exception cref="ArgumentNullException"><paramref name="context" /> is <see langword="null" />.</exception>
    [SuppressMessage(
        "ReSharper",
        "ExceptionNotDocumentedOptional",
        Justification =
            "Potential exceptions originate from underlying implementation details and are not part of the method contract.")]
    public override Task Challenge(JwtBearerChallengeContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        var httpContextProblemDetails =
            context.HttpContext.Features.Get<ProblemDetails>();
        var bearerError = httpContextProblemDetails != null ? "invalid_token" : "invalid_request";
        var bearerErrorDescription = httpContextProblemDetails != null
            ? "The access token is not valid"
            : "The access token is missing";
        if (httpContextProblemDetails == null)
        {
            httpContextProblemDetails =
                ProblemDetailsFactoryCore.CreateProblemDetailsUnauthorizedMissingToken(_localizer);
            context.HttpContext.Features.Set(httpContextProblemDetails);
        }

        context.HttpContext.Items["BearerError"] = bearerError;
        context.HttpContext.Items["BearerErrorDescription"] = bearerErrorDescription;

        return base.Challenge(context);
    }

    /// <summary>
    ///     Called when a JWT token has been successfully validated. Invokes the base implementation without additional
    ///     processing.
    /// </summary>
    /// <param name="context">Context for the validated token.</param>
    /// <exception cref="ArgumentNullException"><paramref name="context" /> is <see langword="null" />.</exception>
    public override Task TokenValidated(TokenValidatedContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        return base.TokenValidated(context);
    }

    /// <summary>
    ///     Called when access is forbidden (403). Stores a localized forbidden
    ///     <see cref="Microsoft.AspNetCore.Mvc.ProblemDetails" />
    ///     in the HTTP context features.
    /// </summary>
    /// <param name="context">Context for the forbidden response.</param>
    /// <exception cref="ArgumentNullException"><paramref name="context" /> is <see langword="null" />.</exception>
    public override Task Forbidden(ForbiddenContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        var clientProblemDetails = ProblemDetailsFactoryCore.CreateProblemDetailsForbiddenAccess(_localizer);
        context.HttpContext.Features.Set(clientProblemDetails);
        return base.Forbidden(context);
    }

    /// <summary>
    ///     Called when a message is received. Attempts to extract the JWT token from a SignalR query string
    ///     or a developer-mode header, setting <c>context.Token</c> when found.
    /// </summary>
    /// <param name="context">Context for the incoming message.</param>
    /// <exception cref="ArgumentNullException"><paramref name="context" /> is <see langword="null" />.</exception>
    public override Task MessageReceived(MessageReceivedContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (TryExtractSignalRAccessToken(context, out var token) ||
            TryExtractDevToken(context, out token))
            context.Token = token;

        return Task.CompletedTask;
    }

    /// <summary>
    ///     Attempts to extract a JWT access token from the SignalR <c>access_token</c> query parameter
    ///     when the request targets a hub endpoint.
    /// </summary>
    /// <param name="context">The message received context providing access to the request.</param>
    /// <param name="token">
    ///     When this method returns <see langword="true" />, contains the extracted token; otherwise
    ///     <see langword="null" />.
    /// </param>
    /// <returns><see langword="true" /> if a SignalR token was found; otherwise <see langword="false" />.</returns>
    private static bool TryExtractSignalRAccessToken(MessageReceivedContext context, out string? token)
    {
        token = null;
        var accessToken = context.Request.Query["access_token"];
        var path = context.HttpContext.Request.Path;

        if (string.IsNullOrEmpty(accessToken)) return false;
        if (!path.StartsWithSegments(ApiRoutes.HubsAccess,
                StringComparison.InvariantCultureIgnoreCase)) return false;

        token = accessToken;
        return true;
    }

    /// <summary>
    ///     Returns <see langword="true" /> when the incoming request already carries a non-empty <c>Authorization</c> header.
    /// </summary>
    /// <param name="context">The message received context providing access to the request headers.</param>
    /// <returns><see langword="true" /> if an Authorization header is present; otherwise <see langword="false" />.</returns>
    private static bool HasAuthorizationHeader(MessageReceivedContext context)
    {
        return !string.IsNullOrEmpty(context.HttpContext.Request.Headers.Authorization);
    }

    /// <summary>
    ///     Returns <see langword="true" /> when the application is running in a local or test environment
    ///     where automatic developer token injection is permitted.
    /// </summary>
    /// <returns><see langword="true" /> for local/test environments; otherwise <see langword="false" />.</returns>
    private bool IsDevEnvironment()
    {
        return _appConfig.Env is Envs.Local or Envs.Test;
    }

    /// <summary>
    ///     Attempts to inject a developer-mode JWT token when no <c>Authorization</c> header is present
    ///     and the application is running in a local or test environment.
    /// </summary>
    /// <param name="context">The message received context providing access to the request.</param>
    /// <param name="token">
    ///     When this method returns <see langword="true" />, contains the generated dev token; otherwise
    ///     <see langword="null" />.
    /// </param>
    /// <returns><see langword="true" /> if a dev token was injected; otherwise <see langword="false" />.</returns>
    private bool TryExtractDevToken(MessageReceivedContext context, out string? token)
    {
        token = null;
        if (!IsDevEnvironment()) return false;
        if (HasAuthorizationHeader(context)) return false;

        var result = TokenFactory.GenerateGenieAccessToken(_jwtConfig, _appConfig).Try();
        if (result.IsFaulted)
            throw new InternalServerErrorException(
                _localizer["GenieAccessTokenServerError"],
                result.ExtractException());

        token = result.ExtractData();
        return true;
    }
}