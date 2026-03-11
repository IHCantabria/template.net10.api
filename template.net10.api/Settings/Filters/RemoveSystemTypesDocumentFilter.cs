using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace template.net10.api.Settings.Filters;

/// <summary>
///     Swashbuckle document filter that removes all schema components whose key starts with <c>"System"</c>
///     from the generated OpenAPI document, preventing internal .NET framework types from polluting the API spec.
/// </summary>
[UsedImplicitly]
internal sealed class RemoveSystemTypesDocumentFilter : IDocumentFilter, IOrderedFilter
{
    /// <summary>
    ///     Initialises the filter and sets <see cref="Order"/> to 1.
    /// </summary>
    public RemoveSystemTypesDocumentFilter()
    {
        Order = 1;
    }

    /// <summary>
    ///     Removes all schema entries from <paramref name="swaggerDoc"/> whose key begins with <c>"System"</c>.
    /// </summary>
    /// <param name="swaggerDoc">The OpenAPI document to clean up.</param>
    /// <param name="context">Filter context (unused).</param>
    /// <exception cref="ArgumentNullException">
    ///     <paramref name="swaggerDoc" /> is <see langword="null" />.
    ///     <paramref name="context" /> is <see langword="null" />.
    /// </exception>
    [SuppressMessage(
        "ReSharper",
        "ExceptionNotDocumentedOptional",
        Justification =
            "Potential exceptions originate from underlying implementation details and are not part of the method contract.")]
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        ArgumentNullException.ThrowIfNull(swaggerDoc);

        var keysToRemove = (from schema in swaggerDoc.Components?.Schemas
            where schema.Key.StartsWith("System", StringComparison.InvariantCulture)
            select schema.Key).ToList();

        // Remove the unwanted schemas
        foreach (var key in keysToRemove) swaggerDoc.Components?.Schemas?.Remove(key);
    }

    /// <summary>
    ///     The relative execution order of this filter among all registered <see cref="IOrderedFilter"/> instances.
    /// </summary>
    public int Order { get; }
}