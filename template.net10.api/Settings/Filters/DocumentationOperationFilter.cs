using System.Globalization;
using System.Net.Mime;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;
using template.net10.api.Controllers;
using template.net10.api.Core.Contracts;

namespace template.net10.api.Settings.Filters;

/// <summary>
///     Swashbuckle operation filter that appends common error response schemas (408, 500) to every
///     operation in the OpenAPI document, ensuring consistent ProblemDetails documentation across all endpoints.
/// </summary>
[UsedImplicitly]
internal sealed class DocumentationOperationFilter : IOperationFilter, IOrderedFilter
{
    /// <summary>
    ///     Initialises the filter and sets <see cref="Order"/> to 1 so it runs before security filters.
    /// </summary>
    public DocumentationOperationFilter()
    {
        Order = 1;
    }

    /// <summary>
    ///     Appends 408 (Request Timeout) and 500 (Internal Server Error) response schemas with
    ///     ProblemDetails content to the given <paramref name="operation"/>.
    /// </summary>
    /// <param name="operation">The OpenAPI operation to enrich.</param>
    /// <param name="context">Filter context providing access to the schema generator.</param>
    /// <exception cref="ArgumentNullException">
    ///     <paramref name="operation" /> is <see langword="null" />.
    ///     <paramref name="context" /> is <see langword="null" />.
    /// </exception>
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        ArgumentNullException.ThrowIfNull(operation);
        ArgumentNullException.ThrowIfNull(context);
        operation.Responses?.TryAdd(StatusCodes.Status408RequestTimeout.ToString(CultureInfo.InvariantCulture),
            new OpenApiResponse
            {
                Description = SwaggerDocumentation.Filter.RequestTimeoutErrorDescription,
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    [MediaTypeNames.Application.ProblemJson] = new()
                    {
                        Schema = context.SchemaGenerator.GenerateSchema(typeof(RequestTimeoutProblemDetailsResource),
                            context.SchemaRepository)
                    }
                }
            });
        operation.Responses?.TryAdd(StatusCodes.Status500InternalServerError.ToString(CultureInfo.InvariantCulture),
            new OpenApiResponse
            {
                Description = SwaggerDocumentation.Filter.InternalServerErrorDescription,
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    [MediaTypeNames.Application.ProblemJson] = new()
                    {
                        Schema = context.SchemaGenerator.GenerateSchema(typeof(InternalServerProblemDetailsResource),
                            context.SchemaRepository)
                    }
                }
            });
    }

    /// <summary>
    ///     The relative execution order of this filter among all registered <see cref="IOrderedFilter"/> instances.
    ///     Set to 1 so it runs before authorization filters.
    /// </summary>
    public int Order { get; }
}