using System.Diagnostics.CodeAnalysis;

namespace template.net10.api.Core.Interfaces;

/// <summary>
///     Marker interface for data transfer objects (DTOs). Used for generic type constraints and architectural separation.
/// </summary>
[SuppressMessage("Design",
    "CA1515:Consider making public types internal",
    Justification =
        "The interface is part of the public API contract and must remain publicly accessible.")]
[SuppressMessage(
    "Design",
    "CA1040:Avoid empty interfaces",
    Justification =
        "Marker interface used for generic type constraints and architectural separation of DTO contracts.")]
internal interface IDto;