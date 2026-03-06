using MediatR;
using template.net10.api.Behaviors;
using template.net10.api.Behaviors.Extensions;
using template.net10.api.Features.Extensions;

namespace template.net10.api.Settings.Extensions;

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
        internal void AddBehaviours()
        {
            config.AddOpenBehavior(typeof(LoggingBehavior<,>));
            config.ConfigureValidations();
        }

        /// <summary>
        ///     ADD DOCUMENTATION
        /// </summary>
        internal void AddValidation<TRequest, TResponse>()
            where TRequest : notnull
        {
            config
                .AddBehavior<IPipelineBehavior<TRequest, LanguageExt.Common.Result<TResponse>>,
                    ValidationBehavior<TRequest, TResponse>>();
        }

        /// <summary>
        ///     ADD DOCUMENTATION
        /// </summary>
        internal void AddPostProcesses()
        {
            config.ConfigurePostProcesses();
        }
    }
}