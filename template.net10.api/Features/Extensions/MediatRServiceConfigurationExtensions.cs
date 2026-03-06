using template.net10.api.Domain.DTOs;
using template.net10.api.Features.Commands;
using template.net10.api.Features.Querys;
using template.net10.api.Persistence.Models;
using template.net10.api.Settings.Extensions;

namespace template.net10.api.Features.Extensions;

/// <summary>
///     ADD DOCUMENTATION
/// </summary>
internal static class MediatRServiceConfigurationExtensions
{
    extension(MediatRServiceConfiguration config)
    {
        /// <summary>
        ///     ADD DOCUMENTATION
        /// </summary>
        internal void ConfigureValidations()
        {
            config.AddValidation<CommandCreateUser, User>();
            config.AddValidation<CommandDeleteUser, User>();
            config.AddValidation<CommandUpdateUser, User>();
            config.AddValidation<CommandDisableUser, User>();
            config.AddValidation<CommandEnableUser, User>();
            config.AddValidation<CommandResetUserPassword, UserResetedPasswordDto>();

            config.AddValidation<QueryAccessUser, AccessTokenDto>();
            config.AddValidation<QueryGetUser, UserDto>();
            config.AddValidation<QueryGetUsers, IEnumerable<UserDto>>();
            config.AddValidation<QueryLoginUser, IdTokenDto>();
        }
    }
}