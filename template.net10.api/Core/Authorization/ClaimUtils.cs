using System.Security.Claims;
using LanguageExt;
using template.net10.api.Core.Exceptions;

namespace template.net10.api.Core.Authorization;

/// <summary>
///     Provides utility methods for extracting claims from a claims principal.
/// </summary>
internal static class ClaimUtils
{
    /// <summary>
    ///     Extracts the user's unique identifier as a <see cref="Guid"/> from the claims principal.
    /// </summary>
    /// <param name="claimsPrincipal">The claims principal to extract the identifier from.</param>
    /// <returns>A <see cref="Try{A}"/> containing the parsed <see cref="Guid"/>, or an <see cref="UnauthorizedException"/> if absent or invalid.</returns>
    internal static Try<Guid> GetUserGuid(ClaimsPrincipal claimsPrincipal)
    {
        return () =>
        {
            var stringUuid = claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = Guid.TryParse(stringUuid, out var uuid);
            return result
                ? uuid
                : new LanguageExt.Common.Result<Guid>(new UnauthorizedException(
                    "You dont have a valid uuid in your Access Token"));
        };
    }

    /// <summary>
    ///     Extracts the role name from the claims principal.
    /// </summary>
    /// <param name="claimsPrincipal">The claims principal to extract the role from.</param>
    /// <returns>The role name claim value, or <see langword="null"/> if not present.</returns>
    internal static string? GetRoleName(ClaimsPrincipal claimsPrincipal)
    {
        return claimsPrincipal.FindFirstValue(ClaimCoreConstants.RoleClaim);
    }

    /// <summary>
    ///     Extracts the user's unique identifier as a string from the claims principal.
    /// </summary>
    /// <param name="claimsPrincipal">The claims principal to extract the identifier from.</param>
    /// <returns>The NameIdentifier claim value as a string, or <see langword="null"/> if not present.</returns>
    internal static string? GetUserUuid(ClaimsPrincipal claimsPrincipal)
    {
        return claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
    }
}