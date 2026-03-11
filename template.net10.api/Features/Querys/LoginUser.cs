using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Numerics;
using FluentValidation;
using JetBrains.Annotations;
using MediatR;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using template.net10.api.Core.Exceptions;
using template.net10.api.Core.Extensions;
using template.net10.api.Domain.DTOs;
using template.net10.api.Domain.Factory;
using template.net10.api.Domain.Password;
using template.net10.api.Domain.Specifications;
using template.net10.api.Localize.Resources;
using template.net10.api.Persistence.Context;
using template.net10.api.Persistence.Models;
using template.net10.api.Persistence.Repositories.Interfaces;
using template.net10.api.Settings.Options;

namespace template.net10.api.Features.Querys;

/// <summary>
///     Represents a MediatR query request to authenticate a user by email and password and generate an identity token.
/// </summary>
[SuppressMessage(
    "Design",
    "CA1515:Consider making public types internal",
    Justification =
        "Public visibility is required because this request is part of the application messaging contract (MediatR).")]
[SuppressMessage(
    "Design",
    "MemberCanBeInternal",
    Justification =
        "Public visibility is required because this request is part of the application messaging contract (MediatR).")]
public sealed record QueryLoginUser(QueryLoginUserParamsDto QueryParams)
    : IRequest<LanguageExt.Common.Result<IdTokenDto>>, IEqualityOperators<QueryLoginUser, QueryLoginUser, bool>;

/// <summary>
///     Handles the <see cref="QueryLoginUser"/> request by authenticating the user and generating an identity token (JWT).
/// </summary>
internal sealed class QueryLoginUserHandler(
    IGenericDbRepositoryReadContext<AppDbContext, User> repository,
    IOptions<JwtOptions> jwtConfig,
    IOptions<AppOptions> appConfig)
    : IRequestHandler<QueryLoginUser, LanguageExt.Common.Result<IdTokenDto>>
{
    /// <summary>
    ///     Application configuration options used for token generation.
    /// </summary>
    private readonly AppOptions _appConfig = appConfig.Value ?? throw new ArgumentNullException(nameof(appConfig));

    /// <summary>
    ///     JWT configuration options including signing key, issuer, and audience.
    /// </summary>
    private readonly JwtOptions _jwtConfig = jwtConfig.Value ?? throw new ArgumentNullException(nameof(jwtConfig));

    /// <summary>
    ///     Read-only repository for querying <see cref="User"/> entities.
    /// </summary>
    private readonly IGenericDbRepositoryReadContext<AppDbContext, User> _repository =
        repository ?? throw new ArgumentNullException(nameof(repository));


    /// <summary>
    ///     Handles the login request by authenticating the user via email and returning a signed identity token.
    /// </summary>
    /// <param name="request">The MediatR query request containing the user credentials for login.</param>
    /// <param name="cancellationToken">A token to observe for cancellation of the asynchronous operation.</param>
    /// <exception cref="ResultFaultedInvalidOperationException">
    ///     Result is not a failure! Use ExtractData method instead and
    ///     Check the state of Result with IsSuccess or IsFaulted before use this method or ExtractData method
    /// </exception>
    /// <exception cref="ResultSuccessInvalidOperationException">
    ///     Result is not a success! Use ExtractException method instead
    ///     and Check the state of Result with IsSuccess or IsFaulted before use this method or ExtractException method
    /// </exception>
    public async Task<LanguageExt.Common.Result<IdTokenDto>> Handle(QueryLoginUser request,
        CancellationToken cancellationToken)
    {
        var specification = new UserReadByEmailSpecification(request.QueryParams.Email);
        var resultUser = await _repository
            .GetFirstAsync(specification, UserProjections.UserIdTokenProjection, cancellationToken)
            .ConfigureAwait(false);
        return resultUser.IsSuccess
            ? TokenFactory.GenerateIdTokenDto(resultUser.ExtractData(), _jwtConfig, _appConfig)
            : new LanguageExt.Common.Result<IdTokenDto>(resultUser.ExtractException());
    }
}

/// <summary>
///     FluentValidation validator that verifies user credentials (email, active status, and password) during login.
/// </summary>
[UsedImplicitly]
internal sealed class LoginUserEmailValidator : AbstractValidator<QueryLoginUser>
{
    /// <summary>
    ///     Password configuration options including the pepper used for credential verification.
    /// </summary>
    private readonly PasswordOptions _config;

    /// <summary>
    ///     Localization service for retrieving validation error messages.
    /// </summary>
    private readonly IStringLocalizer<ResourceMain> _localizer;

    /// <summary>
    ///     Read-only repository for querying <see cref="User"/> entities during validation.
    /// </summary>
    private readonly IGenericDbRepositoryReadContext<AppDbContext, User> _repository;

    /// <summary>
    ///     Initializes a new instance of the <see cref="LoginUserEmailValidator"/> class with email, password, and active status validation rules.
    /// </summary>
    /// <param name="repository">The read-only repository used to query user data during validation.</param>
    /// <param name="config">The password configuration options containing the pepper for credential verification.</param>
    /// <param name="localizer">The string localizer for retrieving validation error messages.</param>
    /// <exception cref="ArgumentNullException">
    ///     <paramref name="repository" /> is <see langword="null" />.
    ///     -or-
    ///     <paramref name="config" /> is <see langword="null" />.
    ///     -or-
    ///     <paramref name="localizer" /> is <see langword="null" />.
    /// </exception>
    public LoginUserEmailValidator(
        IGenericDbRepositoryReadContext<AppDbContext, User> repository, IOptions<PasswordOptions> config,
        IStringLocalizer<ResourceMain> localizer)
    {
        ArgumentNullException.ThrowIfNull(config);

        _repository = repository ?? throw new ArgumentNullException(nameof(repository));

        _config = config.Value ?? throw new ArgumentNullException(nameof(config));

        _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));

        ClassLevelCascadeMode = CascadeMode.Stop;

        RuleFor(static x => x.QueryParams.Email).EmailAddress()
            .OverridePropertyName("email")
            .WithMessage(_localizer["LoginUserValidatorEmailNotValidMsg"])
            .WithErrorCode(_localizer["LoginUserValidatorEmailNotValidCode"])
            .WithState(static _ => HttpStatusCode.BadRequest);

        RuleFor(static y => y.QueryParams.Email)
            .Must(ValidateEmail)
            .OverridePropertyName("email")
            .WithMessage(_localizer["LoginUserValidatorEmailNotFoundMsg"])
            .WithErrorCode(_localizer["LoginUserValidatorEmailNotFoundCode"])
            .WithState(static _ => HttpStatusCode.NotFound);

        RuleFor(static x => x.QueryParams.Email)
            .Must(ValidateUserActive)
            .OverridePropertyName("email")
            .WithMessage(_localizer["LoginUserValidatorUserDisabledMsg"])
            .WithErrorCode(_localizer["LoginUserValidatorUserDisabledCode"])
            .WithState(static _ => HttpStatusCode.Forbidden);

        RuleFor(static x => x.QueryParams)
            .Must(ValidatePassword)
            .OverridePropertyName("password")
            .WithMessage(_localizer["LoginUserValidatorPasswordInvalidMsg"])
            .WithErrorCode(_localizer["LoginUserValidatorPasswordInvalidCode"])
            .WithState(static _ => HttpStatusCode.Forbidden);
    }

    /// <summary>
    ///     Validates that the user associated with the specified email is currently active (enabled).
    /// </summary>
    /// <param name="email">The email address of the user to check active status for.</param>
    /// <returns><see langword="true"/> if the user is active; otherwise, <see langword="false"/>.</returns>
    private bool ValidateUserActive(string email)
    {
        var verification = new UserEnabledVerification(email);
        var result = _repository.Verificate(verification).Try();
        if (result.IsFaulted)
            throw new InternalServerErrorException(_localizer["InternalServerErrorValidationData"],
                result.ExtractException());

        return result.ExtractData();
    }

    /// <summary>
    ///     Validates that a user account with the specified email exists in the system.
    /// </summary>
    /// <param name="email">The email address to verify existence for.</param>
    /// <returns><see langword="true"/> if a user with the given email exists; otherwise, <see langword="false"/>.</returns>
    private bool ValidateEmail(string email)
    {
        var verification = new UserEmailVerification(email);
        var result = _repository.Verificate(verification).Try();
        if (result.IsFaulted)
            throw new InternalServerErrorException(_localizer["InternalServerErrorValidationData"],
                result.ExtractException());

        return result.ExtractData();
    }

    /// <summary>
    ///     Validates the user's password by retrieving credentials from the database and verifying against the provided password.
    /// </summary>
    /// <param name="queryParams">The login query parameters containing the email and password to verify.</param>
    /// <returns><see langword="true"/> if the provided password matches the stored credentials; otherwise, <see langword="false"/>.</returns>
    private bool ValidatePassword(QueryLoginUserParamsDto queryParams)
    {
        var specification = new UserReadByEmailSpecification(queryParams.Email);
        var resultUser = _repository.GetFirst(specification, UserProjections.UserCredentialsProjection).Try();
        if (resultUser.IsFaulted)
            throw new InternalServerErrorException(_localizer["InternalServerErrorValidationData"],
                resultUser.ExtractException());

        var resultValidation = PasswordUtils
            .VerifyUserCredentials(resultUser.ExtractData(), queryParams.Password, _config.Pepper).Try();
        if (resultValidation.IsFaulted)
            throw new InternalServerErrorException(_localizer["InternalServerErrorValidationData"],
                resultValidation.ExtractException());

        return resultValidation.ExtractData();
    }
}