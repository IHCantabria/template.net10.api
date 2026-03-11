using System.Diagnostics.CodeAnalysis;
using System.Net.Mime;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Timeouts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Annotations;
using template.net10.api.Business;
using template.net10.api.Contracts;
using template.net10.api.Controllers.Extensions;
using template.net10.api.Core.Contracts;
using template.net10.api.Core.Exceptions;
using template.net10.api.Core.Timeout;
using template.net10.api.Domain.DTOs;
using template.net10.api.Domain.Extensions;
using template.net10.api.Features.Commands;
using template.net10.api.Features.Querys;
using template.net10.api.Hubs.User.Utils;
using template.net10.api.Localize.Resources;
using template.net10.api.Persistence.Models;
using template.net10.api.Settings.Options;

namespace template.net10.api.Controllers;

/// <summary>
///     API controller managing user-related operations including CRUD, enable/disable, and password reset.
/// </summary>
[SuppressMessage("Design",
    "CA1515:Consider making public types internal",
    Justification =
        "Controllers must remain public to allow OpenAPI discovery and correct API exposure.")]
[SwaggerTag(SwaggerDocumentation.Users.ControllerDescription)]
[Authorize(Policy = PoliciesConstants.ApiAccessPolicy)]
[Route(ApiRoutes.UsersController.PathController)]
[ApiController]
public sealed class Users(
    IMediator mediator,
    IStringLocalizer<ResourceMain> localizer,
    ILogger<Users> logger)
    : MyControllerBase(mediator, localizer, logger)
{
    /// <summary>
    ///     Route parameter key used for user location headers.
    /// </summary>
    private const string UserKey = "user-key";

    /// <summary>
    ///     Creates a new user in the system.
    /// </summary>
    /// <param name="commandParams">The user creation parameters.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>An <see cref="IActionResult"/> containing the created user with a 201 Created response.</returns>
    /// <exception cref="ResultSuccessInvalidOperationException">
    ///     Result is not a success! Use ExtractException method instead
    ///     and Check the state of Result with IsSuccess or IsFaulted before use this method or ExtractException method
    /// </exception>
    [HttpPost]
    [Authorize(Policy = PoliciesConstants.UserCreationPolicy)]
    [RequestTimeout(RequestConstants.RequestCommandGenericPolicy)]
    [Route(ApiRoutes.UsersController.CreateUser)]
    [Consumes(MediaTypeNames.Application.Json)]
    [SwaggerOperation(
        Summary = SwaggerDocumentation.Users.CreateUser.Summary,
        Description =
            SwaggerDocumentation.Users.CreateUser.Description,
        OperationId = SwaggerDocumentation.Users.CreateUser.Id
    )]
    [SwaggerResponse(StatusCodes.Status201Created, SwaggerDocumentation.Users.CreateUser.Created,
        typeof(UserCreatedResource), MediaTypeNames.Application.Json)]
    [SwaggerResponse(StatusCodes.Status400BadRequest,
        SwaggerDocumentation.Users.CreateUser.BadRequest,
        typeof(BadRequestProblemDetailsResource), MediaTypeNames.Application.ProblemJson)]
    [SwaggerResponse(StatusCodes.Status404NotFound,
        SwaggerDocumentation.Users.CreateUser.NotFound,
        typeof(NotFoundProblemDetailsResource), MediaTypeNames.Application.ProblemJson)]
    [SwaggerResponse(StatusCodes.Status409Conflict,
        SwaggerDocumentation.Users.CreateUser.Conflict,
        typeof(ConflictProblemDetailsResource), MediaTypeNames.Application.ProblemJson)]
    [SwaggerResponse(StatusCodes.Status422UnprocessableEntity,
        SwaggerDocumentation.Users.CreateUser.UnprocessableEntity,
        typeof(UnprocessableEntityProblemDetailsResource), MediaTypeNames.Application.ProblemJson)]
    public async Task<IActionResult> CreateUserAsync(CommandCreateUserParamsResource commandParams,
        CancellationToken cancellationToken)
    {
        CommandCreateUserParamsDto paramsDto = commandParams;
        var query = new CommandCreateUser(paramsDto.AddIdentifier(User));
        var result = await Mediator.Send(query, cancellationToken).ConfigureAwait(false);
        var action =
            ActionResultPayload<User, UserCreatedResource>.CreatedWithLocation(
                (nameof(Users), nameof(GetUserAsync)), ("Uuid", UserKey), static obj => obj);
        return result.ToActionResult(this, action, Localizer);
    }

    /// <summary>
    ///     Updates an existing user in the system.
    /// </summary>
    /// <param name="commandParams">The user update parameters.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>An <see cref="IActionResult"/> containing the updated user state.</returns>
    /// <exception cref="ResultSuccessInvalidOperationException">
    ///     Result is not a success! Use ExtractException method instead
    ///     and Check the state of Result with IsSuccess or IsFaulted before use this method or ExtractException method
    /// </exception>
    [HttpPut]
    [Authorize(Policy = PoliciesConstants.UserUpdatePolicy)]
    [RequestTimeout(RequestConstants.RequestCommandGenericPolicy)]
    [Route(ApiRoutes.UsersController.UpdateUser)]
    [Consumes(MediaTypeNames.Application.Json)]
    [SwaggerOperation(
        Summary = SwaggerDocumentation.Users.UpdateUser.Summary,
        Description =
            SwaggerDocumentation.Users.UpdateUser.Description,
        OperationId = SwaggerDocumentation.Users.UpdateUser.Id
    )]
    [SwaggerResponse(StatusCodes.Status200OK, SwaggerDocumentation.Users.UpdateUser.Ok,
        typeof(UserStateResource), MediaTypeNames.Application.Json)]
    [SwaggerResponse(StatusCodes.Status400BadRequest,
        SwaggerDocumentation.Users.UpdateUser.BadRequest,
        typeof(BadRequestProblemDetailsResource), MediaTypeNames.Application.ProblemJson)]
    [SwaggerResponse(StatusCodes.Status404NotFound,
        SwaggerDocumentation.Users.UpdateUser.NotFound,
        typeof(NotFoundProblemDetailsResource), MediaTypeNames.Application.ProblemJson)]
    [SwaggerResponse(StatusCodes.Status409Conflict,
        SwaggerDocumentation.Users.UpdateUser.Conflict,
        typeof(ConflictProblemDetailsResource), MediaTypeNames.Application.ProblemJson)]
    [SwaggerResponse(StatusCodes.Status422UnprocessableEntity,
        SwaggerDocumentation.Users.UpdateUser.UnprocessableEntity,
        typeof(UnprocessableEntityProblemDetailsResource), MediaTypeNames.Application.ProblemJson)]
    public async Task<IActionResult> UpdateUserAsync(CommandUpdateUserParamsResource commandParams,
        CancellationToken cancellationToken)
    {
        CommandUpdateUserParamsDto paramsDto = commandParams;
        var query = new CommandUpdateUser(paramsDto.AddIdentifier(User));
        var result = await Mediator.Send(query, cancellationToken).ConfigureAwait(false);
        var action =
            ActionResultPayload<User, UserStateResource>.OkWithLocation((nameof(Users), nameof(GetUserAsync)),
                ("Uuid", UserKey), static obj => obj);
        return result.ToActionResult(this, action, Localizer);
    }

    /// <summary>
    ///     Deletes an existing user from the system.
    /// </summary>
    /// <param name="commandParams">The user deletion parameters from the route.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>An <see cref="IActionResult"/> containing the deleted user state.</returns>
    /// <exception cref="ResultSuccessInvalidOperationException">
    ///     Result is not a success! Use ExtractException method instead
    ///     and Check the state of Result with IsSuccess or IsFaulted before use this method or ExtractException method
    /// </exception>
    [HttpDelete]
    [Authorize(Policy = PoliciesConstants.UserDeletePolicy)]
    [RequestTimeout(RequestConstants.RequestCommandGenericPolicy)]
    [Route(ApiRoutes.UsersController.DeleteUser)]
    [Consumes(MediaTypeNames.Application.Json)]
    [SwaggerOperation(
        Summary = SwaggerDocumentation.Users.DeleteUser.Summary,
        Description =
            SwaggerDocumentation.Users.DeleteUser.Description,
        OperationId = SwaggerDocumentation.Users.DeleteUser.Id
    )]
    [SwaggerResponse(StatusCodes.Status200OK, SwaggerDocumentation.Users.DeleteUser.Ok,
        typeof(UserResource), MediaTypeNames.Application.Json)]
    [SwaggerResponse(StatusCodes.Status400BadRequest,
        SwaggerDocumentation.Users.DeleteUser.BadRequest,
        typeof(BadRequestProblemDetailsResource), MediaTypeNames.Application.ProblemJson)]
    [SwaggerResponse(StatusCodes.Status404NotFound,
        SwaggerDocumentation.Users.DeleteUser.NotFound,
        typeof(NotFoundProblemDetailsResource), MediaTypeNames.Application.ProblemJson)]
    [SwaggerResponse(StatusCodes.Status409Conflict,
        SwaggerDocumentation.Users.DeleteUser.Conflict,
        typeof(ConflictProblemDetailsResource), MediaTypeNames.Application.ProblemJson)]
    [SwaggerResponse(StatusCodes.Status422UnprocessableEntity,
        SwaggerDocumentation.Users.DeleteUser.UnprocessableEntity,
        typeof(UnprocessableEntityProblemDetailsResource), MediaTypeNames.Application.ProblemJson)]
    public async Task<IActionResult> DeleteUserAsync([FromRoute] CommandDeleteUserParamsResource commandParams,
        CancellationToken cancellationToken)
    {
        CommandDeleteUserParamsDto paramsDto = commandParams;
        var command = new CommandDeleteUser(paramsDto.AddIdentifier(User));
        var result = await Mediator.Send(command, cancellationToken).ConfigureAwait(false);
        var action = ActionResultPayload<User, UserStateResource>.Ok(static obj => obj);
        return result.ToActionResult(this, action, Localizer);
    }

    /// <summary>
    ///     Disables an existing user in the system.
    /// </summary>
    /// <param name="commandParams">The user disable parameters from the route.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>An <see cref="IActionResult"/> containing the disabled user state.</returns>
    /// <exception cref="ResultSuccessInvalidOperationException">
    ///     Result is not a success! Use ExtractException method instead
    ///     and Check the state of Result with IsSuccess or IsFaulted before use this method or ExtractException method
    /// </exception>
    [HttpPut]
    [Authorize(Policy = PoliciesConstants.UserDisablePolicy)]
    [RequestTimeout(RequestConstants.RequestCommandGenericPolicy)]
    [Route(ApiRoutes.UsersController.DisableUser)]
    [SwaggerOperation(
        Summary = SwaggerDocumentation.Users.DisableUser.Summary,
        Description =
            SwaggerDocumentation.Users.DisableUser.Description,
        OperationId = SwaggerDocumentation.Users.DisableUser.Id
    )]
    [SwaggerResponse(StatusCodes.Status200OK, SwaggerDocumentation.Users.DisableUser.Ok,
        typeof(UserResource), MediaTypeNames.Application.Json)]
    [SwaggerResponse(StatusCodes.Status400BadRequest,
        SwaggerDocumentation.Users.DisableUser.BadRequest,
        typeof(BadRequestProblemDetailsResource), MediaTypeNames.Application.ProblemJson)]
    [SwaggerResponse(StatusCodes.Status404NotFound,
        SwaggerDocumentation.Users.DisableUser.NotFound,
        typeof(NotFoundProblemDetailsResource), MediaTypeNames.Application.ProblemJson)]
    [SwaggerResponse(StatusCodes.Status409Conflict,
        SwaggerDocumentation.Users.DisableUser.Conflict,
        typeof(ConflictProblemDetailsResource), MediaTypeNames.Application.ProblemJson)]
    [SwaggerResponse(StatusCodes.Status422UnprocessableEntity,
        SwaggerDocumentation.Users.DisableUser.UnprocessableEntity,
        typeof(UnprocessableEntityProblemDetailsResource), MediaTypeNames.Application.ProblemJson)]
    public async Task<IActionResult> DisableUserAsync([FromRoute] CommandDisableUserParamsResource commandParams,
        CancellationToken cancellationToken)
    {
        CommandDisableUserParamsDto paramsDto = commandParams;
        var command = new CommandDisableUser(paramsDto.AddIdentifier(User));
        var result = await Mediator.Send(command, cancellationToken).ConfigureAwait(false);
        var action =
            ActionResultPayload<User, UserStateResource>.OkWithLocation((nameof(Users), nameof(GetUserAsync)),
                ("Uuid", UserKey), static obj => obj);
        return result.ToActionResult(this, action, Localizer);
    }

    /// <summary>
    ///     Enables a previously disabled user in the system.
    /// </summary>
    /// <param name="commandParams">The user enable parameters from the route.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>An <see cref="IActionResult"/> containing the enabled user state.</returns>
    /// <exception cref="ResultSuccessInvalidOperationException">
    ///     Result is not a success! Use ExtractException method instead
    ///     and Check the state of Result with IsSuccess or IsFaulted before use this method or ExtractException method
    /// </exception>
    [HttpPut]
    [Authorize(Policy = PoliciesConstants.UserEnablePolicy)]
    [RequestTimeout(RequestConstants.RequestCommandGenericPolicy)]
    [Route(ApiRoutes.UsersController.EnableUser)]
    [SwaggerOperation(
        Summary = SwaggerDocumentation.Users.EnableUser.Summary,
        Description =
            SwaggerDocumentation.Users.EnableUser.Description,
        OperationId = SwaggerDocumentation.Users.EnableUser.Id
    )]
    [SwaggerResponse(StatusCodes.Status200OK, SwaggerDocumentation.Users.EnableUser.Ok,
        typeof(UserResource), MediaTypeNames.Application.Json)]
    [SwaggerResponse(StatusCodes.Status400BadRequest,
        SwaggerDocumentation.Users.EnableUser.BadRequest,
        typeof(BadRequestProblemDetailsResource), MediaTypeNames.Application.ProblemJson)]
    [SwaggerResponse(StatusCodes.Status404NotFound,
        SwaggerDocumentation.Users.EnableUser.NotFound,
        typeof(NotFoundProblemDetailsResource), MediaTypeNames.Application.ProblemJson)]
    [SwaggerResponse(StatusCodes.Status409Conflict,
        SwaggerDocumentation.Users.EnableUser.Conflict,
        typeof(ConflictProblemDetailsResource), MediaTypeNames.Application.ProblemJson)]
    [SwaggerResponse(StatusCodes.Status422UnprocessableEntity,
        SwaggerDocumentation.Users.EnableUser.UnprocessableEntity,
        typeof(UnprocessableEntityProblemDetailsResource), MediaTypeNames.Application.ProblemJson)]
    public async Task<IActionResult> EnableUserAsync([FromRoute] CommandEnableUserParamsResource commandParams,
        CancellationToken cancellationToken)
    {
        CommandEnableUserParamsDto paramsDto = commandParams;
        var command = new CommandEnableUser(paramsDto.AddIdentifier(User));
        var result = await Mediator.Send(command, cancellationToken).ConfigureAwait(false);
        var action =
            ActionResultPayload<User, UserStateResource>.OkWithLocation((nameof(Users), nameof(GetUserAsync)),
                ("Uuid", UserKey), static obj => obj);
        return result.ToActionResult(this, action, Localizer);
    }

    /// <summary>
    ///     Resets the password for an existing user in the system.
    /// </summary>
    /// <param name="commandParams">The password reset parameters.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>An <see cref="IActionResult"/> containing the new user credentials.</returns>
    /// <exception cref="ResultSuccessInvalidOperationException">
    ///     Result is not a success! Use ExtractException method instead
    ///     and Check the state of Result with IsSuccess or IsFaulted before use this method or ExtractException method
    /// </exception>
    [HttpPut]
    [Authorize(Policy = PoliciesConstants.UserResetPasswordPolicy)]
    [RequestTimeout(RequestConstants.RequestCommandGenericPolicy)]
    [Route(ApiRoutes.UsersController.ResetUserPassword)]
    [Consumes(MediaTypeNames.Application.Json)]
    [SwaggerOperation(
        Summary = SwaggerDocumentation.Users.ResetUserPassword.Summary,
        Description =
            SwaggerDocumentation.Users.ResetUserPassword.Description,
        OperationId = SwaggerDocumentation.Users.ResetUserPassword.Id
    )]
    [SwaggerResponse(StatusCodes.Status200OK, SwaggerDocumentation.Users.ResetUserPassword.Ok,
        typeof(UserResetedPasswordResource), MediaTypeNames.Application.Json)]
    [SwaggerResponse(StatusCodes.Status400BadRequest,
        SwaggerDocumentation.Users.ResetUserPassword.BadRequest,
        typeof(BadRequestProblemDetailsResource), MediaTypeNames.Application.ProblemJson)]
    [SwaggerResponse(StatusCodes.Status404NotFound,
        SwaggerDocumentation.Users.ResetUserPassword.NotFound,
        typeof(NotFoundProblemDetailsResource), MediaTypeNames.Application.ProblemJson)]
    [SwaggerResponse(StatusCodes.Status422UnprocessableEntity,
        SwaggerDocumentation.Users.ResetUserPassword.UnprocessableEntity,
        typeof(UnprocessableEntityProblemDetailsResource), MediaTypeNames.Application.ProblemJson)]
    public async Task<IActionResult> ResetUserPasswordAsync(CommandResetUserPasswordParamsResource commandParams,
        CancellationToken cancellationToken)
    {
        CommandResetUserPasswordParamsDto paramsDto = commandParams;
        var query = new CommandResetUserPassword(paramsDto.AddIdentifier(User));
        var result = await Mediator.Send(query, cancellationToken).ConfigureAwait(false);
        var action = ActionResultPayload<UserResetedPasswordDto, UserResetedPasswordResource>.OkWithLocation(
            (nameof(Users), nameof(GetUserAsync)),
            ("Uuid", UserKey), static obj => obj);
        return result.ToActionResult(this, action, Localizer);
    }

    /// <summary>
    ///     Retrieves all users available in the system.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>An <see cref="IActionResult"/> containing the collection of users.</returns>
    [SuppressMessage(
        "ReSharper",
        "ExceptionNotDocumentedOptional",
        Justification =
            "Potential exceptions originate from underlying implementation details and are not part of the method contract.")]
    [HttpGet]
    [Authorize(Policy = PoliciesConstants.UserReadPolicy)]
    [RequestTimeout(RequestConstants.RequestQueryGenericPolicy)]
    [Route(ApiRoutes.UsersController.GetUsers)]
    [SwaggerOperation(
        Summary = SwaggerDocumentation.Users.GetUsers.Summary,
        Description =
            SwaggerDocumentation.Users.GetUsers.Description,
        OperationId = SwaggerDocumentation.Users.GetUsers.Id
    )]
    [SwaggerResponse(StatusCodes.Status200OK, SwaggerDocumentation.Users.GetUsers.Ok,
        typeof(IEnumerable<UserResource>), MediaTypeNames.Application.Json)]
    public async Task<IActionResult> GetUsersAsync(CancellationToken cancellationToken)
    {
        var query = new QueryGetUsers();
        var result = await Mediator.Send(query, cancellationToken).ConfigureAwait(false);
        var action =
            ActionResultPayload<IEnumerable<UserDto>, IEnumerable<UserResource>>.Ok(static obj =>
                UserDto.ToCollection([..obj]));
        return result.ToActionResult(this, action, Localizer);
    }

    /// <summary>
    ///     Retrieves a specific user by their unique identifier.
    /// </summary>
    /// <param name="queryParams">The query parameters containing the user key.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>An <see cref="IActionResult"/> containing the user data.</returns>
    [HttpGet]
    [Authorize(Policy = PoliciesConstants.UserReadPolicy)]
    [RequestTimeout(RequestConstants.RequestQueryGenericPolicy)]
    [Route(ApiRoutes.UsersController.GetUser)]
    [SwaggerOperation(
        Summary = SwaggerDocumentation.Users.GetUser.Summary,
        Description =
            SwaggerDocumentation.Users.GetUser.Description,
        OperationId = SwaggerDocumentation.Users.GetUser.Id
    )]
    [SwaggerResponse(StatusCodes.Status200OK, SwaggerDocumentation.Users.GetUser.Ok,
        typeof(UserResource), MediaTypeNames.Application.Json)]
    [SwaggerResponse(StatusCodes.Status400BadRequest,
        SwaggerDocumentation.Users.GetUser.BadRequest,
        typeof(BadRequestProblemDetailsResource), MediaTypeNames.Application.ProblemJson)]
    [SwaggerResponse(StatusCodes.Status404NotFound,
        SwaggerDocumentation.Users.GetUser.NotFound,
        typeof(NotFoundProblemDetailsResource), MediaTypeNames.Application.ProblemJson)]
    public async Task<IActionResult> GetUserAsync([FromRoute] QueryGetUserParamsResource queryParams,
        CancellationToken cancellationToken)
    {
        var query = new QueryGetUser(queryParams);
        var result = await Mediator.Send(query, cancellationToken).ConfigureAwait(false);
        var action = ActionResultPayload<UserDto, UserResource>.Ok(static obj => obj);
        return result.ToActionResult(this, action, Localizer);
    }

    /// <summary>
    ///     Retrieves the list of available user hub events and their SignalR connection details.
    /// </summary>
    /// <param name="config">The Swagger options containing the server URL configuration.</param>
    /// <returns>An <see cref="IActionResult"/> containing the collection of hub events.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="config" /> is <see langword="null" />.</exception>
    [HttpGet]
    [AllowAnonymous]
    [RequestTimeout(RequestConstants.RequestQueryGenericPolicy)]
    [Route(ApiRoutes.UsersController.Hubs)]
    [SwaggerOperation(
        Summary = SwaggerDocumentation.Users.GetUserEvents.Summary,
        Description =
            SwaggerDocumentation.Users.GetUserEvents.Description,
        OperationId = SwaggerDocumentation.Users.GetUserEvents.Id
    )]
    [SwaggerResponse(StatusCodes.Status200OK, SwaggerDocumentation.Users.GetUserEvents.Ok,
        typeof(IEnumerable<HubEventResource>), MediaTypeNames.Application.Json)]
    public Task<IActionResult> GetUserEventsAsync(IOptions<SwaggerOptions> config)
    {
        ArgumentNullException.ThrowIfNull(config);
        var fullUrl = config.Value.ServerUrl.OriginalString + ApiRoutes.UsersHub.PathHub;
        var eventList = HubUserEvents.GetEvents(Localizer, fullUrl);
        var result = new LanguageExt.Common.Result<IEnumerable<HubEventResource>>(eventList);
        var action =
            ActionResultPayload<IEnumerable<HubEventResource>, IEnumerable<HubEventResource>>.Ok(static obj => obj);
        return Task.FromResult(result.ToActionResult(this, action, Localizer));
    }
}