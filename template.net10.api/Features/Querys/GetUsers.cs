using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using MediatR;
using template.net10.api.Core.Exceptions;
using template.net10.api.Core.Extensions;
using template.net10.api.Domain.DTOs;
using template.net10.api.Domain.Specifications;
using template.net10.api.Persistence.Context;
using template.net10.api.Persistence.Models;
using template.net10.api.Persistence.Repositories.Interfaces;

namespace template.net10.api.Features.Querys;

/// <summary>
///     Represents a MediatR query request to retrieve all users from the system.
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
public sealed record QueryGetUsers : IRequest<LanguageExt.Common.Result<IEnumerable<UserDto>>>,
    IEqualityOperators<QueryGetUsers, QueryGetUsers, bool>;

/// <summary>
///     Handles the <see cref="QueryGetUsers"/> request by retrieving all users from the database.
/// </summary>
internal sealed class QueryGetUsersHandler(
    IGenericDbRepositoryReadContext<AppDbContext, User> repository)
    : IRequestHandler<QueryGetUsers, LanguageExt.Common.Result<IEnumerable<UserDto>>>
{
    /// <summary>
    ///     Read-only repository for querying <see cref="User"/> entities.
    /// </summary>
    private readonly IGenericDbRepositoryReadContext<AppDbContext, User> _repository =
        repository ?? throw new ArgumentNullException(nameof(repository));

    /// <summary>
    ///     Handles the query by retrieving all users with a read specification and returning their projected DTOs.
    /// </summary>
    /// <param name="request">The MediatR query request to retrieve all users.</param>
    /// <param name="cancellationToken">A token to observe for cancellation of the asynchronous operation.</param>
    /// <exception cref="ResultFaultedInvalidOperationException">
    ///     Result is not a failure! Use ExtractData method instead and
    ///     Check the state of Result with IsSuccess or IsFaulted before use this method or ExtractData method
    /// </exception>
    public async Task<LanguageExt.Common.Result<IEnumerable<UserDto>>> Handle(QueryGetUsers request,
        CancellationToken cancellationToken)
    {
        var specification = new UsersReadSpecification();
        var result = await _repository.GetAsync(specification, UserProjections.UserProjection, cancellationToken)
            .ConfigureAwait(false);
        return result.IsSuccess
            ? result
            : new LanguageExt.Common.Result<IEnumerable<UserDto>>(result.ExtractException());
    }
}