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
///     Represents a MediatR command request to enable a previously disabled user in the system.
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
public sealed record CommandEnableUser(CommandEnableUserParamsDto CommandParams)
    : IRequest<LanguageExt.Common.Result<User>>, IEqualityOperators<CommandEnableUser, CommandEnableUser, bool>;

/// <summary>
///     Handles the <see cref="CommandEnableUser" /> command by enabling a user and persisting changes to the database.
/// </summary>
internal sealed class CommandEnableUserHandler(
    IGenericDbRepositoryWriteContext<AppDbContext, User> repository,
    IUnitOfWork<AppDbContext> unitOfWork) : IRequestHandler<CommandEnableUser, LanguageExt.Common.Result<User>>
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
    ///     Handles the enable user command by retrieving the user, marking it as enabled, and saving changes.
    /// </summary>
    /// <param name="request">The MediatR command containing the key of the user to enable.</param>
    /// <param name="cancellationToken">A token to observe for cancellation of the asynchronous operation.</param>
    /// <exception cref="ResultFaultedInvalidOperationException">
    ///     Result is not a failure! Use ExtractData method instead and
    ///     Check the state of Result with IsSuccess or IsFaulted before use this method or ExtractData method
    /// </exception>
    /// <exception cref="ResultSuccessInvalidOperationException">
    ///     Result is not a success! Use ExtractException method instead
    ///     and Check the state of Result with IsSuccess or IsFaulted before use this method or ExtractException method
    /// </exception>
    public async Task<LanguageExt.Common.Result<User>> Handle(CommandEnableUser request,
        CancellationToken cancellationToken)
    {
        var specification = new UserWriteByKeySpecification(request.CommandParams.Key);
        var userResult = await _repository.GetFirstAsync(specification, cancellationToken)
            .ConfigureAwait(false);
        if (userResult.IsFaulted)
            return new LanguageExt.Common.Result<User>(userResult.ExtractException());

        var userToEnable = userResult.ExtractData();
        userToEnable.EnableUser();
        var updateResult = _repository.Update(userToEnable).Try();
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
///     enabling a user.
/// </summary>
[UsedImplicitly]
internal sealed class EnableUserIdentifierValidator : AbstractValidator<CommandEnableUser>
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
    ///     Initializes a new instance of the <see cref="EnableUserIdentifierValidator" /> class with repository and
    ///     localization dependencies.
    /// </summary>
    /// <exception cref="ArgumentNullException">
    ///     <paramref name="repository"/> is <see langword="null"/>.
    ///     -or-
    ///     <paramref name="localizer"/> is <see langword="null"/>.
    /// </exception>
    public EnableUserIdentifierValidator(
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
///     FluentValidation validator that ensures the target user exists and is not already enabled.
/// </summary>
[UsedImplicitly]
internal sealed class EnableUserUserValidator : AbstractValidator<CommandEnableUser>
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
    ///     Initializes a new instance of the <see cref="EnableUserUserValidator" /> class with repository and localization
    ///     dependencies.
    /// </summary>
    /// <exception cref="ArgumentNullException">
    ///     <paramref name="repository"/> is <see langword="null"/>.
    ///     -or-
    ///     <paramref name="localizer"/> is <see langword="null"/>.
    /// </exception>
    public EnableUserUserValidator(
        IGenericDbRepositoryReadContext<AppDbContext, User> repository,
        IStringLocalizer<ResourceMain> localizer)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));

        _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));

        ClassLevelCascadeMode = CascadeMode.Stop;

        RuleFor(static x => x.CommandParams.Key)
            .Must(ValidateUserUuid)
            .WithMessage(_localizer["EnableUserValidatorUuidNotFoundMsg"])
            .WithErrorCode(_localizer["EnableUserValidatorUuidNotFoundCode"])
            .WithErrorCode(StatusCodes.Status404NotFound.ToString(CultureInfo.InvariantCulture))
            .WithState(static _ => HttpStatusCode.NotFound);

        RuleFor(static x => x.CommandParams.Key)
            .Must(ValidateUserEnabled)
            .OverridePropertyName("user-key")
            .WithMessage(_localizer["EnableUserValidatorUserEnabledMsg"])
            .WithErrorCode(_localizer["EnableUserValidatorUserEnabledCode"])
            .WithState(static _ => HttpStatusCode.Conflict);
    }

    /// <summary>
    ///     Validates that a user with the specified UUID exists in the database.
    /// </summary>
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
    ///     Validates that the user with the specified UUID is not already enabled.
    /// </summary>
    private bool ValidateUserEnabled(Guid userUuid)
    {
        var verification = new UserEnabledVerification(userUuid);
        var result = _repository.Verificate(verification).Try();
        if (result.IsFaulted)
            throw new InternalServerErrorException(_localizer["InternalServerErrorValidationData"],
                result.ExtractException());

        return !result.ExtractData();
    }
}