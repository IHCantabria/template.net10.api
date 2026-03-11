using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LanguageExt;
using Microsoft.IdentityModel.Tokens;
using template.net10.api.Core.Authorization;
using template.net10.api.Core.Authorization.DTOs.Base;
using template.net10.api.Core.Exceptions;
using template.net10.api.Core.Extensions;
using template.net10.api.Domain.DTOs;
using template.net10.api.Settings;
using template.net10.api.Settings.Options;

namespace template.net10.api.Domain.Factory;

/// <summary>
///     Factory responsible for generating JWT identity, access, and genie tokens.
/// </summary>
internal static class TokenFactory
{
    /// <summary>
    ///     Generates an ID token DTO containing a signed JWT for the specified user.
    /// </summary>
    /// <param name="user">The user data to include as claims in the ID token.</param>
    /// <param name="jwtConfig">The JWT configuration options.</param>
    /// <param name="appConfig">The application configuration options.</param>
    /// <returns>An <see cref="IdTokenDto" /> containing the signed ID token string.</returns>
    internal static IdTokenDto GenerateIdTokenDto(UserIdTokenBaseDto user, JwtOptions jwtConfig, AppOptions appConfig)
    {
        return new IdTokenDto
        {
            IdToken = GenerateIdToken(user, jwtConfig, appConfig)
        };
    }

    /// <summary>
    ///     Generates an access token DTO containing signed access and refresh JWT tokens for the specified user.
    /// </summary>
    /// <param name="user">The user data to include as claims in the access token.</param>
    /// <param name="jwtConfig">The JWT configuration options.</param>
    /// <param name="appConfig">The application configuration options.</param>
    /// <returns>A <see cref="Try{A}" /> containing an <see cref="AccessTokenDto" /> with both access and refresh tokens.</returns>
    internal static Try<AccessTokenDto> GenerateAccessTokenDto(UserAccessTokenBaseDto user, JwtOptions jwtConfig,
        AppOptions appConfig)
    {
        return () => new AccessTokenDto
        {
            AccessToken = GenerateAccessToken(user, jwtConfig, appConfig),
            RefreshToken = GenerateAccessToken(user, jwtConfig, appConfig)
        };
    }

    /// <summary>
    ///     Generates an access token DTO for the Genie (system) user using predefined claims.
    /// </summary>
    /// <param name="jwtConfig">The JWT configuration options.</param>
    /// <param name="appConfig">The application configuration options.</param>
    /// <returns>A <see cref="Try{A}" /> containing an <see cref="AccessTokenDto" /> with the Genie access token.</returns>
    /// <exception cref="ResultFaultedInvalidOperationException">
    ///     Result is not a failure! Use ExtractData method instead and
    ///     Check the state of Result with IsSuccess or IsFaulted before use this method or ExtractData method
    /// </exception>
    /// <exception cref="ResultSuccessInvalidOperationException">
    ///     Result is not a success! Use ExtractException method instead
    ///     and Check the state of Result with IsSuccess or IsFaulted before use this method or ExtractException method
    /// </exception>
    /// <exception cref="ArgumentNullException">if 'key' is null.</exception>
    /// <exception cref="ArgumentException">If 'expires' &lt;= 'notbefore'.</exception>
    /// <exception cref="EncoderFallbackException">
    ///     A fallback occurred (for more information, see Character Encoding in .NET)
    ///     -and-
    ///     <see cref="EncoderFallback" /> is set to <see cref="EncoderExceptionFallback" />.
    /// </exception>
    internal static Try<AccessTokenDto> GenerateGenieAccessTokenDto(JwtOptions jwtConfig, AppOptions appConfig)
    {
        return () =>
        {
            var result = GenerateGenieAccessToken(jwtConfig, appConfig).Try();
            if (result.IsFaulted)
                return new LanguageExt.Common.Result<AccessTokenDto>(result.ExtractException());

            var accessToken = result.ExtractData();
            return new AccessTokenDto
            {
                AccessToken = accessToken,
                RefreshToken = accessToken
            };
        };
    }

    /// <summary>
    ///     Generates a signed JWT ID token string for the specified user.
    /// </summary>
    /// <param name="user">The user data to embed as claims.</param>
    /// <param name="jwtConfig">The JWT configuration options.</param>
    /// <param name="appConfig">The application configuration options.</param>
    /// <returns>A signed JWT token string.</returns>
    private static string GenerateIdToken(UserIdTokenBaseDto user, JwtOptions jwtConfig, AppOptions appConfig)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.Secret));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var exp = GetExpirationDateTime(jwtConfig.TokenLifetime, appConfig.Env);
        var claims = AddUserClaims(user);
        var token = new JwtSecurityToken(jwtConfig.Issuer, jwtConfig.Audience, claims, null, exp, credentials);
        var tokenHandler = new JwtSecurityTokenHandler();
        return tokenHandler.WriteToken(token);
    }

    /// <summary>
    ///     Generates a signed JWT access token string for the specified user.
    /// </summary>
    /// <param name="user">The user data to embed as claims.</param>
    /// <param name="jwtConfig">The JWT configuration options.</param>
    /// <param name="appConfig">The application configuration options.</param>
    /// <returns>A signed JWT token string.</returns>
    private static string GenerateAccessToken(UserAccessTokenBaseDto user, JwtOptions jwtConfig, AppOptions appConfig)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.Secret));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var claims = AddUserClaims(user);
        var exp = GetExpirationDateTime(jwtConfig.TokenLifetime, appConfig.Env);
        var token = new JwtSecurityToken(jwtConfig.Issuer, jwtConfig.Audience, claims, null, exp, credentials);
        var tokenHandler = new JwtSecurityTokenHandler();
        return tokenHandler.WriteToken(token);
    }

    /// <summary>
    ///     Builds the list of JWT claims for an ID token from the specified user data.
    /// </summary>
    /// <param name="user">The user data to convert into claims.</param>
    /// <returns>A list of <see cref="Claim" /> instances representing the user's ID token claims.</returns>
    [SuppressMessage(
        "ReSharper",
        "ReturnTypeCanBeEnumerable.Local",
        Justification =
            "Concrete return type is intentional for performance and to avoid interface-based enumeration overhead.")]
    private static List<Claim> AddUserClaims(UserIdTokenBaseDto user)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Uuid.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(JwtRegisteredClaimNames.Name, user.Username),
            new(ClaimCoreConstants.TokenTypeClaim, TokenTypesIdentityConstants.IdTokenType),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
        claims.AddClaimIfNotNull(ClaimCoreConstants.RoleClaim, user.RoleName);
        claims.AddClaimIfNotNull(JwtRegisteredClaimNames.FamilyName, user.LastName);
        claims.AddClaimIfNotNull(JwtRegisteredClaimNames.GivenName, user.FirstName);
        return claims;
    }

    /// <summary>
    ///     Builds the list of JWT claims for an access token from the specified user data, including application privileges.
    /// </summary>
    /// <param name="user">The user data to convert into claims.</param>
    /// <returns>A list of <see cref="Claim" /> instances representing the user's access token claims.</returns>
    [SuppressMessage(
        "ReSharper",
        "ReturnTypeCanBeEnumerable.Local",
        Justification =
            "Concrete return type is intentional for performance and to avoid interface-based enumeration overhead.")]
    private static List<Claim> AddUserClaims(UserAccessTokenBaseDto user)
    {
        var claims = new Collection<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Uuid.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(JwtRegisteredClaimNames.Name, user.Username),
            new(ClaimCoreConstants.TokenTypeClaim, TokenTypesIdentityConstants.AccessTokenType),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
        claims.AddClaimIfNotNull(ClaimCoreConstants.RoleClaim, user.RoleName);
        claims.AddClaimIfNotNull(JwtRegisteredClaimNames.FamilyName, user.LastName);
        claims.AddClaimIfNotNull(JwtRegisteredClaimNames.GivenName, user.FirstName);
        return
        [
            ..claims.Append(AddApplicationPrivileges(user.RoleClaims.Select(static rc => rc.Name)
                .Concat(user.UserClaims.Select(static uc => uc.Name)).Distinct()))
        ];
    }

    /// <summary>
    ///     Generates a signed JWT access token string for the Genie (system) user.
    /// </summary>
    /// <param name="jwtConfig">The JWT configuration options.</param>
    /// <param name="appConfig">The application configuration options.</param>
    /// <returns>A <see cref="Try{A}" /> containing the signed JWT token string.</returns>
    /// <exception cref="ArgumentNullException">if 'key' is null.</exception>
    /// <exception cref="ArgumentException">If 'expires' &lt;= 'notbefore'.</exception>
    /// <exception cref="EncoderFallbackException">
    ///     A fallback occurred (for more information, see Character Encoding in .NET)
    ///     -and-
    ///     <see cref="EncoderFallback" /> is set to <see cref="EncoderExceptionFallback" />.
    /// </exception>
    [SuppressMessage(
        "ReSharper",
        "ExceptionNotDocumentedOptional",
        Justification =
            "Potential exceptions originate from underlying implementation details and are not part of the method contract.")]
    internal static Try<string> GenerateGenieAccessToken(JwtOptions jwtConfig, AppOptions appConfig)
    {
        return () =>
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.Secret));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var exp = GetExpirationDateTime(jwtConfig.TokenLifetime, appConfig.Env);
            var token = new JwtSecurityToken(jwtConfig.Issuer, jwtConfig.Audience, GenerateGenieClaims(), null, exp,
                credentials);
            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(token);
        };
    }

    /// <summary>
    ///     Generates the predefined set of JWT claims for the Genie (system) user.
    /// </summary>
    /// <returns>A list of <see cref="Claim" /> instances representing the Genie identity.</returns>
    [SuppressMessage(
        "ReSharper",
        "ReturnTypeCanBeEnumerable.Local",
        Justification =
            "Concrete return type is intentional for performance and to avoid interface-based enumeration overhead.")]
    private static List<Claim> GenerateGenieClaims()
    {
        return
        [
            new Claim(ClaimCoreConstants.RoleClaim, GenieIdentityConstants.RoleName),
            new Claim(JwtRegisteredClaimNames.Sub, GenieIdentityConstants.Identifier),
            new Claim(JwtRegisteredClaimNames.Email, GenieIdentityConstants.Email),
            new Claim(JwtRegisteredClaimNames.Name, GenieIdentityConstants.UserName),
            new Claim(JwtRegisteredClaimNames.FamilyName, GenieIdentityConstants.LastName),
            new Claim(JwtRegisteredClaimNames.GivenName, GenieIdentityConstants.FirsName),
            new Claim(ClaimCoreConstants.TokenTypeClaim, TokenTypesIdentityConstants.AccessTokenType),
            new Claim(ClaimCoreConstants.ScopeClaim, GenieIdentityConstants.Scope),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        ];
    }

    /// <summary>
    ///     Converts a collection of privilege names into application-privilege claims.
    /// </summary>
    /// <param name="privileges">The privilege names to include as claims.</param>
    /// <returns>A list of <see cref="Claim" /> instances, one per privilege.</returns>
    [SuppressMessage(
        "ReSharper",
        "ReturnTypeCanBeEnumerable.Local",
        Justification =
            "Concrete return type is intentional for performance and to avoid interface-based enumeration overhead.")]
    private static List<Claim> AddApplicationPrivileges(IEnumerable<string> privileges)
    {
        return
        [
            ..privileges
                .Select(static privilege => new Claim(ClaimCoreConstants.ApplicationPrivilegesClaim, privilege))
        ];
    }

    /// <summary>
    ///     Calculates the token expiration date. In non-production environments or when no lifetime is configured, returns the
    ///     maximum allowed JWT date.
    /// </summary>
    /// <param name="tokenLifetime">The optional token lifetime duration.</param>
    /// <param name="env">The current application environment name.</param>
    /// <returns>The UTC expiration <see cref="DateTime" /> for the token.</returns>
    private static DateTime GetExpirationDateTime(TimeSpan? tokenLifetime, string env)
    {
        var isDev = env is not Envs.Production;
        if (isDev || tokenLifetime is null)
            return new DateTimeOffset(2038, 1, 19, 3, 14, 7, TimeSpan.Zero)
                .UtcDateTime; // Max duration allowed for JWT token

        return DateTime.UtcNow + (TimeSpan)tokenLifetime;
    }

    extension(ICollection<Claim> claims)
    {
        /// <summary>
        ///     Adds a claim to the collection only if the provided value is not <see langword="null" />.
        /// </summary>
        /// <param name="claimType">The claim type identifier.</param>
        /// <param name="value">The claim value. If <see langword="null" />, the claim is not added.</param>
        private void AddClaimIfNotNull(string claimType, string? value)
        {
            if (value is not null) claims.Add(new Claim(claimType, value));
        }
    }
}