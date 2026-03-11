namespace template.net10.api.Core.Authorization;

/// <summary>
///     Contains constants for token type identifiers.
/// </summary>
internal static class TokenTypesIdentityConstants
{
    /// <summary>
    ///     The identifier for an ID token type.
    /// </summary>
    internal const string IdTokenType = "id_token";

    /// <summary>
    ///     The identifier for an access token type.
    /// </summary>
    internal const string AccessTokenType = "access_token";
}

/// <summary>
///     Contains default identity constants for the system genie user.
/// </summary>
internal static class GenieIdentityConstants
{
    /// <summary>
    ///     The default username for the genie user.
    /// </summary>
    internal const string UserName = "genio";

    /// <summary>
    ///     The default first name for the genie user.
    /// </summary>
    internal const string FirsName = "will";

    /// <summary>
    ///     The default last name for the genie user.
    /// </summary>
    internal const string LastName = "smith";

    /// <summary>
    ///     The default email placeholder for the genie user.
    /// </summary>
    internal const string Email = "un_genio_no_necesita_email";

    /// <summary>
    ///     The default role name for the genie user.
    /// </summary>
    internal const string RoleName = "el_genio_de_la_lampara";

    /// <summary>
    ///     The default unique identifier for the genie user.
    /// </summary>
    internal const string Identifier = "no_hay_un_genio_tan_genial";

    /// <summary>
    ///     The default scope for the genie user.
    /// </summary>
    internal const string Scope = "un_espacio_chiquitin_para_vivir";
}

/// <summary>
///     Contains constants for core authorization claim types.
/// </summary>
internal static class ClaimCoreConstants
{
    /// <summary>
    ///     The claim type name for user roles.
    /// </summary>
    internal const string RoleClaim = "role";

    /// <summary>
    ///     The claim type name for scopes.
    /// </summary>
    internal const string ScopeClaim = "scope";

    /// <summary>
    ///     The claim type name for token types.
    /// </summary>
    internal const string TokenTypeClaim = "token_type";

    /// <summary>
    ///     The claim type name for application privileges.
    /// </summary>
    internal const string ApplicationPrivilegesClaim = "application_privileges";
}