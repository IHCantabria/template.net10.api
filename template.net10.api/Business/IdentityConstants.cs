namespace template.net10.api.Business;

/// <summary>
///     Defines authorization policy name constants used across the application's security configuration.
/// </summary>
internal static class PoliciesConstants
{
    /// <summary>
    ///     Policy name required for general API access.
    /// </summary>
    internal const string ApiAccessPolicy = nameof(ApiAccessPolicy);

    /// <summary>
    ///     Policy name required to read user information.
    /// </summary>
    internal const string UserReadPolicy = nameof(UserReadPolicy);

    /// <summary>
    ///     Policy name required to create new users.
    /// </summary>
    internal const string UserCreationPolicy = nameof(UserCreationPolicy);

    /// <summary>
    ///     Policy name required to reset a user's password.
    /// </summary>
    internal const string UserResetPasswordPolicy = nameof(UserResetPasswordPolicy);

    /// <summary>
    ///     Policy name required to update user details.
    /// </summary>
    internal const string UserUpdatePolicy = nameof(UserUpdatePolicy);

    /// <summary>
    ///     Policy name required to delete a user.
    /// </summary>
    internal const string UserDeletePolicy = nameof(UserDeletePolicy);

    /// <summary>
    ///     Policy name required to disable a user account.
    /// </summary>
    internal const string UserDisablePolicy = nameof(UserDisablePolicy);

    /// <summary>
    ///     Policy name required to enable a user account.
    /// </summary>
    internal const string UserEnablePolicy = nameof(UserEnablePolicy);
}

/// <summary>
///     Defines claim value constants used to match identity claims against authorization policies.
/// </summary>
internal static class ClaimIdentityConstants
{
    /// <summary>
    ///     Claim value granting general API access.
    /// </summary>
    internal const string AccessClaimValue = "ih_template_net_10_access";

    /// <summary>
    ///     Claim value granting permission to read user information.
    /// </summary>
    internal const string UserReadClaimValue = "ih_template_net_10_user_read";

    /// <summary>
    ///     Claim value granting permission to create new users.
    /// </summary>
    internal const string UserCreateClaimValue = "ih_template_net_10_user_create";

    /// <summary>
    ///     Claim value granting permission to reset a user's password.
    /// </summary>
    internal const string UserResetPasswordClaimValue = "ih_template_net_10_user_reset_password";

    /// <summary>
    ///     Claim value granting permission to edit user details.
    /// </summary>
    internal const string UserEditClaimValue = "ih_template_net_10_user_edit";

    /// <summary>
    ///     Claim value granting permission to delete a user.
    /// </summary>
    internal const string UserDeleteClaimValue = "ih_template_net_10_user_delete";

    /// <summary>
    ///     Claim value granting permission to disable a user account.
    /// </summary>
    internal const string UserEditDisableClaimValue = "ih_template_net_10_user_edit_disable";

    /// <summary>
    ///     Claim value granting permission to enable a user account.
    /// </summary>
    internal const string UserEditEnableClaimValue = "ih_template_net_10_user_edit_enable";
}