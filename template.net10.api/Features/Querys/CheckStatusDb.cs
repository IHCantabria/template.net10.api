using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using MediatR;
using Microsoft.Extensions.Options;
using Npgsql;
using template.net10.api.Core.DTOs;
using template.net10.api.Core.Exceptions;
using template.net10.api.Core.Extensions;
using template.net10.api.Logger;
using template.net10.api.Persistence.Context;
using template.net10.api.Persistence.Models;
using template.net10.api.Persistence.Repositories.Interfaces;
using template.net10.api.Settings.Options;

namespace template.net10.api.Features.Querys;

/// <summary>
///     Represents a MediatR query request to check the database connection status and API health.
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
public sealed record QueryCheckStatus : IRequest<LanguageExt.Common.Result<InfoDto>>,
    IEqualityOperators<QueryCheckStatus, QueryCheckStatus, bool>;

/// <summary>
///     Handles the <see cref="QueryCheckStatus" /> request by verifying database connectivity and returning API status
///     information.
/// </summary>
internal sealed class QueryCheckStatusHandler(
    IGenericDbRepositoryReadContext<AppDbContext, CurrentVersion> repository,
    IOptions<ProjectOptions> options,
    ILogger<QueryCheckStatusHandler> logger)
    : IRequestHandler<QueryCheckStatus, LanguageExt.Common.Result<InfoDto>>
{
    /// <summary>
    ///     Logger instance for recording database status check operations.
    /// </summary>
    private readonly ILogger _logger =
        logger ?? throw new ArgumentNullException(nameof(logger));

    /// <summary>
    ///     Project configuration options containing version information.
    /// </summary>
    private readonly ProjectOptions _options =
        options.Value ?? throw new ArgumentNullException(nameof(options));

    /// <summary>
    ///     Read-only repository for verifying database connectivity via the <see cref="CurrentVersion" /> entity.
    /// </summary>
    private readonly IGenericDbRepositoryReadContext<AppDbContext, CurrentVersion> _repository =
        repository ?? throw new ArgumentNullException(nameof(repository));

    /// <summary>
    ///     Handles the status check by verifying database connectivity and returning API health information.
    /// </summary>
    /// <param name="request">The MediatR query request to check the database status.</param>
    /// <param name="cancellationToken">A token to observe for cancellation of the asynchronous operation.</param>
    /// <exception cref="ResultFaultedInvalidOperationException">
    ///     Result is not a failure! Use ExtractData method instead and
    ///     Check the state of Result with IsSuccess or IsFaulted before use this method or ExtractData method
    /// </exception>
    public async Task<LanguageExt.Common.Result<InfoDto>> Handle(QueryCheckStatus request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _repository.VerificateAsync(null, cancellationToken).ConfigureAwait(false);
            return result.IsSuccess
                ? new InfoDto
                {
                    StatusCode = StatusCodes.Status200OK,
                    StatusInfo = "API is running fine.",
                    Version = _options.Version
                }
                : new InfoDto
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    StatusInfo = result.ExtractException().Message,
                    Version = _options.Version
                };
        }
        catch (NpgsqlException ex)
        {
            _logger.LogStatusDbFail(ex);
            return new InfoDto
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                StatusInfo = ex.Message,
                Version = _options.Version
            };
        }
    }
}