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
using template.net10.api.Persistence.Repositories.Interfaces;

namespace template.net10.api.Features.Commands;

/// <summary>
///     Represents a MediatR command request to delete an existing user from the system.
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
public sealed record CommandDeleteUser(CommandDeleteUserParamsDto CommandParams)
    : IRequest<LanguageExt.Common.Result<User>>, IEqualityOperators<CommandDeleteUser, CommandDeleteUser, bool>;

/// <summary>
///     Handles the <see cref="CommandDeleteUser" /> command by deleting a user from the database.
/// </summary>
internal sealed class CommandDeleteUserHandler(
    IGenericDbRepositoryWriteContext<AppDbContext, User> repository,
    IUnitOfWork<AppDbContext> unitOfWork) : IRequestHandler<CommandDeleteUser, LanguageExt.Common.Result<User>>
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
    ///     Handles the delete user command by locating the user by key and removing it from the database.
    /// </summary>
    /// <param name="request">The MediatR command containing the key of the user to delete.</param>
    /// <param name="cancellationToken">A token to observe for cancellation of the asynchronous operation.</param>
    /// <exception cref="ResultFaultedInvalidOperationException">
    ///     Result is not a failure! Use ExtractData method instead and
    ///     Check the state of Result with IsSuccess or IsFaulted before use this method or ExtractData method
    /// </exception>
    /// <exception cref="ResultSuccessInvalidOperationException">
    ///     Result is not a success! Use ExtractException method instead
    ///     and Check the state of Result with IsSuccess or IsFaulted before use this method or ExtractException method
    /// </exception>
    public async Task<LanguageExt.Common.Result<User>> Handle(CommandDeleteUser request,
        CancellationToken cancellationToken)
    {
        var specification = new UserWriteDeleteByKeySpecification(request.CommandParams.Key);
        var userResult = await _repository.GetFirstAsync(specification, cancellationToken)
            .ConfigureAwait(false);
        if (userResult.IsFaulted)
            return new LanguageExt.Common.Result<User>(userResult.ExtractException());

        return await DeleteUserAsync(userResult.ExtractData(),
            cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    ///     Deletes the specified user entity from the repository and commits the transaction.
    /// </summary>
    private async Task<LanguageExt.Common.Result<User>> DeleteUserAsync(User user,
        CancellationToken cancellationToken)
    {
        var deletedResult = _repository.Delete(user).Try();
        if (deletedResult.IsFaulted)
            return new LanguageExt.Common.Result<User>(deletedResult.ExtractException());

        var unitResult = await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return unitResult.IsSuccess
            ? deletedResult
            : new LanguageExt.Common.Result<User>(unitResult.ExtractException());
    }
}

/// <summary>
///     FluentValidation validator that verifies the requesting user's identity token is valid and the user is active when
///     deleting a user.
/// </summary>
[UsedImplicitly]
internal sealed class DeleteUserHandlerIdentifierValidator : AbstractValidator<CommandDeleteUser>
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
    ///     Initializes a new instance of the <see cref="DeleteUserHandlerIdentifierValidator" /> class with repository and
    ///     localization dependencies.
    /// </summary>
    /// <exception cref="ArgumentNullException">
    ///     <paramref name="repository"/> is <see langword="null"/>.
    ///     -or-
    ///     <paramref name="localizer"/> is <see langword="null"/>.
    /// </exception>
    public DeleteUserHandlerIdentifierValidator(
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
///     FluentValidation validator that ensures the target user exists before deletion.
/// </summary>
[UsedImplicitly]
internal sealed class DeleteUserUserValidator : AbstractValidator<CommandDeleteUser>
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
    ///     Initializes a new instance of the <see cref="DeleteUserUserValidator" /> class with repository and localization
    ///     dependencies.
    /// </summary>
    /// <exception cref="ArgumentNullException">
    ///     <paramref name="repository"/> is <see langword="null"/>.
    ///     -or-
    ///     <paramref name="localizer"/> is <see langword="null"/>.
    /// </exception>
    public DeleteUserUserValidator(
        IGenericDbRepositoryReadContext<AppDbContext, User> repository,
        IStringLocalizer<ResourceMain> localizer)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));

        _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));

        ClassLevelCascadeMode = CascadeMode.Stop;

        RuleFor(static x => x.CommandParams.Key)
            .Must(ValidateUserUuid)
            .OverridePropertyName("user-key")
            .WithMessage(_localizer["DeleteUserValidatorUuidNotFoundMsg"])
            .WithErrorCode(_localizer["DeleteUserValidatorUuidNotFoundCode"])
            .WithState(static _ => HttpStatusCode.NotFound);
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
}