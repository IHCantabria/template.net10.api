using Microsoft.EntityFrameworkCore;
using template.net10.api.Core.Base;
using template.net10.api.Core.Interfaces;
using template.net10.api.Persistence.Models;

namespace template.net10.api.Domain.Specifications;

/// <summary>
///     ADD DOCUMENTATION
/// </summary>
internal sealed class CurrentVersionReadSpecification : SpecificationBase<CurrentVersion>
{
    /// <summary>
    ///     ADD DOCUMENTATION
    /// </summary>
    internal CurrentVersionReadSpecification()
    {
        AddOrderBy(static cv => cv.VersionId, OrderByType.Desc);
        SetQueryTrackStrategy(QueryTrackingBehavior.NoTracking);
        SetQuerySplitStrategy(QuerySplittingBehavior.SingleQuery);
    }
}