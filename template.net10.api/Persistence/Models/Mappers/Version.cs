using template.net10.api.Core.DTOs;
using template.net10.api.Core.Interfaces;
using template.net10.api.Domain.DTOs;

namespace template.net10.api.Persistence.Models;

/// <summary>
///     Contains EF Core query projections from <see cref="CurrentVersion"/> to version-related DTOs.
/// </summary>
internal static class CurrentVersionProjections
{
    /// <summary>
    ///     Projection from <see cref="CurrentVersion"/> to <see cref="VersionDto"/>,
    ///     navigating through the related <see cref="Version"/> to populate all version fields.
    /// </summary>
    internal static IProjection<CurrentVersion, VersionDto> VersionProjection =>
        new Projection<CurrentVersion, VersionDto>(static p => new VersionDto
        {
            Id = p.Version.Id,
            Name = p.Version.Name,
            Tag = p.Version.Tag,
            Date = p.Version.Date
        });
}