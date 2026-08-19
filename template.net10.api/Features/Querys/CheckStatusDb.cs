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
    IOptions<AppOptions> appOptions,
    ILogger<QueryCheckStatusHandler> logger)
    : IRequestHandler<QueryCheckStatus, LanguageExt.Common.Result<InfoDto>>
{
    /// <summary>
    ///     Status label reported when the API and its database are responding correctly.
    /// </summary>
    private const string HealthyStatusInfo = "OK";

    /// <summary>
    ///     Application configuration options containing the current deployment environment name.
    /// </summary>
    private readonly AppOptions _appOptions =
        appOptions.Value ?? throw new ArgumentNullException(nameof(appOptions));

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
                ? BuildInfo(StatusCodes.Status200OK, HealthyStatusInfo)
                : BuildInfo(StatusCodes.Status500InternalServerError, result.ExtractException().Message);
        }
        catch (NpgsqlException ex)
        {
            _logger.LogStatusDbFail(ex);
            return BuildInfo(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    /// <summary>
    ///     Normalizes the configured version by dropping the optional leading <c>v</c> prefix (<c>v1.2.3</c> becomes
    ///     <c>1.2.3</c>), so every API of the platform reports the version in the same shape.
    /// </summary>
    /// <param name="version">The raw version string read from configuration.</param>
    /// <returns>The normalized version string.</returns>
    private static string NormalizeVersion(string version)
    {
        var trimmed = version.AsSpan().Trim();
        return trimmed.Length > 1 && trimmed[0] is 'v' or 'V' && char.IsAsciiDigit(trimmed[1])
            ? trimmed[1..].ToString()
            : trimmed.ToString();
    }

    /// <summary>
    ///     Builds the health check payload, attaching the version and environment metadata shared by every response.
    /// </summary>
    /// <param name="status">The HTTP status code describing the current API state.</param>
    /// <param name="statusInfo">The human-readable label describing the current API state.</param>
    /// <returns>The <see cref="InfoDto" /> exposed by the health check endpoint.</returns>
    private InfoDto BuildInfo(short status, string statusInfo)
    {
        return new InfoDto
        {
            Status = status,
            StatusInfo = statusInfo,
            Version = NormalizeVersion(_options.Version),
            Environment = _appOptions.Env.ToUpperInvariant()
        };
    }
}