using template.net10.api.Behaviors.Users;

namespace template.net10.api.Behaviors.Extensions;

/// <summary>
///     Provides extension methods for <see cref="MediatRServiceConfiguration"/> to register post-processing behaviors.
/// </summary>
internal static class MediatRServiceConfigurationExtensions
{
    extension(MediatRServiceConfiguration config)
    {
        /// <summary>
        ///     Registers all user-related MediatR post-processors for create, update, and delete operations.
        /// </summary>
        internal void ConfigurePostProcesses()
        {
            config.AddRequestPostProcessor<CreateUserProcessor>();
            config.AddRequestPostProcessor<UpdateUserProcessor>();
            config.AddRequestPostProcessor<DeleteUserProcessor>();
        }
    }
}