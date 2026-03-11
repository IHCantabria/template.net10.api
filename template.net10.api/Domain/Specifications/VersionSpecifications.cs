using Microsoft.EntityFrameworkCore;
using template.net10.api.Core.Base;
using template.net10.api.Core.Interfaces;
using template.net10.api.Persistence.Models;

namespace template.net10.api.Domain.Specifications;

/// <summary>
///     Read-only specification that retrieves the current application version, ordered descending by version identifier.
/// </summary>
internal sealed class CurrentVersionReadSpecification : SpecificationBase<CurrentVersion>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="CurrentVersionReadSpecification"/> class with descending version order and no-tracking behavior.
    /// </summary>
    internal CurrentVersionReadSpecification()
    {
        AddOrderBy(static cv => cv.VersionId, OrderByType.Desc);
        SetQueryTrackStrategy(QueryTrackingBehavior.NoTracking);
        SetQuerySplitStrategy(QuerySplittingBehavior.SingleQuery);
    }
}