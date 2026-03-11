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
using template.net10.api.Domain.Specifications;
using template.net10.api.Domain.Specifications.Generic;
using template.net10.api.Localize.Resources;
using template.net10.api.Persistence.Context;
using template.net10.api.Persistence.Models;
using template.net10.api.Persistence.Repositories.Interfaces;
using template.net10.api.Settings.Options;

namespace template.net10.api.Features.Commands;

/// <summary>
///     Represents a MediatR command request to create a new user in the system.
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
public sealed record CommandCreateUser(CommandCreateUserParamsDto CommandParams)
    : IRequest<LanguageExt.Common.Result<User>>, IEqualityOperators<CommandCreateUser, CommandCreateUser, bool>;

/// <summary>
///     Handles the <see cref="CommandCreateUser" /> command by creating a new user and persisting it to the database.
/// </summary>
[UsedImplicitly]
internal sealed class CommandCreateUserHandler(
    IGenericDbRepositoryWriteContext<AppDbContext, User> repository,
    IOptions<PasswordOptions> config,
    IUnitOfWork<AppDbContext> unitOfWork) : IRequestHandler<CommandCreateUser, LanguageExt.Common.Result<User>>
{
    /// <summary>
    ///     Password configuration options used for hashing and salting.
    /// </summary>
    private readonly PasswordOptions _config = config.Value ?? throw new ArgumentNullException(nameof(config));

    /// <summary>
    ///     Repository for write operations on <see cref="User" /> entities.
    /// </summary>
    private readonly IGenericDbRepositoryWriteContext<AppDbContext, User> _repository =
        repository ?? throw new ArgumentNullException(nameof(repository));

    /// <summary>
    ///     Unit of work for committing database transactions.
    /// </summary>
    private readonly IUnitOfWork<AppDbContext> _unitOfWork =
        unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

    /// <summary>
    ///     Handles the create user command by building the user entity, inserting it into the repository, and saving changes.
    /// </summary>
    /// <param name="request">The MediatR command containing the parameters for creating a new user.</param>
    /// <param name="cancellationToken">A token to observe for cancellation of the asynchronous operation.</param>
    /// <exception cref="ResultFaultedInvalidOperationException">
    ///     Result is not a failure! Use ExtractData method instead and
    ///     Check the state of Result with IsSuccess or IsFaulted before use this method or ExtractData method
    /// </exception>
    /// <exception cref="ResultSuccessInvalidOperationException">
    ///     Result is not a success! Use ExtractException method instead
    ///     and Check the state of Result with IsSuccess or IsFaulted before use this method or ExtractException method
    /// </exception>
    public async Task<LanguageExt.Common.Result<User>> Handle(CommandCreateUser request,
        CancellationToken cancellationToken)
    {
        var createUserResult = CreateUser(request);
        if (createUserResult.IsFaulted)
            return new LanguageExt.Common.Result<User>(createUserResult.ExtractException());

        var userInsertResult = await _repository.InsertAsync(createUserResult.ExtractData(), cancellationToken)
            .ConfigureAwait(false);
        if (userInsertResult.IsFaulted)
            return new LanguageExt.Common.Result<User>(userInsertResult.ExtractException());

        var unitResult = await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return unitResult.IsSuccess
            ? userInsertResult
            : new LanguageExt.Common.Result<User>(unitResult.ExtractException());
    }

    /// <summary>
    ///     Creates a new user DTO from the command parameters using the configured password pepper.
    /// </summary>
    private LanguageExt.Common.Result<CreateUserDto> CreateUser(CommandCreateUser request)
    {
        return UserFactory.CreateUser(request.CommandParams, _config.Pepper).Try();
    }
}

/// <summary>
///     FluentValidation validator that ensures the password and confirmation password match when creating a user.
/// </summary>
[UsedImplicitly]
internal sealed class CreateUserPasswordValidator : AbstractValidator<CommandCreateUser>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="CreateUserPasswordValidator" /> class with localization support.
    /// </summary>
    /// <exception cref="ArgumentNullException"><paramref name="localizer"/> is <see langword="null"/>.</exception>
    public CreateUserPasswordValidator(IStringLocalizer<ResourceMain> localizer)
    {
        var localizerService = localizer ?? throw new ArgumentNullException(nameof(localizer));

        RuleFor(static x => x.CommandParams.Password).Equal(static x => x.CommandParams.ConfirmPassword)
            .OverridePropertyName("confirm-password")
            .WithMessage(localizerService["CreateUserValidatorPasswordNotValidMsg"])
            .WithErrorCode(localizerService["CreateUserValidatorPasswordNotValidCode"])
            .WithState(static _ => HttpStatusCode.BadRequest);
    }
}

/// <summary>
///     FluentValidation validator that ensures the email is valid and not already in use when creating a user.
/// </summary>
[UsedImplicitly]
internal sealed class CreateUserEmailValidator : AbstractValidator<CommandCreateUser>
{
    /// <summary>
    ///     Localization service for retrieving validation error messages.
    /// </summary>
    private readonly IStringLocalizer<ResourceMain> _localizer;

    /// <summary>
    ///     Repository for read operations on <see cref="User" /> entities used during email validation.
    /// </summary>
    private readonly IGenericDbRepositoryReadContext<AppDbContext, User> _repository;

    /// <summary>
    ///     Initializes a new instance of the <see cref="CreateUserEmailValidator" /> class with repository and localization
    ///     dependencies.
    /// </summary>
    /// <exception cref="ArgumentNullException">
    ///     <paramref name="repository"/> is <see langword="null"/>.
    ///     -or-
    ///     <paramref name="localizer"/> is <see langword="null"/>.
    /// </exception>
    public CreateUserEmailValidator(
        IGenericDbRepositoryReadContext<AppDbContext, User> repository,
        IStringLocalizer<ResourceMain> localizer)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));

        _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));

        ClassLevelCascadeMode = CascadeMode.Stop;

        RuleFor(static x => x.CommandParams.Email).EmailAddress()
            .WithMessage(_localizer["CreateUserValidatorEmailNotValidMsg"])
            .WithErrorCode(_localizer["CreateUserValidatorEmailNotValidCode"])
            .WithState(static _ => HttpStatusCode.BadRequest);

        RuleFor(static x => x.CommandParams.Email)
            .Must(ValidateEmail)
            .WithMessage(_localizer["CreateUserValidatorEmailUsedMsg"])
            .WithErrorCode(_localizer["CreateUserValidatorEmailUsedCode"])
            .WithState(static _ => HttpStatusCode.Conflict);
    }

    /// <summary>
    ///     Validates that the provided email is not already registered in the system.
    /// </summary>
    private bool ValidateEmail(string email)
    {
        var verification = new UserEmailVerification(email);
        var result = _repository.Verificate(verification).Try();
        if (result.IsFaulted)
            throw new InternalServerErrorException(_localizer["InternalServerErrorValidationData"],
                result.ExtractException());

        return !result.ExtractData();
    }
}

/// <summary>
///     FluentValidation validator that ensures the specified role exists when creating a user.
/// </summary>
[UsedImplicitly]
internal sealed class CreateUserRoleValidator : AbstractValidator<CommandCreateUser>
{
    /// <summary>
    ///     Localization service for retrieving validation error messages.
    /// </summary>
    private readonly IStringLocalizer<ResourceMain> _localizer;

    /// <summary>
    ///     Repository for read operations on <see cref="Role" /> entities used during role validation.
    /// </summary>
    private readonly IGenericDbRepositoryReadContext<AppDbContext, Role> _repository;

    /// <summary>
    ///     Initializes a new instance of the <see cref="CreateUserRoleValidator" /> class with repository and localization
    ///     dependencies.
    /// </summary>
    /// <exception cref="ArgumentNullException">
    ///     <paramref name="repository"/> is <see langword="null"/>.
    ///     -or-
    ///     <paramref name="localizer"/> is <see langword="null"/>.
    /// </exception>
    public CreateUserRoleValidator(
        IGenericDbRepositoryReadContext<AppDbContext, Role> repository,
        IStringLocalizer<ResourceMain> localizer)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));

        _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));

        RuleFor(static x => x.CommandParams.RoleId ?? 0)
            .Must(ValidateRoleId)
            .OverridePropertyName("role-id")
            .WithMessage(_localizer["CreateUserValidatorRoleNotFoundCode"])
            .WithErrorCode(_localizer["CreateUserValidatorRoleNotFoundMsg"])
            .WithState(static _ => HttpStatusCode.NotFound)
            .When(static x => x.CommandParams.RoleId is not null);
    }

    /// <summary>
    ///     Validates that the specified role identifier exists in the database.
    /// </summary>
    private bool ValidateRoleId(short roleId)
    {
        var verification = new EntityVerificationById<Role, short>(roleId);
        var result = _repository.Verificate(verification).Try();
        if (result.IsFaulted)
            throw new InternalServerErrorException(_localizer["InternalServerErrorValidationData"],
                result.ExtractException());

        return result.ExtractData();
    }
}

/// <summary>
///     FluentValidation validator that verifies the requesting user's identity token is valid and the user is active.
/// </summary>
[UsedImplicitly]
internal sealed class CreateUserIdentifierValidator : AbstractValidator<CommandCreateUser>
{
    /// <summary>
    ///     Localization service for retrieving validation error messages.
    /// </summary>
    private readonly IStringLocalizer<ResourceMain> _localizer;

    /// <summary>
    ///     Repository for read operations on <see cref="User" /> entities used during identity validation.
    /// </summary>
    private readonly IGenericDbRepositoryReadContext<AppDbContext, User> _repository;

    /// <summary>
    ///     Initializes a new instance of the <see cref="CreateUserIdentifierValidator" /> class with repository and
    ///     localization dependencies.
    /// </summary>
    /// <exception cref="ArgumentNullException">
    ///     <paramref name="repository"/> is <see langword="null"/>.
    ///     -or-
    ///     <paramref name="localizer"/> is <see langword="null"/>.
    /// </exception>
    public CreateUserIdentifierValidator(
        IGenericDbRepositoryReadContext<AppDbContext, User> repository,
        IStringLocalizer<ResourceMain> localizer)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));

        _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));

        RuleFor(static x => x.CommandParams.Identity.UserUuid ?? Guid.Empty)
            .Must(ValidateIdentifier)
            .OverridePropertyName("access_token")
            .WithMessage(_localizer["TokenValidatorUserExistMsg"])
            .WithErrorCode(_localizer["TokenValidatorUserExistCode"])
            .WithState(static _ => HttpStatusCode.Forbidden)
            .When(static x => x.CommandParams.Identity.UserUuid is not null);

        RuleFor(static x => x.CommandParams.Identity.UserUuid ?? Guid.Empty)
            .Must(ValidateUserActive)
            .OverridePropertyName("access_token")
            .WithMessage(_localizer["TokenValidatorUserDisabledMsg"])
            .WithErrorCode(_localizer["TokenValidatorUserDisabledCode"])
            .WithState(static _ => HttpStatusCode.Forbidden)
            .When(static x => x.CommandParams.Identity.UserUuid is not null);
    }

    /// <summary>
    ///     Validates that a user with the specified UUID exists in the system.
    /// </summary>
    private bool ValidateIdentifier(Guid uuid)
    {
        var verification = new EntityVerificationByUuid<User>(uuid);
        var result = _repository.Verificate(verification).Try();
        if (result.IsFaulted)
            throw new InternalServerErrorException(_localizer["InternalServerErrorValidationData"],
                result.ExtractException());

        return result.ExtractData();
    }

    /// <summary>
    ///     Validates that the user with the specified UUID is currently active (enabled).
    /// </summary>
    private bool ValidateUserActive(Guid uuid)
    {
        var verification = new UserEnabledVerification(uuid);
        var result = _repository.Verificate(verification).Try();
        if (result.IsFaulted)
            throw new InternalServerErrorException(_localizer["InternalServerErrorValidationData"],
                result.ExtractException());

        return result.ExtractData();
    }
}