using template.net10.api.Behaviors.Users;

namespace template.net10.api.Behaviors.Extensions;

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
        internal void ConfigurePostProcesses()
        {
            config.AddRequestPostProcessor<CreateUserProcessor>();
            config.AddRequestPostProcessor<UpdateUserProcessor>();
            config.AddRequestPostProcessor<DeleteUserProcessor>();
        }
    }
}