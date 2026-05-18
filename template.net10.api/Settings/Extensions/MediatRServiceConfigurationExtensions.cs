using template.net10.api.Behaviors;
using template.net10.api.Behaviors.Extensions;

namespace template.net10.api.Settings.Extensions;

/// <summary>
///     Extension methods for <see cref="MediatRServiceConfiguration" /> to register pipeline behaviors
///     for logging, validation, and post-processing of MediatR requests.
/// </summary>
internal static class MediatRServiceConfigurationExtensions
{
    extension(MediatRServiceConfiguration config)
    {
        /// <summary>
        ///     Registers the open <see cref="LoggingBehavior{TRequest,TResponse}" /> pipeline behavior
        ///     and all configured validation behaviors for MediatR requests.
        /// </summary>
        internal void AddBehaviours()
        {
            config.ConfigureBehavior();
        }

        /// <summary>
        ///     Configures all registered MediatR post-processing pipeline behaviors.
        /// </summary>
        internal void AddPostProcesses()
        {
            config.ConfigurePostProcesses();
        }
    }
}