using LanguageExt;
using template.net10.api.Domain.DTOs;

namespace template.net10.api.Domain.Password;

/// <summary>
///     Provides utility methods for user password verification.
/// </summary>
internal static class PasswordUtils
{
    /// <summary>
    ///     Verifies user credentials by comparing the provided password against the stored hash and salt.
    /// </summary>
    /// <param name="payload">The stored user credentials containing the password hash and salt.</param>
    /// <param name="password">The plain-text password to verify.</param>
    /// <param name="pepper">The application-level secret pepper used during hashing.</param>
    /// <returns>A <see cref="Try{A}"/> containing <see langword="true"/> if the password matches; otherwise <see langword="false"/>.</returns>
    internal static Try<bool> VerifyUserCredentials(UserCredentialsDto payload, string password, string pepper)
    {
        return () =>
        {
            var verifyPayload = new VerifyUserPasswordDto
            {
                Password = password,
                Pepper = pepper,
                Salt = payload.PasswordSalt,
                Hash = payload.PasswordHash
            };
            return PasswordHasher.VerifyPassword(verifyPayload).Try();
        };
    }
}