using System.Diagnostics.CodeAnalysis;

namespace template.net10.api.Settings.Interfaces;

/// <summary>
///     Options contract for HTTP client configurations. Extends <see cref="IConnectionsOptions"/> with
///     the base URI and request timeout required to configure typed <see cref="System.Net.Http.HttpClient"/> instances.
/// </summary>
[SuppressMessage(
    "ReSharper",
    "UnusedType.Global",
    Justification = "Reusable configuration contract; it may not be used in every application scenario.")]
[SuppressMessage(
    "ReSharper",
    "UnusedMember.Global",
    Justification =
        "Members are part of the reusable configuration contract and may not be used in all implementations.")]
internal interface IHttpClientsOptions : IConnectionsOptions
{
    /// <summary>
    ///     The base URI of the remote service that the HTTP client will connect to.
    /// </summary>
    Uri UriBase { get; init; }

    /// <summary>
    ///     The maximum duration to wait for a response before the HTTP client times out.
    /// </summary>
    TimeSpan Timeout { get; init; }
}