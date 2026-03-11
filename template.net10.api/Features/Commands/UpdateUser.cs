using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Numerics;
using FluentValidation;
using JetBrains.Annotations;
using MediatR;
using Microsoft.Extensions.Localization;
using template.net10.api.Core.Exceptions;
using template.net10.api.Core.Extensions;
using template.net10.api.Domain.DTOs;
using template.net10.api.Domain.Specifications;
using template.net10.api.Domain.Specifications.Generic;
using template.net10.api.Localize.Resources;
using template.net10.api.Persistence.Context;
using template.net10.api.Persistence.Models;
using template.net10.api.Persistence.Models.Extensions;
using template.net10.api.Persistence.Repositories.Interfaces;

namespace template.net10.api.Features.Commands;

/// <summary>
///     Represents a MediatR command request to update an existing user in the system.
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
public sealed record CommandUpdateUser(CommandUpdateUserParamsDto CommandParams)
    : IRequest<LanguageExt.Common.Result<User>>, IEqualityOperators<CommandUpdateUser, CommandUpdateUser, bool>;

/// <summary>
///     Handles the <see cref="CommandUpdateUser" /> command by updating an existing user and persisting changes to the
///     database.
/// </summary>
internal sealed class CommandUpdateUserHandler(
    IGenericDbRepositoryWriteContext<AppDbContext, User> repository,
    IUnitOfWork<AppDbContext> unitOfWork) : IRequestHandler<CommandUpdateUser, LanguageExt.Common.Result<User>>
{
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
    ///     Handles the update user command by retrieving the user, applying changes, and saving to the database.
    /// </summary>
    /// <param name="request">The MediatR command containing the parameters for updating an existing user.</param>
    /// <param name="cancellationToken">A token to observe for cancellation of the asynchronous operation.</param>
    /// <exception cref="ResultSuccessInvalidOperationException">
    ///     Result is not a success! Use ExtractException method instead
    ///     and Check the state of Result with IsSuccess or IsFaulted before use this method or ExtractException method
    /// </exception>
    /// <exception cref="ResultFaultedInvalidOperationException">
    ///     Result is not a failure! Use ExtractData method instead and
    ///     Check the state of Result with IsSuccess or IsFaulted before use this method or ExtractData method
    /// </exception>
    public async Task<LanguageExt.Common.Result<User>> Handle(CommandUpdateUser request,
        CancellationToken cancellationToken)
    {
        var specification = new UserWriteByKeySpecification(request.CommandParams.Key);
        var userResult = await _repository.GetFirstAsync(specification, cancellationToken)
            .ConfigureAwait(false);
        if (userResult.IsFaulted)
            return new LanguageExt.Common.Result<User>(userResult.ExtractException());

        var userToUpdate = userResult.ExtractData();
        userToUpdate.UpdateUser(request.CommandParams);
        var updateResult = _repository.Update(userToUpdate).Try();
        if (updateResult.IsFaulted)
            return new LanguageExt.Common.Result<User>(updateResult.ExtractException());

        var unitResult = await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return unitResult.IsSuccess
            ? updateResult
            : new LanguageExt.Common.Result<User>(updateResult.ExtractException());
    }
}

/// <summary>
///     FluentValidation validator that ensures the specified role exists when updating a user.
/// </summary>
[UsedImplicitly]
internal sealed class UpdateUserRoleValidator : AbstractValidator<CommandUpdateUser>
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
    ///     Initializes a new instance of the <see cref="UpdateUserRoleValidator" /> class with repository and localization
    ///     dependencies.
    /// </summary>
    /// <exception cref="ArgumentNullException">
    ///     <paramref name="repository"/> is <see langword="null"/>.
    ///     -or-
    ///     <paramref name="localizer"/> is <see langword="null"/>.
    /// </exception>
    public UpdateUserRoleValidator(
        IGenericDbRepositoryReadContext<AppDbContext, Role> repository,
        IStringLocalizer<ResourceMain> localizer)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));

        _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));

        RuleFor(static x => x.CommandParams.RoleId ?? 0)
            .Must(ValidateRoleId)
            .OverridePropertyName("role-id")
            .WithMessage(_localizer["UpdateUserValidatorRoleNotFoundCode"])
            .WithErrorCode(_localizer["UpdateUserValidatorRoleNotFoundMsg"])
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
///     FluentValidation validator that ensures the email is valid and not already in use when updating a user.
/// </summary>
[UsedImplicitly]
internal sealed class UpdateUserEmailValidator : AbstractValidator<CommandUpdateUser>
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
    ///     Initializes a new instance of the <see cref="UpdateUserEmailValidator" /> class with repository and localization
    ///     dependencies.
    /// </summary>
    /// <exception cref="ArgumentNullException">
    ///     <paramref name="repository"/> is <see langword="null"/>.
    ///     -or-
    ///     <paramref name="localizer"/> is <see langword="null"/>.
    /// </exception>
    public UpdateUserEmailValidator(
        IGenericDbRepositoryReadContext<AppDbContext, User> repository,
        IStringLocalizer<ResourceMain> localizer)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));

        _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));

        ClassLevelCascadeMode = CascadeMode.Stop;

        RuleFor(static x => x.CommandParams.Email ?? "").EmailAddress()
            .WithMessage(_localizer["UpdateUserValidatorEmailNotValidMsg"])
            .WithErrorCode(_localizer["UpdateUserValidatorEmailNotValidCode"])
            .WithState(static _ => HttpStatusCode.BadRequest)
            .When(static x => x.CommandParams.Email is not null);

        RuleFor(static x => x.CommandParams.Email ?? "")
            .Must(ValidateEmail)
            .WithMessage(_localizer["UpdateUserValidatorEmailUsedMsg"])
            .WithErrorCode(_localizer["UpdateUserValidatorEmailUsedCode"])
            .WithState(static _ => HttpStatusCode.Conflict)
            .When(static x => x.CommandParams.Email is not null);
    }

    /// <summary>
    ///     Validates that the provided email is not already registered by another user in the system.
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
///     FluentValidation validator that verifies the requesting user's identity token is valid and the user is active when
///     updating a user.
/// </summary>
[UsedImplicitly]
internal sealed class UpdateUserIdentifierValidator : AbstractValidator<CommandUpdateUser>
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
    ///     Initializes a new instance of the <see cref="UpdateUserIdentifierValidator" /> class with repository and
    ///     localization dependencies.
    /// </summary>
    /// <exception cref="ArgumentNullException">
    ///     <paramref name="repository"/> is <see langword="null"/>.
    ///     -or-
    ///     <paramref name="localizer"/> is <see langword="null"/>.
    /// </exception>
    public UpdateUserIdentifierValidator(
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

/// <summary>
///     FluentValidation validator that ensures the target user key (UUID) exists in the system when updating a user.
/// </summary>
[UsedImplicitly]
internal sealed class UpdateUserKeyValidator : AbstractValidator<CommandUpdateUser>
{
    /// <summary>
    ///     Localization service for retrieving validation error messages.
    /// </summary>
    private readonly IStringLocalizer<ResourceMain> _localizer;

    /// <summary>
    ///     Repository for read operations on <see cref="User" /> entities used during key validation.
    /// </summary>
    private readonly IGenericDbRepositoryReadContext<AppDbContext, User> _repository;

    /// <summary>
    ///     Initializes a new instance of the <see cref="UpdateUserKeyValidator" /> class with repository and localization
    ///     dependencies.
    /// </summary>
    /// <exception cref="ArgumentNullException">
    ///     <paramref name="repository"/> is <see langword="null"/>.
    ///     -or-
    ///     <paramref name="localizer"/> is <see langword="null"/>.
    /// </exception>
    public UpdateUserKeyValidator(
        IGenericDbRepositoryReadContext<AppDbContext, User> repository,
        IStringLocalizer<ResourceMain> localizer)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));

        _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));

        ClassLevelCascadeMode = CascadeMode.Stop;

        RuleFor(static x => x.CommandParams.Key)
            .Must(ValidateUserUuid)
            .OverridePropertyName("user-key")
            .WithMessage(_localizer["UpdateUserValidatorUuidNotFoundMsg"])
            .WithErrorCode(_localizer["UpdateUserValidatorUuidNotFoundCode"])
            .WithState(static _ => HttpStatusCode.NotFound);
    }

    /// <summary>
    ///     Validates that a user with the specified UUID exists in the database.
    /// </summary>
    private bool ValidateUserUuid(Guid key)
    {
        var verification = new EntityVerificationByUuid<User>(key);
        var result = _repository.Verificate(verification).Try();
        if (result.IsFaulted)
            throw new InternalServerErrorException(_localizer["InternalServerErrorValidationData"],
                result.ExtractException());

        return result.ExtractData();
    }
}