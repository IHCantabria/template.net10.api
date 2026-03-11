using template.net10.api.Core.Contracts;

namespace template.net10.api.Core.DTOs;

internal sealed partial record InfoDto
{
    /// <summary>
    ///     Implicitly converts an <see cref="InfoDto" /> to an <see cref="InfoResource" />.
    /// </summary>
    /// <param name="dto">The DTO to convert.</param>
    /// <returns>A new <see cref="InfoResource" /> mapped from the DTO properties.</returns>
    public static implicit operator InfoResource(InfoDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        return new InfoResource
        {
            StatusInfo = dto.StatusInfo,
            Version = dto.Version,
            StatusCode = dto.StatusCode
        };
    }
}