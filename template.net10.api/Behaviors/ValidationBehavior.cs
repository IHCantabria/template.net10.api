using FluentValidation;
using FluentValidation.Results;
using JetBrains.Annotations;
using MediatR;
using Microsoft.Extensions.Localization;
using template.net10.api.Core.Parallel;
using template.net10.api.Localize.Resources;

namespace template.net10.api.Behaviors;

/// <summary>
///     MediatR pipeline behavior that validates incoming requests using FluentValidation before forwarding them to the next handler.
/// </summary>
/// <typeparam name="TRequest">The type of the request to validate.</typeparam>
/// <typeparam name="TResponse">The type of the response wrapped in a <c>Result</c>.</typeparam>
[UsedImplicitly]
internal sealed class ValidationBehavior<TRequest, TResponse>(
    IEnumerable<IValidator<TRequest>> validators,
    IStringLocalizer<ResourceMain> localizer)
    : IPipelineBehavior<TRequest, LanguageExt.Common.Result<TResponse>>
    where TRequest : notnull
{
    /// <summary>
    ///     String localizer for producing localized validation error messages.
    /// </summary>
    private readonly IStringLocalizer<ResourceMain> _localizer =
        localizer ?? throw new ArgumentNullException(nameof(localizer));

    /// <summary>
    ///     Collection of FluentValidation validators registered for <typeparamref name="TRequest"/>.
    /// </summary>
    private readonly IEnumerable<IValidator<TRequest>> _validators =
        validators ?? throw new ArgumentNullException(nameof(validators));

    /// <summary>
    ///     Handles an incoming request by running validation before delegating to the next handler.
    /// </summary>
    /// <param name="request">The incoming MediatR request.</param>
    /// <param name="next">The delegate for the next handler in the pipeline.</param>
    /// <param name="cancellationToken">A token to observe for cancellation.</param>
    /// <returns>A <c>Result</c> containing either the successful response or a <see cref="ValidationException"/>.</returns>
    public Task<LanguageExt.Common.Result<TResponse>> Handle(TRequest request,
        RequestHandlerDelegate<LanguageExt.Common.Result<TResponse>> next,
        CancellationToken cancellationToken)
    {
        return BehaviorLogicAsync(request, next, cancellationToken);
    }

    /// <summary>
    ///     Validates the request and either returns a faulted result with validation errors or delegates to the next handler.
    /// </summary>
    /// <param name="request">The request to validate.</param>
    /// <param name="next">The delegate for the next handler in the pipeline.</param>
    /// <param name="cancellationToken">A token to observe for cancellation.</param>
    /// <returns>A <c>Result</c> containing either the response or a <see cref="ValidationException"/>.</returns>
    private async Task<LanguageExt.Common.Result<TResponse>> BehaviorLogicAsync(TRequest request,
        RequestHandlerDelegate<LanguageExt.Common.Result<TResponse>> next,
        CancellationToken cancellationToken = default)
    {
        var failures = await ValidateRequestAsync(request, cancellationToken).ConfigureAwait(false);
        return failures.Count > 0
            ? new LanguageExt.Common.Result<TResponse>(new ValidationException(failures))
            : await next(cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    ///     Runs all registered validators against the request and collects any validation failures.
    /// </summary>
    /// <param name="request">The request to validate.</param>
    /// <param name="cancellationToken">A token to observe for cancellation.</param>
    /// <returns>A collection of <see cref="ValidationFailure"/> instances from all validators.</returns>
    private async Task<ICollection<ValidationFailure>> ValidateRequestAsync(TRequest request,
        CancellationToken cancellationToken = default)
    {
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        var results = await ValidateValidatorsAsync(request, cts).ConfigureAwait(false);
        return [..AggregateValidationResults(results)];
    }

    /// <summary>
    ///     Executes all registered validators in parallel and returns their results.
    /// </summary>
    /// <param name="request">The request to validate.</param>
    /// <param name="cts">A linked cancellation token source for coordinating parallel validator execution.</param>
    /// <returns>The collection of <see cref="ValidationResult"/> produced by each validator.</returns>
    /// <exception cref="ValidationException">Thrown when one or more validators fail to complete.</exception>
    private async Task<IEnumerable<ValidationResult>> ValidateValidatorsAsync(TRequest request,
        CancellationTokenSource cts)
    {
        var tasks = _validators.Select(validator =>
        {
            return Task.Run(() => validator.ValidateAsync(new ValidationContext<TRequest>(request), cts.Token),
                cts.Token);
        });
        var result = await ParallelUtils.ExecuteDependentInParallelAsync(tasks, cts).ConfigureAwait(false);
        var validateValidatorsAsync = result.ToList();
        return validateValidatorsAsync.Length() != _validators.Length()
            ? throw new ValidationException(_localizer["GenericValidatorError"])
            : validateValidatorsAsync;
    }

    /// <summary>
    ///     Aggregates and flattens all <see cref="ValidationFailure"/> instances from multiple validation results, filtering out nulls and duplicates.
    /// </summary>
    /// <param name="results">The validation results to aggregate.</param>
    /// <returns>A distinct sequence of non-null <see cref="ValidationFailure"/> instances.</returns>
    private static IEnumerable<ValidationFailure> AggregateValidationResults(IEnumerable<ValidationResult> results)
    {
        return results
            .Where(static r => r?.Errors != null)
            .SelectMany(static r => r.Errors).Distinct()
            .Where(static f => f is not null);
    }
}