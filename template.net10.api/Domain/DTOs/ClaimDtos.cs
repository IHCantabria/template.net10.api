using System.Numerics;
using template.net10.api.Core.Authorization.DTOs.Base;

namespace template.net10.api.Domain.DTOs;

/// <summary>
///     ADD DOCUMENTATION
/// </summary>
internal sealed record ClaimDto(short Id, string Name)
    : ClaimBaseDto(Id, Name), IEqualityOperators<ClaimDto, ClaimDto, bool>;