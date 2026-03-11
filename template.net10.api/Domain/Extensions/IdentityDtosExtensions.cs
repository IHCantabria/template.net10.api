using System.Security.Claims;
using template.net10.api.Core.Authorization;
using template.net10.api.Core.Exceptions;
using template.net10.api.Core.Extensions;
using template.net10.api.Core.Interfaces;
using template.net10.api.Domain.DTOs;
using template.net10.api.Domain.Interfaces;

namespace template.net10.api.Domain.Extensions;

/// <summary>
///     Provides extension methods for populating identity information on DTOs that implement <see cref="IIdentity"/>.
/// </summary>
internal static class IdentityExtensions
{
    extension<T>(T dto) where T : IIdentity, IDto
    {
        /// <summary>
        ///     Extracts identity claims from the given <see cref="ClaimsPrincipal"/> and attaches them to the DTO.
        /// </summary>
        /// <param name="claimsPrincipal">The claims principal containing the user's authentication claims.</param>
        /// <returns>The same DTO instance with the <see cref="IIdentity.Identity"/> property populated.</returns>
        /// <exception cref="ResultSuccessInvalidOperationException">
        ///     Result is not a success! Use ExtractException method instead
        ///     and Check the state of Result with IsSuccess or IsFaulted before use this method or ExtractException method
        /// </exception>
        internal T AddIdentifier(ClaimsPrincipal claimsPrincipal)
        {
            var userGuidResult = ClaimUtils.GetUserGuid(claimsPrincipal).Try();
            var userRoleName = ClaimUtils.GetRoleName(claimsPrincipal);
            var userIdentifier = ClaimUtils.GetUserUuid(claimsPrincipal);
            dto.Identity = new IdentityDto(userGuidResult.IsSuccess ? userGuidResult.ExtractData() : null,
                userRoleName, userIdentifier);
            return dto;
        }
    }
}