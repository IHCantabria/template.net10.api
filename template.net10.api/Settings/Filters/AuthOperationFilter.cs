using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Net.Mime;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;
using template.net10.api.Controllers;
using template.net10.api.Core.Contracts;
using template.net10.api.Settings.Options;
using ZLinq;

namespace template.net10.api.Settings.Filters;

/// <summary>
///     Swashbuckle operation filter that annotates authorized endpoints with JWT security requirements
///     and adds standard 401/403 ProblemDetails response schemas to the OpenAPI document.
///     Reads <see cref="AuthorizeAttribute" /> metadata from the action and its declaring controller.
/// </summary>
[UsedImplicitly]
internal sealed class AuthOperationFilter(IOptions<SwaggerSecurityOptions> config)
    : IOperationFilter, IOrderedFilter
{
    /// <summary>
    ///     Swagger security configuration containing the security scheme identifier used to build
    ///     <see cref="Microsoft.OpenApi.OpenApiSecurityRequirement" /> entries.
    /// </summary>
    private readonly SwaggerSecurityOptions _config =
        config.Value ?? throw new ArgumentNullException(nameof(config));

    /// <summary>
    ///     Applies JWT security metadata to the operation: adds 401/403 response schemas and sets the
    ///     <see cref="Microsoft.OpenApi.OpenApiSecurityRequirement" /> scopes from the resolved
    ///     <see cref="AuthorizeAttribute" /> values. Clears security info for anonymous endpoints.
    /// </summary>
    /// <param name="operation">The OpenAPI operation to enrich.</param>
    /// <param name="context">Filter context providing access to the method and schema generator.</param>
    /// <exception cref="ArgumentNullException">
    ///     <paramref name="operation" /> is <see langword="null" />.
    ///     <paramref name="context" /> is <see langword="null" />.
    /// </exception>
    [SuppressMessage(
        "ReSharper",
        "ExceptionNotDocumentedOptional",
        Justification =
            "Potential exceptions originate from underlying implementation details and are not part of the method contract.")]
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        // Add common response types.
        ArgumentNullException.ThrowIfNull(operation);
        ArgumentNullException.ThrowIfNull(context);

        var attributes = GetAttributes(context);
        var isAuthorized = attributes?.Count > 0;
        if (!isAuthorized)
        {
            operation.Security?.Clear();
            return;
        }

        var attr = attributes?[0];

        // Add what should be show inside the security section
        IList<string> securityInfos =
        [
            $"{nameof(AuthorizeAttribute.Policy)}:{attr?.Policy}", $"{nameof(AuthorizeAttribute.Roles)}:{attr?.Roles}",
            $"{nameof(AuthorizeAttribute.AuthenticationSchemes)}:{attr?.AuthenticationSchemes}"
        ];

        // Add common security response types.
        operation.Responses?.TryAdd(StatusCodes.Status401Unauthorized.ToString(CultureInfo.InvariantCulture),
            new OpenApiResponse
            {
                Description = SwaggerDocumentation.Filter.AuthorizationErrorDescription,
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    [MediaTypeNames.Application.ProblemJson] = new()
                    {
                        Schema = context.SchemaGenerator.GenerateSchema(typeof(UnauthorizedProblemDetailsResource),
                            context.SchemaRepository)
                    }
                }
            });
        operation.Responses?.TryAdd(StatusCodes.Status403Forbidden.ToString(CultureInfo.InvariantCulture),
            new OpenApiResponse
            {
                Description = SwaggerDocumentation.Filter.ForbiddenErrorDescription,
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    [MediaTypeNames.Application.ProblemJson] = new()
                    {
                        Schema = context.SchemaGenerator.GenerateSchema(typeof(ForbiddenProblemDetailsResource),
                            context.SchemaRepository)
                    }
                }
            });

        var schemeRef = new OpenApiSecuritySchemeReference(_config.SchemeId, context.Document);

        operation.Security =
        [
            new OpenApiSecurityRequirement
            {
                [schemeRef] = [.. securityInfos]
            }
        ];
    }

    /// <summary>
    ///     The relative execution order of this filter among all registered <see cref="IOrderedFilter" /> instances.
    ///     Runs after <see cref="DocumentationOperationFilter" /> (order 1), so value is 2.
    /// </summary>
    public int Order => 2;

    /// <summary>
    ///     Collects all <see cref="AuthorizeAttribute" /> instances from the action method and its declaring controller.
    /// </summary>
    /// <param name="context">Filter context providing reflection access to the action and controller.</param>
    /// <returns>A list of authorization attributes; empty if the action is anonymous.</returns>
    private static List<AuthorizeAttribute> GetAttributes(OperationFilterContext context)
    {
        // Get Authorize attribute
        var declaringTypeAttributes = context.MethodInfo.DeclaringType?.GetCustomAttributes(true);
        var methodInfoAttributes = context.MethodInfo.GetCustomAttributes(true);
        return declaringTypeAttributes is not null
            ? [..methodInfoAttributes.Union(declaringTypeAttributes).OfType<AuthorizeAttribute>()]
            : [.. methodInfoAttributes.OfType<AuthorizeAttribute>()];
    }
}