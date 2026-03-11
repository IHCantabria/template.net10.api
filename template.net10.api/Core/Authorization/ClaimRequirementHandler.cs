using System.Numerics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace template.net10.api.Core.Authorization;

/// <summary>
///     Handles authorization requirements based on claims present in the user's principal.
/// </summary>
internal sealed class ClaimRequirementHandler : AuthorizationHandler<ClaimRequirements>
{
    /// <summary>
    ///     Evaluates the authorization context against the specified claim requirements.
    /// </summary>
    /// <param name="context">The authorization handler context containing the user principal.</param>
    /// <param name="requirement">The claim requirements to evaluate against the principal.</param>
    /// <returns>A completed <see cref="Task"/>.</returns>
    /// <exception cref="ArgumentNullException">
    ///     <paramref name="context" /> is <see langword="null" />.
    ///     -or-
    ///     <paramref name="requirement" /> is <see langword="null" />.
    /// </exception>
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ClaimRequirements requirement)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(requirement);
        if (HasRequiredClaims(context.User, requirement))
            context.Succeed(requirement);
        return Task.CompletedTask;
    }

    /// <summary>
    ///     Determines whether the claims principal satisfies all or any of the required claims based on the claim logic.
    /// </summary>
    /// <param name="principal">The claims principal to evaluate.</param>
    /// <param name="requirements">The claim requirements to check.</param>
    /// <returns><see langword="true"/> if the principal satisfies the requirements; otherwise, <see langword="false"/>.</returns>
    private static bool HasRequiredClaims(ClaimsPrincipal principal, ClaimRequirements requirements)
    {
        return requirements.ClaimLogic switch
        {
            ClaimLogic.All => requirements.ClaimRequirementsCollection.All(claimRequirement =>
                HasRequiredClaim(principal.Claims, claimRequirement)),
            ClaimLogic.Any => requirements.ClaimRequirementsCollection.Any(claimRequirement =>
                HasRequiredClaim(principal.Claims, claimRequirement)),
            _ => false
        };
    }

    /// <summary>
    ///     Checks whether the collection of claims contains a claim matching the specified type and value.
    /// </summary>
    /// <param name="claims">The collection of claims to search.</param>
    /// <param name="requirement">The individual claim requirement to match.</param>
    /// <returns><see langword="true"/> if a matching claim is found; otherwise, <see langword="false"/>.</returns>
    private static bool HasRequiredClaim(IEnumerable<Claim> claims, ClaimRequirement requirement)
    {
        return claims.Any(c =>
            c.Type == requirement.ClaimType &&
            c.Value.Contains(requirement.ClaimValue, StringComparison.InvariantCultureIgnoreCase));
    }
}

/// <summary>
///     Represents a set of claim requirements with a logical operator for authorization evaluation.
/// </summary>
internal sealed record ClaimRequirements(
    IEnumerable<ClaimRequirement> ClaimRequirementsCollection,
    ClaimLogic ClaimLogic = ClaimLogic.All)
    : IAuthorizationRequirement, IEqualityOperators<ClaimRequirements, ClaimRequirements, bool>
{
    /// <summary>
    ///     Gets the collection of individual claim requirements to evaluate.
    /// </summary>
    public IEnumerable<ClaimRequirement> ClaimRequirementsCollection { get; } = ClaimRequirementsCollection;

    /// <summary>
    ///     Gets the logical operator used to evaluate the claim requirements (All or Any).
    /// </summary>
    public ClaimLogic ClaimLogic { get; } = ClaimLogic;
}

/// <summary>
///     Represents a single claim requirement with a type and expected value.
/// </summary>
internal sealed record ClaimRequirement(string ClaimType, string ClaimValue)
    : IEqualityOperators<ClaimRequirement, ClaimRequirement, bool>
{
    /// <summary>
    ///     Gets the claim type to match against.
    /// </summary>
    public string ClaimType { get; } = ClaimType;

    /// <summary>
    ///     Gets the expected claim value to match against.
    /// </summary>
    public string ClaimValue { get; } = ClaimValue;
}

/// <summary>
///     Defines the logical operator for combining multiple claim requirements.
/// </summary>
internal enum ClaimLogic
{
    /// <summary>
    ///     All claim requirements must be satisfied.
    /// </summary>
    All = 0,

    /// <summary>
    ///     At least one claim requirement must be satisfied.
    /// </summary>
    Any = 1
}