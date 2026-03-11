using System.Diagnostics.CodeAnalysis;
using template.net10.api.Domain.DTOs;

namespace template.net10.api.Domain.Interfaces;

/// <summary>
///     Defines a contract for types that carry user identity information.
/// </summary>
[SuppressMessage(
    "ReSharper",
    "UnusedMemberInSuper.Global",
    Justification = "Member is part of the abstraction contract and may be consumed polymorphically.")]
internal interface IIdentity
{
    /// <summary>
    ///     Gets or sets the identity information associated with the current request.
    /// </summary>
    IdentityDto Identity { get; set; }
}