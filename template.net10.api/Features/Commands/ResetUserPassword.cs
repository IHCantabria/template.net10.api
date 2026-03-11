using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Net;
using System.Numerics;
using FluentValidation;
using JetBrains.Annotations;
using LanguageExt;
using MediatR;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
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
using template.net10.api.Settings.Options;

namespace template.net10.api.Features.Commands;

/// <summary>
///     Represents a MediatR command request to reset a user's password.
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
public sealed record CommandResetUserPassword(CommandResetUserPasswordParamsDto CommandParams)
    : IRequest<LanguageExt.Common.Result<UserResetedPasswordDto>>,
        IEqualityOperators<CommandResetUserPassword, CommandResetUserPassword, bool>;

/// <summary>
///     Handles the <see cref="CommandResetUserPassword" /> command by resetting a user's password and persisting the
///     change.
/// </summary>
internal sealed class CommandResetUserPasswordHandler(
    IGenericDbRepositoryWriteContext<AppDbContext, User> repository,
    IOptions<PasswordOptions> config,
    IUnitOfWork<AppDbContext> unitOfWork)
    : IRequestHandler<CommandResetUserPassword, LanguageExt.Common.Result<UserResetedPasswordDto>>
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
    ///     Handles the reset password command by retrieving the user, updating the password, and saving changes.
    /// </summary>
    /// <param name="request">The MediatR command containing the key and new password for the user.</param>
    /// <param name="cancellationToken">A token to observe for cancellation of the asynchronous operation.</param>
    /// <exception cref="ResultFaultedInvalidOperationException">
    ///     Result is not a failure! Use ExtractData method instead and
    ///     Check the state of Result with IsSuccess or IsFaulted before use this method or ExtractData method
    /// </exception>
    /// <exception cref="ResultSuccessInvalidOperationException">
    ///     Result is not a success! Use ExtractException method instead
    ///     and Check the state of Result with IsSuccess or IsFaulted before use this method or ExtractException method
    /// </exception>
    public async Task<LanguageExt.Common.Result<UserResetedPasswordDto>> Handle(CommandResetUserPassword request,
        CancellationToken cancellationToken)
    {
        var specification = new UserWriteByKeySpecification(request.CommandParams.Key);
        var userResult = await _repository.GetFirstAsync(specification, cancellationToken).ConfigureAwait(false);
        if (userResult.IsFaulted)
            return new LanguageExt.Common.Result<UserResetedPasswordDto>(userResult.ExtractException());

        var userUpdateResult = UpdateUser(request, userResult.ExtractData()).Try();
        if (userUpdateResult.IsFaulted)
            return new LanguageExt.Common.Result<UserResetedPasswordDto>(userResult.ExtractException());

        var unitResult = await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        UserResetedPasswordDto userDto = userUpdateResult.ExtractData();
        userDto.Password = request.CommandParams.Password;
        return unitResult.IsSuccess
            ? userDto
            : new LanguageExt.Common.Result<UserResetedPasswordDto>(unitResult.ExtractException());
    }

    /// <summary>
    ///     Updates the user's password using the command parameters and configured pepper.
    /// </summary>
    /// <param name="request">The command containing the new password and user key to apply the update for.</param>
    /// <param name="user">The <see cref="User"/> entity whose password will be updated.</param>
    /// <returns>
    ///     A <see cref="Try{User}"/> that, when executed, returns the updated <see cref="User"/> on success
    ///     or a faulted result containing the exception on failure.
    /// </returns>
    private Try<User> UpdateUser(CommandResetUserPassword request, User user)
    {
        return () =>
        {
            var userUpdatePasswordResult =
                user.UpdateUserPassword(request.CommandParams, _config.Pepper).Try();
            return userUpdatePasswordResult.IsFaulted
                ? new LanguageExt.Common.Result<User>(userUpdatePasswordResult.ExtractException())
                : _repository.Update(user).Try();
        };
    }
}

/// <summary>
///     FluentValidation validator that ensures the password and confirmation password match when resetting a user's
///     password.
/// </summary>
[UsedImplicitly]
internal sealed class ResetUserPasswordPasswordValidator : AbstractValidator<CommandResetUserPassword>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ResetUserPasswordPasswordValidator" /> class with localization
    ///     support.
    /// </summary>
    /// <param name="localizer">The string localizer for retrieving validation error messages.</param>
    /// <exception cref="ArgumentNullException"><paramref name="localizer"/> is <see langword="null"/>.</exception>
    public ResetUserPasswordPasswordValidator(IStringLocalizer<ResourceMain> localizer)
    {
        var localizer1 = localizer ?? throw new ArgumentNullException(nameof(localizer));

        RuleFor(static x => x.CommandParams.Password).Equal(static x => x.CommandParams.ConfirmPassword)
            .OverridePropertyName("confirm_password")
            .WithMessage(localizer1["ResetUserPasswordValidatorPasswordNotValidMsg"])
            .WithErrorCode(localizer1["ResetUserPasswordValidatorPasswordNotValidCode"])
            .WithErrorCode(StatusCodes.Status400BadRequest.ToString(CultureInfo.InvariantCulture))
            .WithState(static _ => HttpStatusCode.BadRequest);
    }
}

/// <summary>
///     FluentValidation validator that verifies the requesting user's identity token is valid and the user is active when
///     resetting a password.
/// </summary>
[UsedImplicitly]
internal sealed class ResetUserPasswordIdentifierValidator : AbstractValidator<CommandResetUserPassword>
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
    ///     Initializes a new instance of the <see cref="ResetUserPasswordIdentifierValidator" /> class with repository and
    ///     localization dependencies.
    /// </summary>
    /// <param name="repository">The read-only repository used to verify user identity during validation.</param>
    /// <param name="localizer">The string localizer for retrieving validation error messages.</param>
    /// <exception cref="ArgumentNullException">
    ///     <paramref name="repository"/> is <see langword="null"/>.
    ///     -or-
    ///     <paramref name="localizer"/> is <see langword="null"/>.
    /// </exception>
    public ResetUserPasswordIdentifierValidator(
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
    /// <param name="uuid">The unique identifier of the requesting user to verify existence for.</param>
    /// <returns><see langword="true"/> if the user exists; otherwise, <see langword="false"/>.</returns>
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
    /// <param name="uuid">The unique identifier of the requesting user to check active status for.</param>
    /// <returns><see langword="true"/> if the user is active; otherwise, <see langword="false"/>.</returns>
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
///     FluentValidation validator that ensures the target user key (UUID) exists and the user is active when resetting a
///     password.
/// </summary>
[UsedImplicitly]
internal sealed class ResetUserPasswordKeyValidator : AbstractValidator<CommandResetUserPassword>
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
    ///     Initializes a new instance of the <see cref="ResetUserPasswordKeyValidator" /> class with repository and
    ///     localization dependencies.
    /// </summary>
    /// <param name="repository">The read-only repository used to verify the target user's existence and state during validation.</param>
    /// <param name="localizer">The string localizer for retrieving validation error messages.</param>
    /// <exception cref="ArgumentNullException">
    ///     <paramref name="repository"/> is <see langword="null"/>.
    ///     -or-
    ///     <paramref name="localizer"/> is <see langword="null"/>.
    /// </exception>
    public ResetUserPasswordKeyValidator(
        IGenericDbRepositoryReadContext<AppDbContext, User> repository,
        IStringLocalizer<ResourceMain> localizer)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));

        _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));

        ClassLevelCascadeMode = CascadeMode.Stop;

        RuleFor(static x => x.CommandParams.Key)
            .Must(ValidateUserUuid)
            .OverridePropertyName("user-key")
            .WithMessage(_localizer["ResetUserPasswordValidatorUuidNotFoundMsg"])
            .WithErrorCode(_localizer["ResetUserPasswordValidatorUuidNotFoundCode"])
            .WithState(static _ => HttpStatusCode.NotFound);

        RuleFor(static x => x.CommandParams.Key)
            .Must(ValidateUserActive)
            .OverridePropertyName("user-key")
            .WithMessage(_localizer["ResetUserPasswordValidatorUserDisabledMsg"])
            .WithErrorCode(_localizer["ResetUserPasswordValidatorUserDisabledCode"])
            .WithState(static _ => HttpStatusCode.Forbidden);
    }

    /// <summary>
    ///     Validates that a user with the specified UUID exists in the database.
    /// </summary>
    /// <param name="key">The unique identifier (key) of the target user to verify existence for.</param>
    /// <returns><see langword="true"/> if the user exists; otherwise, <see langword="false"/>.</returns>
    private bool ValidateUserUuid(Guid key)
    {
        var verification = new EntityVerificationByUuid<User>(key);
        var result = _repository.Verificate(verification).Try();
        if (result.IsFaulted)
            throw new InternalServerErrorException(_localizer["InternalServerErrorValidationData"],
                result.ExtractException());

        return result.ExtractData();
    }

    /// <summary>
    ///     Validates that the user with the specified key is currently active (enabled).
    /// </summary>
    /// <param name="key">The unique identifier (key) of the target user to check active status for.</param>
    /// <returns><see langword="true"/> if the user is active; otherwise, <see langword="false"/>.</returns>
    private bool ValidateUserActive(Guid key)
    {
        var verification = new UserEnabledVerification(key);
        var result = _repository.Verificate(verification).Try();
        if (result.IsFaulted)
            throw new InternalServerErrorException(_localizer["InternalServerErrorValidationData"],
                result.ExtractException());

        return result.ExtractData();
    }
}