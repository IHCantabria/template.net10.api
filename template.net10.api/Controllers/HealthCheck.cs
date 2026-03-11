using System.Diagnostics.CodeAnalysis;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using template.net10.api.Controllers.Extensions;
using template.net10.api.Core.Contracts;
using template.net10.api.Core.DTOs;
using template.net10.api.Features.Querys;
using template.net10.api.Localize.Resources;
using template.net10.api.Settings.Attributes;

namespace template.net10.api.Controllers;

/// <summary>
///     API controller providing health check endpoints for monitoring system availability.
/// </summary>
[SuppressMessage("Design",
    "CA1515:Consider making public types internal",
    Justification =
        "Controllers must remain public to allow OpenAPI discovery and correct API exposure.")]
[Route(ApiRoutes.HealthController.PathController)]
[ApiController]
[DevSwagger]
public sealed class Health(
    IMediator mediator,
    IStringLocalizer<ResourceMain> localizer,
    ILogger<Health> logger)
    : MyControllerBase(mediator, localizer, logger)
{
    /// <summary>
    ///     Performs a health check of the system and returns current status information.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>An <see cref="IActionResult"/> containing the system health status.</returns>
    [HttpGet]
    [Route(ApiRoutes.HealthController.HealthCheck)]
    public async Task<IActionResult> HealthCheckAsync(CancellationToken cancellationToken)
    {
        var query = new QueryCheckStatus();
        var result = await Mediator.Send(query, cancellationToken).ConfigureAwait(false);
        var action = ActionResultPayload<InfoDto, InfoResource>.Ok(static obj => obj);
        return result.ToActionResult(this, action, Localizer);
    }
}