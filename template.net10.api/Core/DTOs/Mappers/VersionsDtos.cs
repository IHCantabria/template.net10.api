using template.net10.api.Core.Contracts;

namespace template.net10.api.Core.DTOs;

internal sealed partial record VersionDto
{
    /// <summary>
    ///     Implicitly converts a <see cref="VersionDto"/> to a <see cref="VersionResource"/>.
    /// </summary>
    /// <param name="dto">The DTO to convert.</param>
    /// <returns>A new <see cref="VersionResource"/> mapped from the DTO properties.</returns>
    public static implicit operator VersionResource(VersionDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        return new VersionResource
        {
            Id = dto.Id,
            Name = dto.Name,
            Tag = dto.Tag,
            Date = dto.Date.UtcDateTime
        };
    }
}