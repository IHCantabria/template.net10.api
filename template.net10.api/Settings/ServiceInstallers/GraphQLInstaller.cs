using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using template.net10.api.GraphQL;
using template.net10.api.GraphQL.Types;
using template.net10.api.Persistence.Context;
using template.net10.api.Settings.Interfaces;

namespace template.net10.api.Settings.ServiceInstallers;

/// <summary>
///     Service installer that registers and configures the Hot Chocolate GraphQL server, including
///     filtering, projections, sorting, spatial support, and the root query type. Load order: 25.
/// </summary>
[UsedImplicitly]
internal sealed class GraphQLInstaller : IServiceInstaller
{
    /// <inheritdoc cref="IServiceInstaller.LoadOrder" />
    public short LoadOrder => 25;

    /// <inheritdoc cref="IServiceInstaller.InstallServiceAsync" />
    /// <exception cref="ArgumentNullException"><paramref name="builder" /> is <see langword="null" />.</exception>
    [SuppressMessage(
        "ReSharper",
        "ExceptionNotDocumentedOptional",
        Justification =
            "Potential exceptions originate from underlying implementation details and are not part of the method contract.")]
    public Task InstallServiceAsync(WebApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        var graphQLServer = builder.Services.AddGraphQLServer();
        if (builder.Environment.EnvironmentName is Envs.Development or Envs.Local)
            graphQLServer.ModifyRequestOptions(static opt => opt.IncludeExceptionDetails = true);

        graphQLServer.AddFiltering().AddProjections().AddSorting().AddSpatialTypes().AddSpatialProjections()
            .AddSpatialFiltering()
            .AddQueryType<QueryProvider>()
            .AddType<PointSortInputType>()
            .AddType<PolygonSortInputType>()
            .RegisterDbContextFactory<AppDbContext>();
        return Task.CompletedTask;
    }
}