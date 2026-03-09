using Microsoft.AspNetCore.Authorization;
using template.net10.api.Business;
using template.net10.api.Core.Authorization;

namespace template.net10.api.Settings.Extensions;

/// <summary>
///     ADD DOCUMENTATION
/// </summary>
internal static class AuthorizationBuilderExtensions
{
    /// <summary>
    ///     ADD DOCUMENTATION
    /// </summary>
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
    ///     ADD DOCUMENTATION
    /// </summary>
    private static List<ClaimRequirement> CreateProductionPolicy(string claimValue)
    {
        return
        [
            new ClaimRequirement(ClaimCoreConstants.ApplicationPrivilegesClaim, claimValue)
        ];
    }

    /// <summary>
    ///     ADD DOCUMENTATION
    /// </summary>
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
        ///     ADD DOCUMENTATION
        /// </summary>
        internal void AddPolicies(bool isProduction)
        {
            var claimLogic = isProduction ? ClaimLogic.All : ClaimLogic.Any;

            foreach (var (policyName, requirements) in GetPolicies(isProduction))
                authorizationBuilder.AddPolicy(policyName,
                    policyOptions => policyOptions.AddRequirements(new ClaimRequirements(requirements, claimLogic)));
        }
    }
}