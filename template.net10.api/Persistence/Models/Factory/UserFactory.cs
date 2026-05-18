using System.Text;
using LanguageExt;
using template.net10.api.Core.Exceptions;
using template.net10.api.Core.Extensions;
using template.net10.api.Domain.DTOs;
using template.net10.api.Domain.Password;

namespace template.net10.api.Persistence.Models.Factory;

/// <summary>
///     Factory responsible for creating user-related domain objects.
/// </summary>
internal static class UserFactory
{
    /// <summary>
    ///     Creates a new user DTO by hashing the provided password and generating a unique identifier.
    /// </summary>
    /// <param name="payload">The command parameters containing the new user's details.</param>
    /// <param name="pepper">The application-level secret pepper used during password hashing.</param>
    /// <returns>
    ///     A <see cref="Try{A}" /> containing the created <see cref="User" /> on success, or the hashing
    ///     exception on failure.
    /// </returns>
    /// <exception cref="ResultSuccessInvalidOperationException">
    ///     Result is not a success! Use ExtractException method instead
    ///     and Check the state of Result with IsSuccess or IsFaulted before use this method or ExtractException method
    /// </exception>
    /// <exception cref="ResultFaultedInvalidOperationException">
    ///     Result is not a failure! Use ExtractData method instead and
    ///     Check the state of Result with IsSuccess or IsFaulted before use this method or ExtractData method
    /// </exception>
    /// <exception cref="EncoderFallbackException">
    ///     A fallback occurred (for more information, see Character Encoding in .NET)
    ///     -and-
    ///     <see cref="EncoderFallback" /> is set to <see cref="EncoderExceptionFallback" />.
    /// </exception>
    internal static Try<User> CreateUser(CommandCreateUserParamsDto payload,
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
                return new LanguageExt.Common.Result<User>(result.ExtractException());

            var passwordInfo = result.ExtractData();
            var utcDate = DateTime.SpecifyKind(DateTime.UtcNow,
                DateTimeKind.Unspecified);
            return new User
            {
                Username = payload.Username,
                Email = payload.Email,
                IsDisabled = payload.IsDisabled,
                RoleId = payload.RoleId,
                FirstName = payload.FirstName,
                LastName = payload.LastName,
                InsertDatetime = utcDate,
                PasswordHash = passwordInfo.Item1,
                PasswordSalt = passwordInfo.Item2,
                Uuid = Guid.NewGuid(),
                InsertUserId = payload.Identity.UserInternalIdentifier,
                UpdateDatetime = utcDate,
                UpdateUserId = payload.Identity.UserInternalIdentifier,
                Id = 0,
                InsertUser = null,
                InverseInsertUser = [],
                InverseUpdateUser = [],
                Role = null,
                UpdateUser = null,
                Claims = []
            };
        };
    }
}