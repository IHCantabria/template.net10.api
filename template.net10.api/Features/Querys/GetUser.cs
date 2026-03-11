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

namespace template.net10.api.Features.Querys;

/// <summary>
///     Represents a MediatR query request to retrieve a single user by key.
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
public sealed record QueryGetUser(QueryGetUserParamsDto QueryParams)
    : IRequest<LanguageExt.Common.Result<UserDto>>, IEqualityOperators<QueryGetUser, QueryGetUser, bool>;

/// <summary>
///     Handles the <see cref="QueryGetUser"/> request by retrieving a single user from the database by key.
/// </summary>
internal sealed class QueryGetUserHandler(
    IGenericDbRepositoryReadContext<AppDbContext, User> repository)
    : IRequestHandler<QueryGetUser, LanguageExt.Common.Result<UserDto>>
{
    /// <summary>
    ///     Read-only repository for querying <see cref="User"/> entities.
    /// </summary>
    private readonly IGenericDbRepositoryReadContext<AppDbContext, User> _repository =
        repository ?? throw new ArgumentNullException(nameof(repository));


    /// <summary>
    ///     Handles the query by retrieving a user matching the specified key and returning a projected DTO.
    /// </summary>
    /// <param name="request">The MediatR query request containing the key of the user to retrieve.</param>
    /// <param name="cancellationToken">A token to observe for cancellation of the asynchronous operation.</param>
    /// <exception cref="ResultFaultedInvalidOperationException">
    ///     Result is not a failure! Use ExtractData method instead and
    ///     Check the state of Result with IsSuccess or IsFaulted before use this method or ExtractData method
    /// </exception>
    public async Task<LanguageExt.Common.Result<UserDto>> Handle(QueryGetUser request,
        CancellationToken cancellationToken)
    {
        var specification = new UserReadByKeySpecification(request.QueryParams.Key);
        var result = await _repository.GetFirstAsync(specification, UserProjections.UserProjection, cancellationToken)
            .ConfigureAwait(false);
        return result.IsSuccess
            ? result
            : new LanguageExt.Common.Result<UserDto>(result.ExtractException());
    }
}

/// <summary>
///     FluentValidation validator that ensures the requested user key (UUID) exists in the system.
/// </summary>
[UsedImplicitly]
internal sealed class GetUserKeyValidator : AbstractValidator<QueryGetUser>
{
    /// <summary>
    ///     Localization service for retrieving validation error messages.
    /// </summary>
    private readonly IStringLocalizer<ResourceMain> _localizer;

    /// <summary>
    ///     Read-only repository for verifying <see cref="User"/> entities during validation.
    /// </summary>
    private readonly IGenericDbRepositoryReadContext<AppDbContext, User> _repository;


    /// <summary>
    ///     Initializes a new instance of the <see cref="GetUserKeyValidator"/> class with repository and localization dependencies.
    /// </summary>
    /// <param name="repository">The read-only repository used to verify user existence during validation.</param>
    /// <param name="localizer">The string localizer for retrieving validation error messages.</param>
    /// <exception cref="ArgumentNullException">
    ///     <paramref name="repository"/> is <see langword="null"/>.
    ///     -or-
    ///     <paramref name="localizer"/> is <see langword="null"/>.
    /// </exception>
    public GetUserKeyValidator(
        IGenericDbRepositoryReadContext<AppDbContext, User> repository,
        IStringLocalizer<ResourceMain> localizer)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));

        _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));

        ClassLevelCascadeMode = CascadeMode.Stop;

        RuleFor(static x => x.QueryParams.Key)
            .Must(ValidateUserUuid)
            .OverridePropertyName("user-key")
            .WithMessage(_localizer["GetUserValidatorUuidNotFoundMsg"])
            .WithErrorCode(_localizer["GetUserValidatorUuidNotFoundCode"])
            .WithState(static _ => HttpStatusCode.NotFound);
    }

    /// <summary>
    ///     Validates that a user with the specified UUID exists in the database.
    /// </summary>
    /// <param name="key">The unique identifier of the user to verify existence for.</param>
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
}