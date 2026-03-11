using System.Diagnostics.CodeAnalysis;
using System.Globalization;
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
///     Represents a MediatR command request to disable an existing user in the system.
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
public sealed record CommandDisableUser(CommandDisableUserParamsDto CommandParams)
    : IRequest<LanguageExt.Common.Result<User>>,
        IEqualityOperators<CommandDisableUser, CommandDisableUser, bool>;

/// <summary>
///     Handles the <see cref="CommandDisableUser" /> command by disabling a user and persisting changes to the database.
/// </summary>
internal sealed class CommandDisableUserHandler(
    IGenericDbRepositoryWriteContext<AppDbContext, User> repository,
    IUnitOfWork<AppDbContext> unitOfWork) : IRequestHandler<CommandDisableUser, LanguageExt.Common.Result<User>>
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
    ///     Handles the disable user command by retrieving the user, marking it as disabled, and saving changes.
    /// </summary>
    /// <param name="request">The MediatR command containing the key of the user to disable.</param>
    /// <param name="cancellationToken">A token to observe for cancellation of the asynchronous operation.</param>
    /// <exception cref="ResultSuccessInvalidOperationException">
    ///     Result is not a success! Use ExtractException method instead
    ///     and Check the state of Result with IsSuccess or IsFaulted before use this method or ExtractException method
    /// </exception>
    /// <exception cref="ResultFaultedInvalidOperationException">
    ///     Result is not a failure! Use ExtractData method instead and
    ///     Check the state of Result with IsSuccess or IsFaulted before use this method or ExtractData method
    /// </exception>
    /// <exception cref="ResultFaultedInvalidOperationException">
    ///     Result is not a failure! Use ExtractData method instead and
    ///     Check the state of Result with IsSuccess or IsFaulted before use this method or ExtractData method
    /// </exception>
    public async Task<LanguageExt.Common.Result<User>> Handle(CommandDisableUser request,
        CancellationToken cancellationToken)
    {
        var specification = new UserWriteByKeySpecification(request.CommandParams.Key);
        var userResult = await _repository.GetFirstAsync(specification, cancellationToken)
            .ConfigureAwait(false);
        if (userResult.IsFaulted)
            return new LanguageExt.Common.Result<User>(userResult.ExtractException());

        var userToDisable = userResult.ExtractData();
        userToDisable.DisableUser();
        var updateResult = _repository.Update(userToDisable).Try();
        if (updateResult.IsFaulted)
            return new LanguageExt.Common.Result<User>(updateResult.ExtractException());

        var unitResult = await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return unitResult.IsSuccess
            ? updateResult
            : new LanguageExt.Common.Result<User>(updateResult.ExtractException());
    }
}

/// <summary>
///     FluentValidation validator that verifies the requesting user's identity token is valid and the user is active when
///     disabling a user.
/// </summary>
[UsedImplicitly]
internal sealed class DisableUserIdentifierValidator : AbstractValidator<CommandDisableUser>
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
    ///     Initializes a new instance of the <see cref="DisableUserIdentifierValidator" /> class with repository and
    ///     localization dependencies.
    /// </summary>
    /// <param name="repository">The read-only repository used to verify user identity during validation.</param>
    /// <param name="localizer">The string localizer for retrieving validation error messages.</param>
    /// <exception cref="ArgumentNullException">
    ///     <paramref name="repository" /> is <see langword="null" />.
    ///     -or-
    ///     <paramref name="localizer" /> is <see langword="null" />.
    /// </exception>
    public DisableUserIdentifierValidator(
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
    /// <param name="uuid">The unique identifier of the user to verify existence for.</param>
    /// <returns><see langword="true" /> if the user exists; otherwise, <see langword="false" />.</returns>
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
    /// <param name="uuid">The unique identifier of the user to check active status for.</param>
    /// <returns><see langword="true" /> if the user is active; otherwise, <see langword="false" />.</returns>
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
///     FluentValidation validator that ensures the target user exists and is not already disabled.
/// </summary>
[UsedImplicitly]
internal sealed class DisableUserUserValidator : AbstractValidator<CommandDisableUser>
{
    /// <summary>
    ///     Localization service for retrieving validation error messages.
    /// </summary>
    private readonly IStringLocalizer<ResourceMain> _localizer;

    /// <summary>
    ///     Repository for read operations on <see cref="User" /> entities used during user validation.
    /// </summary>
    private readonly IGenericDbRepositoryReadContext<AppDbContext, User> _repository;

    /// <summary>
    ///     Initializes a new instance of the <see cref="DisableUserUserValidator" /> class with repository and localization
    ///     dependencies.
    /// </summary>
    /// <param name="repository">The read-only repository used to verify user existence and state during validation.</param>
    /// <param name="localizer">The string localizer for retrieving validation error messages.</param>
    /// <exception cref="ArgumentNullException">
    ///     <paramref name="repository" /> is <see langword="null" />.
    ///     -or-
    ///     <paramref name="localizer" /> is <see langword="null" />.
    /// </exception>
    public DisableUserUserValidator(
        IGenericDbRepositoryReadContext<AppDbContext, User> repository,
        IStringLocalizer<ResourceMain> localizer)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));

        _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));

        ClassLevelCascadeMode = CascadeMode.Stop;

        RuleFor(static x => x.CommandParams.Key)
            .Must(ValidateUserUuid)
            .WithMessage(_localizer["DisableUserValidatorUuidNotFoundMsg"])
            .WithErrorCode(_localizer["DisableUserValidatorUuidNotFoundCode"])
            .WithErrorCode(StatusCodes.Status404NotFound.ToString(CultureInfo.InvariantCulture))
            .WithState(static _ => HttpStatusCode.NotFound);

        RuleFor(static x => x.CommandParams.Key)
            .Must(ValidateUserDisabled)
            .OverridePropertyName("user-key")
            .WithMessage(_localizer["DisableUserValidatorUserDisabledMsg"])
            .WithErrorCode(_localizer["DisableUserValidatorUserDisabledCode"])
            .WithState(static _ => HttpStatusCode.Conflict);
    }

    /// <summary>
    ///     Validates that a user with the specified UUID exists in the database.
    /// </summary>
    /// <param name="userUuid">The unique identifier of the user to validate existence for.</param>
    /// <returns><see langword="true" /> if the user exists; otherwise, <see langword="false" />.</returns>
    private bool ValidateUserUuid(Guid userUuid)
    {
        var verification = new EntityVerificationByUuid<User>(userUuid);
        var result = _repository.Verificate(verification).Try();
        if (result.IsFaulted)
            throw new InternalServerErrorException(_localizer["InternalServerErrorValidationData"],
                result.ExtractException());

        return result.ExtractData();
    }

    /// <summary>
    ///     Validates that the user with the specified UUID is not already disabled.
    /// </summary>
    /// <param name="userUuid">The unique identifier of the user to check disabled state for.</param>
    /// <returns><see langword="true" /> if the user is not already disabled; otherwise, <see langword="false" />.</returns>
    private bool ValidateUserDisabled(Guid userUuid)
    {
        var verification = new UserDisabledVerification(userUuid);
        var result = _repository.Verificate(verification).Try();
        if (result.IsFaulted)
            throw new InternalServerErrorException(_localizer["InternalServerErrorValidationData"],
                result.ExtractException());

        return !result.ExtractData();
    }
}