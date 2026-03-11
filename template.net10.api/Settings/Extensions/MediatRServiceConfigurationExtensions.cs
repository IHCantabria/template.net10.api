using MediatR;
using template.net10.api.Behaviors;
using template.net10.api.Behaviors.Extensions;
using template.net10.api.Features.Extensions;

namespace template.net10.api.Settings.Extensions;

/// <summary>
///     Extension methods for <see cref="MediatRServiceConfiguration"/> to register pipeline behaviors
///     for logging, validation, and post-processing of MediatR requests.
/// </summary>
internal static class MediatRServiceConfigurationExtensions
{
    extension(MediatRServiceConfiguration config)
    {
        /// <summary>
        ///     Registers the open <see cref="LoggingBehavior{TRequest,TResponse}"/> pipeline behavior
        ///     and all configured validation behaviors for MediatR requests.
        /// </summary>
        internal void AddBehaviours()
        {
            config.AddOpenBehavior(typeof(LoggingBehavior<,>));
            config.ConfigureValidations();
        }

        /// <summary>
        ///     Registers a <see cref="ValidationBehavior{TRequest,TResponse}"/> pipeline behavior for requests
        ///     that return <c>Result&lt;TResponse&gt;</c>, enabling automatic FluentValidation execution.
        /// </summary>
        /// <typeparam name="TRequest">The MediatR request type to validate.</typeparam>
        /// <typeparam name="TResponse">The response type wrapped inside <c>LanguageExt.Common.Result&lt;T&gt;</c>.</typeparam>
        internal void AddValidation<TRequest, TResponse>()
            where TRequest : notnull
        {
            config
                .AddBehavior<IPipelineBehavior<TRequest, LanguageExt.Common.Result<TResponse>>,
                    ValidationBehavior<TRequest, TResponse>>();
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