using System.Text;
using LanguageExt;
using template.net10.api.Core.Exceptions;
using template.net10.api.Core.Extensions;
using template.net10.api.Domain.DTOs;
using template.net10.api.Domain.Password;

namespace template.net10.api.Persistence.Models.Extensions;

/// <summary>
///     C# 13 extension block providing domain-level mutation methods on the <see cref="User"/> entity.
/// </summary>
internal static class UserExtensions
{
    extension(User entity)
    {
        /// <summary>
        ///     Updates the mutable fields of this <see cref="User"/> with non-null values from <paramref name="payload"/>.
        ///     Null fields in the payload are ignored, preserving the existing values.
        /// </summary>
        /// <param name="payload">The DTO carrying the updated field values.</param>
        internal void UpdateUser(CommandUpdateUserParamsDto payload)
        {
            entity.Email = payload.Email ?? entity.Email;
            entity.FirstName = payload.FirstName ?? entity.FirstName;
            entity.LastName = payload.LastName ?? entity.LastName;
            entity.Username = payload.Username ?? entity.Username;
            entity.RoleId = payload.RoleId ?? entity.RoleId;
            entity.IsDisabled = payload.IsDisabled ?? entity.IsDisabled;
        }

        /// <summary>
        ///     Sets <see cref="User.IsDisabled"/> to <see langword="true"/>, preventing the user from authenticating.
        /// </summary>
        internal void DisableUser()
        {
            entity.IsDisabled = true;
        }

        /// <summary>
        ///     Sets <see cref="User.IsDisabled"/> to <see langword="false"/>, re-enabling the user account.
        /// </summary>
        public void EnableUser()
        {
            entity.IsDisabled = false;
        }

        /// <summary>
        ///     Hashes the new password from <paramref name="payload"/> using <see cref="PasswordHasher"/> and updates
        ///     <see cref="User.PasswordHash"/> and <see cref="User.PasswordSalt"/> on the entity.
        /// </summary>
        /// <param name="payload">DTO carrying the new plain-text password.</param>
        /// <param name="pepper">Application-level pepper added to the hash input.</param>
        /// <returns>A <see cref="Try{A}"/> wrapping <see langword="true"/> on success, or the exception on failure.</returns>
        /// <exception cref="ResultFaultedInvalidOperationException">
        ///     Result is not a failure! Use ExtractData method instead and
        ///     Check the state of Result with IsSuccess or IsFaulted before use this method or ExtractData method
        /// </exception>
        /// <exception cref="ResultSuccessInvalidOperationException">
        ///     Result is not a success! Use ExtractException method instead
        ///     and Check the state of Result with IsSuccess or IsFaulted before use this method or ExtractException method
        /// </exception>
        /// <exception cref="EncoderFallbackException">
        ///     A fallback occurred (for more information, see Character Encoding in .NET)
        ///     -and-
        ///     <see cref="EncoderFallback" /> is set to <see cref="EncoderExceptionFallback" />.
        /// </exception>
        internal Try<bool> UpdateUserPassword(CommandResetUserPasswordParamsDto payload,
            string pepper)
        {
            return () =>
            {
                var passwordPayload = new CreateUserPasswordDto
                {
                    Password = payload.Password,
                    Pepper = pepper
                };
                var result = PasswordHasher.HashPasword(passwordPayload).Try();
                if (result.IsFaulted)
                    return new LanguageExt.Common.Result<bool>(result.ExtractException());

                var passwordInfo = result.ExtractData();
                entity.PasswordHash = passwordInfo.Item1;
                entity.PasswordSalt = passwordInfo.Item2;
                return true;
            };
        }
    }
}