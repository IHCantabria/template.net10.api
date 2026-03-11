using Microsoft.AspNetCore.Authorization;
using template.net10.api.Business;
using template.net10.api.Core.Authorization;

namespace template.net10.api.Settings.Extensions;

/// <summary>
///     Extension methods for <see cref="AuthorizationBuilder"/> to register all application authorization policies.
/// </summary>
internal static class AuthorizationBuilderExtensions
{
    /// <summary>
    ///     Builds the full map of policy names to their <see cref="ClaimRequirement"/> lists,
    ///     adapting the strictness of each policy based on the current environment.
    /// </summary>
    /// <param name="isProduction">
    ///     <see langword="true"/> to generate production-grade policies (require application privileges only);
    ///     <see langword="false"/> to use broader development policies that also accept an identity scope claim.
    /// </param>
    /// <returns>A dictionary mapping each policy name to its ordered list of claim requirements.</returns>
    private static Dictionary<string, List<ClaimRequirement>> GetPolicies(bool isProduction)
    {
        return new Dictionary<string, List<ClaimRequirement>>
        {
            [PoliciesConstants.ApiAccessPolicy] = Create(ClaimIdentityConstants.AccessClaimValue),
            [PoliciesConstants.UserReadPolicy] = Create(ClaimIdentityConstants.UserReadClaimValue),
            [PoliciesConstants.UserCreationPolicy] = Create(ClaimIdentityConstants.UserCreateClaimValue),
            [PoliciesConstants.UserResetPasswordPolicy] = Create(ClaimIdentityConstants.UserResetPasswordClaimValue),
            [PoliciesConstants.UserUpdatePolicy] = Create(ClaimIdentityConstants.UserEditClaimValue),
            [PoliciesConstants.UserDeletePolicy] = Create(ClaimIdentityConstants.UserDeleteClaimValue),
            [PoliciesConstants.UserDisablePolicy] = Create(ClaimIdentityConstants.UserEditDisableClaimValue),
            [PoliciesConstants.UserEnablePolicy] = Create(ClaimIdentityConstants.UserEditEnableClaimValue)

            // add here business policies and their requirements
        };

        List<ClaimRequirement> Create(string claimValue)
        {
            return isProduction
                ? CreateProductionPolicy(claimValue)
                : CreateDevelopmentPolicy(claimValue);
        }
    }

    /// <summary>
    ///     Creates an authorization policy for production environments that requires
    ///     only the application-specific privilege claim for the given <paramref name="claimValue"/>.
    /// </summary>
    /// <param name="claimValue">The required value of the application privilege claim.</param>
    /// <returns>A list containing the single production <see cref="ClaimRequirement"/>.</returns>
    private static List<ClaimRequirement> CreateProductionPolicy(string claimValue)
    {
        return
        [
            new ClaimRequirement(ClaimCoreConstants.ApplicationPrivilegesClaim, claimValue)
        ];
    }

    /// <summary>
    ///     Creates an authorization policy for development environments that accepts either
    ///     the identity scope claim or the application privilege claim for the given <paramref name="claimValue"/>.
    /// </summary>
    /// <param name="claimValue">The required value of the application privilege claim.</param>
    /// <returns>A list containing both the scope and privilege <see cref="ClaimRequirement"/> entries.</returns>
    private static List<ClaimRequirement> CreateDevelopmentPolicy(string claimValue)
    {
        return
        [
            new ClaimRequirement(ClaimCoreConstants.ScopeClaim, GenieIdentityConstants.Scope),
            new ClaimRequirement(ClaimCoreConstants.ApplicationPrivilegesClaim, claimValue)
        ];
    }

    extension(AuthorizationBuilder authorizationBuilder)
    {
        /// <summary>
        ///     Registers all application authorization policies on the <see cref="AuthorizationBuilder"/>.
        ///     Uses <see cref="ClaimLogic.All"/> (AND) in production and <see cref="ClaimLogic.Any"/> (OR) in development
        ///     so that non-production environments can work with broader identity tokens.
        /// </summary>
        /// <param name="isProduction">
        ///     <see langword="true"/> to apply strict production policies; <see langword="false"/> for development-friendly policies.
        /// </param>
        internal void AddPolicies(bool isProduction)
        {
            var claimLogic = isProduction ? ClaimLogic.All : ClaimLogic.Any;

            foreach (var (policyName, requirements) in GetPolicies(isProduction))
                authorizationBuilder.AddPolicy(policyName,
                    policyOptions => policyOptions.AddRequirements(new ClaimRequirements(requirements, claimLogic)));
        }
    }
}