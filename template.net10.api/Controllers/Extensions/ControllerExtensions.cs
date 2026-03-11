using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using FluentValidation;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using template.net10.api.Business.Exceptions;
using template.net10.api.Contracts.Interfaces;
using template.net10.api.Core.Exceptions;
using template.net10.api.Core.Factory;
using template.net10.api.Localize.Resources;

namespace template.net10.api.Controllers.Extensions;

/// <summary>
///     Provides extension methods and helper utilities for mapping operation results to HTTP action results.
/// </summary>
internal static class ControllerExtensions
{
    /// <summary>
    ///     Processes a successful result and maps it to the appropriate HTTP action result.
    /// </summary>
    /// <param name="controller">The controller handling the request.</param>
    /// <param name="action">The action result payload defining the response mapping.</param>
    /// <param name="obj">The result object to map.</param>
    /// <returns>An <see cref="IActionResult"/> representing the mapped HTTP response.</returns>
    private static IActionResult HandleSuccessResult<TResult, TContract>(
        ControllerBase controller,
        ActionResultPayload<TResult, TContract> action,
        TResult obj)
    {
        AddLinkHeaderIfNeeded(controller, action);

        if (action.Mapper is null)
            return HandleUnmappedAction(controller, action);

        var response = action.Mapper(obj);

        if (response is IFileContract fileResponse)
            return HandleFileContentResult(fileResponse);

        return HandleMappedAction(controller, action, response);
    }

    /// <summary>
    ///     Adds a Link header to the HTTP response if one is configured in the action payload.
    /// </summary>
    /// <param name="controller">The controller handling the request.</param>
    /// <param name="action">The action result payload containing the link header configuration.</param>
    private static void AddLinkHeaderIfNeeded<TResult, TContract>(ControllerBase controller,
        ActionResultPayload<TResult, TContract> action)
    {
        if (action.AddLinkHeader)
            controller.Response.Headers.TryAdd("Link", action.LinkHeader);
    }

    /// <summary>
    ///     Handles an action result when no response mapper is defined, routing to accepted or created results.
    /// </summary>
    /// <param name="controller">The controller handling the request.</param>
    /// <param name="action">The action result payload defining the response type.</param>
    /// <returns>An <see cref="IActionResult"/> for accepted or created responses without a mapped body.</returns>
    private static IActionResult HandleUnmappedAction<TResult, TContract>(ControllerBase controller,
        ActionResultPayload<TResult, TContract> action)
    {
        if (action.IsAcceptedAction)
            return HandleAcceptedContentResult(controller, action);

        return action.IsCreatedAction
            ? HandleCreatedAtActionResult(action)
            : throw new CoreException(
                "Error Creating the Http Action Result. The mapping action for endpoint is not defined");
    }

    /// <summary>
    ///     Handles an action result with a mapped response object, routing to the appropriate HTTP status result.
    /// </summary>
    /// <param name="controller">The controller handling the request.</param>
    /// <param name="action">The action result payload defining the response mapping.</param>
    /// <param name="response">The mapped response contract object.</param>
    /// <returns>An <see cref="IActionResult"/> for OK, created, or accepted responses with a mapped body.</returns>
    private static IActionResult HandleMappedAction<TResult, TContract>(ControllerBase controller,
        ActionResultPayload<TResult, TContract> action, TContract response)
    {
        if (action.IsAcceptedAction)
            return HandleAcceptedContentResult(controller, action, response);

        if (action.IsCreatedAction)
            return HandleCreatedAtActionResult(response, action);

        return HandleOkResult(controller, action, response);
    }

    /// <summary>
    ///     Maps an exception to the appropriate HTTP error action result with problem details.
    /// </summary>
    /// <param name="ex">The exception to handle.</param>
    /// <param name="localizer">The string localizer for error message translations.</param>
    /// <param name="features">The HTTP feature collection for storing problem details.</param>
    /// <returns>An <see cref="IActionResult"/> representing the error response.</returns>
    private static IActionResult ManageExceptionActionResult(Exception ex,
        IStringLocalizer<ResourceMain> localizer,
        IFeatureCollection features)
    {
        if (ex is CoreException or ValidationException)
            return ExceptionMapper.MapExceptionToResult(ex, localizer, features);

        //Exception not Controlled
        //Important: Only Write details for Business Exceptions
        var clientProblemDetails = ProblemDetailsFactoryCore.CreateProblemDetailsInternalServerError(ex, localizer);
        features.Set(clientProblemDetails);

        return new BadRequestResult();
    }

    /// <summary>
    ///     Creates an OK (200) result with the mapped response, optionally including a Location header.
    /// </summary>
    /// <param name="controller">The controller handling the request.</param>
    /// <param name="action">The action result payload defining response configuration.</param>
    /// <param name="response">The mapped response contract object.</param>
    /// <returns>An <see cref="OkObjectResult"/> containing the response.</returns>
    private static OkObjectResult HandleOkResult<TResult, TContract>(ControllerBase controller,
        ActionResultPayload<TResult, TContract> action, TContract response)
    {
        if (!action.AddLocationHeader)
            return new OkObjectResult(response);

        var dictionary = new RouteValueDictionary();
        var type = typeof(TContract);

        if (action.ActionParam != null)
        {
            var value = type.GetProperty(action.ActionParam.Value.Item1)?.GetValue(response);

            dictionary.Add(action.ActionParam.Value.Item2, value?.ToString());
        }

        var locationUrl = controller.Url.Action(
            action.ActionPath?.Item2,
            action.ActionPath?.Item1,
            dictionary,
            controller.Request.Scheme);

        controller.Response.Headers.TryAdd("Location", locationUrl);

        return new OkObjectResult(response);
    }

    /// <summary>
    ///     Creates a Created (201) result with the mapped response and optional location routing values.
    /// </summary>
    /// <param name="response">The mapped response contract object.</param>
    /// <param name="action">The action result payload defining the route and location configuration.</param>
    /// <returns>A <see cref="CreatedAtActionResult"/> containing the response and location.</returns>
    private static CreatedAtActionResult HandleCreatedAtActionResult<TResult, TContract>(TContract response,
        ActionResultPayload<TResult, TContract> action)
    {
        if (!action.AddLocationHeader)
            return new CreatedAtActionResult(null, null, null, response);

        var dictionary = new RouteValueDictionary();
        var type = typeof(TContract);
        if (action.ActionParam == null)
            return action.IsEmptyResponse
                ? new CreatedAtActionResult(action.ActionPath?.Item2, action.ActionPath?.Item1, dictionary, null)
                : new CreatedAtActionResult(action.ActionPath?.Item2, action.ActionPath?.Item1, dictionary, response);

        var value = type.GetProperty(action.ActionParam.Value.Item1)?.GetValue(response);

        dictionary.Add(action.ActionParam.Value.Item2, value?.ToString());

        return action.IsEmptyResponse
            ? new CreatedAtActionResult(action.ActionPath?.Item2, action.ActionPath?.Item1, dictionary, null)
            : new CreatedAtActionResult(action.ActionPath?.Item2, action.ActionPath?.Item1, dictionary, response);
    }

    /// <summary>
    ///     Creates a Created (201) result without a response body, using the action payload for location routing.
    /// </summary>
    /// <param name="action">The action result payload defining the route and location configuration.</param>
    /// <returns>A <see cref="CreatedAtActionResult"/> with location headers but no response body.</returns>
    private static CreatedAtActionResult HandleCreatedAtActionResult<TResult, TContract>(
        ActionResultPayload<TResult, TContract> action)
    {
        if (!action.AddLocationHeader)
            return new CreatedAtActionResult(null, null, null, null);

        RouteValueDictionary? dictionary = null;
        if (action.ActionParam != null)
            dictionary = new RouteValueDictionary
                { { action.ActionParam.Value.Item2, action.ActionParam.Value.Item1 } };
        return new CreatedAtActionResult(action.ActionPath?.Item2, action.ActionPath?.Item1, dictionary, null);
    }

    /// <summary>
    ///     Creates an Accepted (202) result without a response body, optionally including a Location header.
    /// </summary>
    /// <param name="controller">The controller handling the request.</param>
    /// <param name="action">The action result payload defining the location configuration.</param>
    /// <returns>An <see cref="AcceptedResult"/> with optional location header.</returns>
    private static AcceptedResult HandleAcceptedContentResult<TResult, TContract>(ControllerBase controller,
        ActionResultPayload<TResult, TContract> action)
    {
        if (!action.AddLocationHeader)
            return new AcceptedResult();

        RouteValueDictionary? dictionary = null;

        if (action.ActionParam != null)
            dictionary = new RouteValueDictionary
                { { action.ActionParam.Value.Item2, action.ActionParam.Value.Item1 } };

        var locationUrl = controller.Url.Action(
            action.ActionPath?.Item2,
            action.ActionPath?.Item1,
            dictionary,
            controller.Request.Scheme);

        return new AcceptedResult(locationUrl, null);
    }

    /// <summary>
    ///     Creates an Accepted (202) result with a response body, optionally including a Location header.
    /// </summary>
    /// <param name="controller">The controller handling the request.</param>
    /// <param name="action">The action result payload defining the location configuration.</param>
    /// <param name="response">The mapped response contract object.</param>
    /// <returns>An <see cref="AcceptedResult"/> with the response and optional location header.</returns>
    private static AcceptedResult HandleAcceptedContentResult<TResult, TContract>(ControllerBase controller,
        ActionResultPayload<TResult, TContract> action, TContract response)
    {
        if (!action.AddLocationHeader)
            return new AcceptedResult((string?)null, response);

        var dictionary = new RouteValueDictionary();
        var type = typeof(TContract);

        if (action.ActionParam != null)
        {
            var value = type.GetProperty(action.ActionParam.Value.Item1)?.GetValue(response);

            dictionary.Add(action.ActionParam.Value.Item2, value?.ToString());
        }

        var locationUrl = controller.Url.Action(
            action.ActionPath?.Item2,
            action.ActionPath?.Item1,
            dictionary,
            controller.Request.Scheme);

        return action.IsEmptyResponse
            ? new AcceptedResult(locationUrl, null)
            : new AcceptedResult(locationUrl, response);
    }

    /// <summary>
    ///     Creates a file download result from a file contract response.
    /// </summary>
    /// <param name="response">The file contract containing the file data, content type, and file name.</param>
    /// <returns>A <see cref="FileContentResult"/> for file download.</returns>
    private static FileContentResult HandleFileContentResult(IFileContract response)
    {
        return new FileContentResult([..response.Data], response.ContentType)
        {
            FileDownloadName = response.FileName
        };
    }

    extension<TResult>(LanguageExt.Common.Result<TResult> result)
    {
        /// <summary>
        ///     Converts a <see cref="LanguageExt.Common.Result{A}"/> to an appropriate <see cref="IActionResult"/>.
        /// </summary>
        /// <param name="controller">The controller handling the request.</param>
        /// <param name="action">The action result payload defining the response mapping.</param>
        /// <param name="localizer">The string localizer for error message translations.</param>
        /// <returns>An <see cref="IActionResult"/> representing success or error.</returns>
        internal IActionResult ToActionResult<TContract>(MyControllerBase controller,
            ActionResultPayload<TResult, TContract> action, IStringLocalizer<ResourceMain> localizer)
        {
            return result.Match(obj => HandleSuccessResult(controller, action, obj),
                ex => ManageExceptionActionResult(ex, localizer, controller.HttpContext.Features));
        }
    }
}

/// <summary>
///     Immutable payload record that defines how an operation result is mapped to an HTTP action result,
///     including status code, location headers, and response body mapping.
/// </summary>
/// <typeparam name="TResult">The type of the operation result.</typeparam>
/// <typeparam name="TContract">The type of the response contract.</typeparam>
[SuppressMessage(
    "ReSharper",
    "UnusedMember.Global",
    Justification =
        "General-purpose HTTP response builders. Not all response types are required in every API scenario.")]
internal sealed record ActionResultPayload<TResult, TContract> : IEqualityOperators<
    ActionResultPayload<TResult, TContract>, ActionResultPayload<TResult, TContract>, bool>
{
    /// <summary>
    ///     Indicates whether a Location header should be added to the HTTP response.
    /// </summary>
    internal bool AddLocationHeader { get; private init; }

    /// <summary>
    ///     Indicates whether a Link header should be added to the HTTP response.
    /// </summary>
    internal bool AddLinkHeader { get; private init; }

    /// <summary>
    ///     Indicates whether the response body should be empty.
    /// </summary>
    internal bool IsEmptyResponse { get; private init; }

    /// <summary>
    ///     Indicates whether the action should produce an HTTP 202 Accepted response.
    /// </summary>
    internal bool IsAcceptedAction { get; private init; }

    /// <summary>
    ///     Indicates whether the action should produce an HTTP 201 Created response.
    /// </summary>
    internal bool IsCreatedAction { get; private init; }

    /// <summary>
    ///     The value of the Link header to include in the HTTP response, or <c>null</c> if not applicable.
    /// </summary>
    internal string? LinkHeader { get; private init; }

    /// <summary>
    ///     The property name and route parameter name tuple used to build location URLs, or <c>null</c> if not applicable.
    /// </summary>
    internal (string, string)? ActionParam { get; private init; }

    /// <summary>
    ///     The controller name and action name tuple used to build location URLs, or <c>null</c> if not applicable.
    /// </summary>
    internal (string, string)? ActionPath { get; private init; }

    /// <summary>
    ///     The mapping function that transforms the operation result into the response contract, or <c>null</c> if no mapping is required.
    /// </summary>
    internal Func<TResult, TContract>? Mapper { get; private init; }

    /// <summary>
    ///     Creates an action result payload configured for an HTTP 200 OK response.
    /// </summary>
    /// <param name="mapper">The function to map the result to the response contract.</param>
    /// <param name="linkHeader">An optional Link header value to include in the response.</param>
    /// <returns>A configured <see cref="ActionResultPayload{TResult, TContract}"/> for an OK response.</returns>
    internal static ActionResultPayload<TResult, TContract> Ok(
        Func<TResult, TContract> mapper,
        string? linkHeader = null)
    {
        return new ActionResultPayload<TResult, TContract>
        {
            Mapper = mapper,
            AddLinkHeader = linkHeader is not null,
            LinkHeader = linkHeader
        };
    }

    /// <summary>
    ///     Creates an action result payload configured for an HTTP 200 OK response with a Location header.
    /// </summary>
    /// <param name="actionPath">The controller and action names for building the location URL.</param>
    /// <param name="actionParam">The property and route parameter names for location URL values.</param>
    /// <param name="mapper">The function to map the result to the response contract.</param>
    /// <param name="linkHeader">An optional Link header value to include in the response.</param>
    /// <returns>A configured <see cref="ActionResultPayload{TResult, TContract}"/> for an OK response with location.</returns>
    internal static ActionResultPayload<TResult, TContract> OkWithLocation(
        (string ControllerName, string ActionName) actionPath,
        (string PropertyName, string RouteParamName) actionParam,
        Func<TResult, TContract> mapper,
        string? linkHeader = null)
    {
        return new ActionResultPayload<TResult, TContract>
        {
            Mapper = mapper,
            AddLocationHeader = true,
            ActionParam = actionParam,
            ActionPath = actionPath,
            AddLinkHeader = linkHeader is not null,
            LinkHeader = linkHeader
        };
    }

    /// <summary>
    ///     Creates an action result payload configured for a file download response.
    /// </summary>
    /// <param name="mapper">The function to map the result to the file contract.</param>
    /// <param name="linkHeader">An optional Link header value to include in the response.</param>
    /// <typeparam name="TRes">The type of the operation result.</typeparam>
    /// <typeparam name="TFile">The type of the file contract.</typeparam>
    /// <returns>A configured <see cref="ActionResultPayload{TRes, TFile}"/> for a file download response.</returns>
    internal static ActionResultPayload<TRes, TFile> File<TRes, TFile>(
        Func<TRes, TFile> mapper,
        string? linkHeader = null) where TFile : IFileContract
    {
        return new ActionResultPayload<TRes, TFile>
        {
            Mapper = mapper,
            AddLinkHeader = linkHeader is not null,
            LinkHeader = linkHeader
        };
    }

    /// <summary>
    ///     Creates an action result payload configured for an HTTP 201 Created response.
    /// </summary>
    /// <param name="mapper">The function to map the result to the response contract.</param>
    /// <param name="linkHeader">An optional Link header value to include in the response.</param>
    /// <returns>A configured <see cref="ActionResultPayload{TResult, TContract}"/> for a Created response.</returns>
    internal static ActionResultPayload<TResult, TContract> Created(
        Func<TResult, TContract> mapper,
        string? linkHeader = null)
    {
        return new ActionResultPayload<TResult, TContract>
        {
            Mapper = mapper,
            IsCreatedAction = true,
            AddLinkHeader = linkHeader is not null,
            LinkHeader = linkHeader
        };
    }

    /// <summary>
    ///     Creates an action result payload configured for an HTTP 201 Created response with a Location header.
    /// </summary>
    /// <param name="actionPath">The controller and action names for building the location URL.</param>
    /// <param name="actionParam">The property and route parameter names for location URL values.</param>
    /// <param name="mapper">The function to map the result to the response contract.</param>
    /// <param name="linkHeader">An optional Link header value to include in the response.</param>
    /// <returns>A configured <see cref="ActionResultPayload{TResult, TContract}"/> for a Created response with location.</returns>
    internal static ActionResultPayload<TResult, TContract> CreatedWithLocation(
        (string ControllerName, string ActionName) actionPath,
        (string PropertyName, string RouteParamName) actionParam,
        Func<TResult, TContract> mapper,
        string? linkHeader = null)
    {
        return new ActionResultPayload<TResult, TContract>
        {
            Mapper = mapper,
            IsCreatedAction = true,
            AddLocationHeader = true,
            ActionParam = actionParam,
            ActionPath = actionPath,
            AddLinkHeader = linkHeader is not null,
            LinkHeader = linkHeader
        };
    }

    /// <summary>
    ///     Creates an action result payload configured for an HTTP 201 Created response with no response body.
    /// </summary>
    /// <param name="linkHeader">An optional Link header value to include in the response.</param>
    /// <returns>A configured <see cref="ActionResultPayload{TResult, TContract}"/> for an empty Created response.</returns>
    internal static ActionResultPayload<TResult, TContract> CreatedEmpty(
        string? linkHeader = null)
    {
        return new ActionResultPayload<TResult, TContract>
        {
            IsCreatedAction = true,
            IsEmptyResponse = true,
            AddLinkHeader = linkHeader is not null,
            LinkHeader = linkHeader
        };
    }

    /// <summary>
    ///     Creates an action result payload for an HTTP 201 Created response with no body and a Location header.
    /// </summary>
    /// <param name="actionPath">The controller and action names for building the location URL.</param>
    /// <param name="actionParam">The property and route parameter names for location URL values.</param>
    /// <param name="linkHeader">An optional Link header value to include in the response.</param>
    /// <returns>A configured <see cref="ActionResultPayload{TResult, TContract}"/> for an empty Created response with location.</returns>
    internal static ActionResultPayload<TResult, TContract> CreatedEmptyWithLocation(
        (string ControllerName, string ActionName) actionPath,
        (string PropertyName, string RouteParamName) actionParam,
        string? linkHeader = null)
    {
        return new ActionResultPayload<TResult, TContract>
        {
            IsCreatedAction = true,
            IsEmptyResponse = true,
            AddLocationHeader = true,
            ActionParam = actionParam,
            ActionPath = actionPath,
            AddLinkHeader = linkHeader is not null,
            LinkHeader = linkHeader
        };
    }

    /// <summary>
    ///     Creates an action result payload for an HTTP 201 Created response with a mapper, empty body flag, and a Location header.
    /// </summary>
    /// <param name="actionPath">The controller and action names for building the location URL.</param>
    /// <param name="actionParam">The property and route parameter names for location URL values.</param>
    /// <param name="mapper">The function to map the result to the response contract.</param>
    /// <param name="linkHeader">An optional Link header value to include in the response.</param>
    /// <returns>A configured <see cref="ActionResultPayload{TResult, TContract}"/> for an empty Created response with location and mapper.</returns>
    internal static ActionResultPayload<TResult, TContract> CreatedEmptyWithLocation(
        (string ControllerName, string ActionName) actionPath,
        (string PropertyName, string RouteParamName) actionParam,
        Func<TResult, TContract> mapper,
        string? linkHeader = null)
    {
        return new ActionResultPayload<TResult, TContract>
        {
            Mapper = mapper,
            IsCreatedAction = true,
            IsEmptyResponse = true,
            AddLocationHeader = true,
            ActionParam = actionParam,
            ActionPath = actionPath,
            AddLinkHeader = linkHeader is not null,
            LinkHeader = linkHeader
        };
    }

    /// <summary>
    ///     Creates an action result payload configured for an HTTP 202 Accepted response.
    /// </summary>
    /// <param name="mapper">The function to map the result to the response contract.</param>
    /// <param name="linkHeader">An optional Link header value to include in the response.</param>
    /// <returns>A configured <see cref="ActionResultPayload{TResult, TContract}"/> for an Accepted response.</returns>
    internal static ActionResultPayload<TResult, TContract> Accepted(
        Func<TResult, TContract> mapper,
        string? linkHeader = null)
    {
        return new ActionResultPayload<TResult, TContract>
        {
            Mapper = mapper,
            IsAcceptedAction = true,
            AddLinkHeader = linkHeader is not null,
            LinkHeader = linkHeader
        };
    }

    /// <summary>
    ///     Creates an action result payload configured for an HTTP 202 Accepted response with a Location header.
    /// </summary>
    /// <param name="actionPath">The controller and action names for building the location URL.</param>
    /// <param name="actionParam">The property and route parameter names for location URL values.</param>
    /// <param name="mapper">The function to map the result to the response contract.</param>
    /// <param name="linkHeader">An optional Link header value to include in the response.</param>
    /// <returns>A configured <see cref="ActionResultPayload{TResult, TContract}"/> for an Accepted response with location.</returns>
    internal static ActionResultPayload<TResult, TContract> AcceptedWithLocation(
        (string ControllerName, string ActionName) actionPath,
        (string PropertyName, string RouteParamName) actionParam,
        Func<TResult, TContract> mapper,
        string? linkHeader = null)
    {
        return new ActionResultPayload<TResult, TContract>
        {
            Mapper = mapper,
            IsAcceptedAction = true,
            AddLocationHeader = true,
            ActionParam = actionParam,
            ActionPath = actionPath,
            AddLinkHeader = linkHeader is not null,
            LinkHeader = linkHeader
        };
    }

    /// <summary>
    ///     Creates an action result payload configured for an HTTP 202 Accepted response with no response body.
    /// </summary>
    /// <param name="linkHeader">An optional Link header value to include in the response.</param>
    /// <returns>A configured <see cref="ActionResultPayload{TResult, TContract}"/> for an empty Accepted response.</returns>
    internal static ActionResultPayload<TResult, TContract> AcceptedEmpty(
        string? linkHeader = null)
    {
        return new ActionResultPayload<TResult, TContract>
        {
            IsAcceptedAction = true,
            IsEmptyResponse = true,
            AddLinkHeader = linkHeader is not null,
            LinkHeader = linkHeader
        };
    }

    /// <summary>
    ///     Creates an action result payload for an HTTP 202 Accepted response with no body and a Location header.
    /// </summary>
    /// <param name="actionPath">The controller and action names for building the location URL.</param>
    /// <param name="actionParam">The property and route parameter names for location URL values.</param>
    /// <param name="linkHeader">An optional Link header value to include in the response.</param>
    /// <returns>A configured <see cref="ActionResultPayload{TResult, TContract}"/> for an empty Accepted response with location.</returns>
    internal static ActionResultPayload<TResult, TContract> AcceptedEmptyWithLocation(
        (string ControllerName, string ActionName) actionPath,
        (string PropertyName, string RouteParamName) actionParam,
        string? linkHeader = null)
    {
        return new ActionResultPayload<TResult, TContract>
        {
            IsAcceptedAction = true,
            IsEmptyResponse = true,
            AddLocationHeader = true,
            ActionParam = actionParam,
            ActionPath = actionPath,
            AddLinkHeader = linkHeader is not null,
            LinkHeader = linkHeader
        };
    }

    /// <summary>
    ///     Creates an action result payload for an HTTP 202 Accepted response with a mapper, empty body flag, and a Location header.
    /// </summary>
    /// <param name="actionPath">The controller and action names for building the location URL.</param>
    /// <param name="actionParam">The property and route parameter names for location URL values.</param>
    /// <param name="mapper">The function to map the result to the response contract.</param>
    /// <param name="linkHeader">An optional Link header value to include in the response.</param>
    /// <returns>A configured <see cref="ActionResultPayload{TResult, TContract}"/> for an empty Accepted response with location and mapper.</returns>
    internal static ActionResultPayload<TResult, TContract> AcceptedEmptyWithLocation(
        (string ControllerName, string ActionName) actionPath,
        (string PropertyName, string RouteParamName) actionParam,
        Func<TResult, TContract> mapper,
        string? linkHeader = null)
    {
        return new ActionResultPayload<TResult, TContract>
        {
            Mapper = mapper,
            IsAcceptedAction = true,
            IsEmptyResponse = true,
            AddLocationHeader = true,
            ActionParam = actionParam,
            ActionPath = actionPath,
            AddLinkHeader = linkHeader is not null,
            LinkHeader = linkHeader
        };
    }
}