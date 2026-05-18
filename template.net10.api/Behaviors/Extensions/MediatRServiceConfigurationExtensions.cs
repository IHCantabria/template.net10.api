using MediatR;
using template.net10.api.Behaviors.Users;
using template.net10.api.Features.Extensions;

namespace template.net10.api.Behaviors.Extensions;

/// <summary>
///     Provides extension methods for <see cref="MediatRServiceConfiguration" /> to register post-processing behaviors.
/// </summary>
internal static class MediatRServiceConfigurationExtensions
{
    extension(MediatRServiceConfiguration config)
    {
        /// <summary>
        ///     Registers all MediatR post-processors for business operations.
        /// </summary>
        internal void ConfigurePostProcesses()
        {
            config.AddRequestPostProcessor<CreateUserProcessor>();
            config.AddRequestPostProcessor<UpdateUserProcessor>();
            config.AddRequestPostProcessor<DeleteUserProcessor>();
        }

        /// <summary>
        ///     Registers the logging pipeline behavior and all validation behaviors into the MediatR pipeline.
        /// </summary>
        internal void ConfigureBehavior()
        {
            config.AddOpenBehavior(typeof(LoggingBehavior<,>));
            config.ConfigureValidations();
        }

        /// <summary>
        ///     Registers a <see cref="ValidationBehavior{TRequest,TResponse}" /> pipeline behavior for requests
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
    }
}