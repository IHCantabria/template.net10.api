using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using System.Text;
using LanguageExt;
using template.net10.api.Domain.DTOs;

namespace template.net10.api.Domain.Password;

/// <summary>
///     Provides password hashing and verification using PBKDF2 with SHA-512.
/// </summary>
internal static class PasswordHasher
{
    /// <summary>
    ///     The size in bytes of the derived key and salt.
    /// </summary>
    private const short KeySize = 64;

    /// <summary>
    ///     The number of PBKDF2 iterations used for key derivation.
    /// </summary>
    private const int Iterations = 350000;

    /// <summary>
    ///     The hash algorithm used for PBKDF2 key derivation.
    /// </summary>
    private static readonly HashAlgorithmName HashAlgorithm = HashAlgorithmName.SHA512;

    /// <summary>
    ///     Hashes a password using PBKDF2 with a randomly generated salt and the application pepper.
    /// </summary>
    /// <param name="payload">The password and pepper to hash.</param>
    /// <returns>A <see cref="Try{A}" /> containing a tuple of (hash, salt) as hexadecimal strings.</returns>
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
    internal static Try<(string, string)> HashPasword(CreateUserPasswordDto payload)
    {
        return () =>
        {
            var salt = RandomNumberGenerator.GetBytes(KeySize);
            var spicyPassword = $"{payload.Password}{payload.Pepper}";
            var hash = Rfc2898DeriveBytes.Pbkdf2(Encoding.UTF8.GetBytes(spicyPassword), salt, Iterations, HashAlgorithm,
                KeySize);
            return (Convert.ToHexString(hash), Convert.ToHexString(salt));
        };
    }

    /// <summary>
    ///     Verifies a password against a stored hash and salt using PBKDF2 with constant-time comparison.
    /// </summary>
    /// <param name="payload">The password, pepper, salt, and hash to verify against.</param>
    /// <returns>
    ///     A <see cref="Try{A}" /> containing <see langword="true" /> if the password matches; otherwise
    ///     <see langword="false" />.
    /// </returns>
    [SuppressMessage(
        "ReSharper",
        "ExceptionNotDocumented",
        Justification =
            "Potential exceptions originate from underlying implementation details and are not part of the method contract.")]
    [SuppressMessage(
        "ReSharper",
        "ExceptionNotDocumentedOptional",
        Justification =
            "Potential exceptions originate from underlying implementation details and are not part of the method contract.")]
    internal static Try<bool> VerifyPassword(VerifyUserPasswordDto payload)
    {
        return () =>
        {
            var spicyPassword = $"{payload.Password}{payload.Pepper}";
            var saltBytes = SaltHexStringToByteArray(payload.Salt);
            var hashToCompare =
                Rfc2898DeriveBytes.Pbkdf2(spicyPassword, saltBytes, Iterations, HashAlgorithm, KeySize);
            return CryptographicOperations.FixedTimeEquals(hashToCompare,
                Convert.FromHexString(payload.Hash));
        };
    }

    /// <summary>
    ///     Converts a hexadecimal string representation of a salt into a byte array.
    /// </summary>
    /// <param name="hex">The hexadecimal string to convert.</param>
    /// <returns>A byte array decoded from the hex string.</returns>
    private static byte[] SaltHexStringToByteArray(string hex)
    {
        return
        [
            ..Enumerable.Range(0, hex.Length)
                .Where(static x => x % 2 == 0)
                .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
        ];
    }
}