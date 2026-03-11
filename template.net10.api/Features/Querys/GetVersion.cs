using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using MediatR;
using template.net10.api.Core.DTOs;
using template.net10.api.Core.Exceptions;
using template.net10.api.Core.Extensions;
using template.net10.api.Domain.Specifications;
using template.net10.api.Persistence.Context;
using template.net10.api.Persistence.Models;
using template.net10.api.Persistence.Repositories.Interfaces;

namespace template.net10.api.Features.Querys;

/// <summary>
///     Represents a MediatR query request to retrieve the current database version.
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
public sealed record QueryGetVersion : IRequest<LanguageExt.Common.Result<VersionDto>>,
    IEqualityOperators<QueryGetVersion, QueryGetVersion, bool>;

/// <summary>
///     Handles the <see cref="QueryGetVersion"/> request by retrieving the current database version.
/// </summary>
internal sealed class QueryGetVersionHandler(
    IGenericDbRepositoryReadContext<AppDbContext, CurrentVersion> repository)
    : IRequestHandler<QueryGetVersion, LanguageExt.Common.Result<VersionDto>>
{
    /// <summary>
    ///     Read-only repository for querying <see cref="CurrentVersion"/> entities.
    /// </summary>
    private readonly IGenericDbRepositoryReadContext<AppDbContext, CurrentVersion> _repository =
        repository ?? throw new ArgumentNullException(nameof(repository));

    /// <summary>
    ///     Handles the query by retrieving the current database version using a read specification.
    /// </summary>
    /// <param name="request">The MediatR query request to retrieve the current version.</param>
    /// <param name="cancellationToken">A token to observe for cancellation of the asynchronous operation.</param>
    /// <exception cref="ResultFaultedInvalidOperationException">
    ///     Result is not a failure! Use ExtractData method instead and
    ///     Check the state of Result with IsSuccess or IsFaulted before use this method or ExtractData method
    /// </exception>
    public async Task<LanguageExt.Common.Result<VersionDto>> Handle(QueryGetVersion request,
        CancellationToken cancellationToken)
    {
        var specification = new CurrentVersionReadSpecification();
        var result = await _repository
            .GetFirstAsync(specification, CurrentVersionProjections.VersionProjection, cancellationToken)
            .ConfigureAwait(false);
        return result.IsSuccess
            ? result
            : new LanguageExt.Common.Result<VersionDto>(result.ExtractException());
    }
}