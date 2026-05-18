using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using HotChocolate.Resolvers;
using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using template.net10.api.Core.Extensions;
using template.net10.api.Domain.DTOs;
using template.net10.api.Persistence.Context;
using template.net10.api.Persistence.Models;

namespace template.net10.api.Features.GraphQL;

/// <summary>
///     Represents a MediatR query request to retrieve users via GraphQL.
/// </summary>
[SuppressMessage(
    "Design",
    "CA1515:Consider making public types internal",
    Justification =
        "Public visibility is required because this request is part of the application messaging contract (MediatR).")]
[SuppressMessage(
    "Design",
    "MemberCanBeInternal",
    Justification =
        "Public visibility is required because this request is part of the application messaging contract (MediatR).")]
public sealed record GraphQLQueryGetUsers(IResolverContext Context) : IRequest<IQueryable<UserDto>>,
    IEqualityOperators<GraphQLQueryGetUsers, GraphQLQueryGetUsers, bool>;

/// <summary>
///     Handles the <see cref="GraphQLQueryGetUsers" /> request by querying users from the database context for GraphQL
///     projections.
/// </summary>
[MustDisposeResource]
internal sealed class GraphQLGetUsersHandlerQuery(IDbContextFactory<AppDbContext> context)
    : IRequestHandler<GraphQLQueryGetUsers, IQueryable<UserDto>>, IAsyncDisposable
{
    /// <summary>
    ///     Database context instance used for querying user data.
    /// </summary>
    private readonly AppDbContext _context =
        context.CreateDbContext() ?? throw new ArgumentNullException(nameof(context));

    /// <summary>
    ///     Asynchronously disposes the database context.
    /// </summary>
    public ValueTask DisposeAsync()
    {
        return _context.DisposeAsync();
    }

    /// <summary>
    ///     Handles the GraphQL users query by returning a projected, no-tracking queryable of user DTOs.
    /// </summary>
    /// <param name="request">The MediatR GraphQL query request containing the resolver context for projections.</param>
    /// <param name="cancellationToken">A token to observe for cancellation of the asynchronous operation.</param>
    [SuppressMessage(
        "ReSharper",
        "ExceptionNotDocumentedOptional",
        Justification =
            "Potential exceptions originate from underlying implementation details and are not part of the method contract.")]
    public Task<IQueryable<UserDto>> Handle(GraphQLQueryGetUsers request,
        CancellationToken cancellationToken)
    {
        return Task.FromResult(_context.Users.AsNoTracking().AsSplitQuery()
            .ApplyProjection(UserProjections.UserProjection, request.Context));
    }
}