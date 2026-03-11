using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using template.net10.api.Core.Base;
using template.net10.api.Core.Interfaces;
using template.net10.api.Persistence.Models;

namespace template.net10.api.Domain.Specifications;

/// <summary>
///     Verification specification that checks whether a user with the given email address exists.
/// </summary>
internal sealed class UserEmailVerification : VerificationBase<User>
{
    /// <summary>
    ///     Initializes a new instance filtering by the specified email address.
    /// </summary>
    /// <param name="email">The email address to verify.</param>
    internal UserEmailVerification(string email)
    {
        AddFilter(u => u.Email == email);
    }
}

/// <summary>
///     Verification specification that checks whether a user identified by UUID is disabled.
/// </summary>
internal sealed class UserDisabledVerification : VerificationBase<User>
{
    /// <summary>
    ///     Initializes a new instance filtering by UUID and disabled status.
    /// </summary>
    /// <param name="key">The UUID of the user to verify.</param>
    internal UserDisabledVerification(Guid key)
    {
        AddFilter(u => u.Uuid == key);
        AddFilter(static u => u.IsDisabled);
    }
}

/// <summary>
///     Verification specification that checks whether a user is enabled (not disabled).
/// </summary>
internal sealed class UserEnabledVerification : VerificationBase<User>
{
    /// <summary>
    ///     Initializes a new instance filtering by UUID and enabled status.
    /// </summary>
    /// <param name="key">The UUID of the user to verify.</param>
    internal UserEnabledVerification(Guid key)
    {
        AddFilter(u => u.Uuid == key);
        AddFilter(static u => !u.IsDisabled);
    }

    /// <summary>
    ///     Initializes a new instance filtering by email address and enabled status.
    /// </summary>
    /// <param name="email">The email address of the user to verify.</param>
    internal UserEnabledVerification(string email)
    {
        AddFilter(u => u.Email == email);
        AddFilter(static u => !u.IsDisabled);
    }
}

/// <summary>
///     Read-only specification that retrieves a user by UUID, ordered by insert date descending.
/// </summary>
internal sealed class UserReadByKeySpecification : SpecificationBase<User>
{
    /// <summary>
    ///     Initializes a new instance filtering by UUID with no-tracking and single-query behavior.
    /// </summary>
    /// <param name="key">The UUID of the user to retrieve.</param>
    internal UserReadByKeySpecification(Guid key)
    {
        AddFilter(u => u.Uuid == key);
        AddOrderBy(static u => u.InsertDatetime ?? DateTime.SpecifyKind(DateTime.MinValue, DateTimeKind.Utc),
            OrderByType.Desc);
        SetQueryTrackStrategy(QueryTrackingBehavior.NoTracking);
        SetQuerySplitStrategy(QuerySplittingBehavior.SingleQuery);
    }
}

/// <summary>
///     Tracked specification that retrieves a user by UUID for write operations, ordered by insert date descending.
/// </summary>
internal sealed class UserWriteByKeySpecification : SpecificationBase<User>
{
    /// <summary>
    ///     Initializes a new instance filtering by UUID with full tracking and single-query behavior.
    /// </summary>
    /// <param name="key">The UUID of the user to retrieve for modification.</param>
    internal UserWriteByKeySpecification(Guid key)
    {
        AddFilter(u => u.Uuid == key);
        AddOrderBy(static u => u.InsertDatetime ?? DateTime.SpecifyKind(DateTime.MinValue, DateTimeKind.Utc),
            OrderByType.Desc);
        SetQueryTrackStrategy(QueryTrackingBehavior.TrackAll);
        SetQuerySplitStrategy(QuerySplittingBehavior.SingleQuery);
    }
}

/// <summary>
///     Tracked specification that retrieves a user and its claims by UUID for delete operations.
/// </summary>
internal sealed class UserWriteDeleteByKeySpecification : SpecificationBase<User>
{
    /// <summary>
    ///     Initializes a new instance filtering by UUID, including related claims, with full tracking.
    /// </summary>
    /// <param name="key">The UUID of the user to retrieve for deletion.</param>
    [SuppressMessage(
        "ReSharper",
        "ExceptionNotDocumentedOptional",
        Justification =
            "Potential exceptions originate from underlying implementation details and are not part of the method contract.")]
    internal UserWriteDeleteByKeySpecification(Guid key)
    {
        AddFilter(u => u.Uuid == key);
        AddInclude(static q => q.Include(static u => u.Claims));
        AddOrderBy(static u => u.InsertDatetime ?? DateTime.SpecifyKind(DateTime.MinValue, DateTimeKind.Utc),
            OrderByType.Desc);
        SetQueryTrackStrategy(QueryTrackingBehavior.TrackAll);
        SetQuerySplitStrategy(QuerySplittingBehavior.SingleQuery);
    }
}

/// <summary>
///     Read-only specification that retrieves a user by email address, ordered by insert date descending.
/// </summary>
internal sealed class UserReadByEmailSpecification : SpecificationBase<User>
{
    /// <summary>
    ///     Initializes a new instance filtering by email with no-tracking and single-query behavior.
    /// </summary>
    /// <param name="email">The email address of the user to retrieve.</param>
    internal UserReadByEmailSpecification(string email)
    {
        AddFilter(u => u.Email == email);
        AddOrderBy(static u => u.InsertDatetime ?? DateTime.SpecifyKind(DateTime.MinValue, DateTimeKind.Utc),
            OrderByType.Desc);
        SetQueryTrackStrategy(QueryTrackingBehavior.NoTracking);
        SetQuerySplitStrategy(QuerySplittingBehavior.SingleQuery);
    }
}

/// <summary>
///     Read-only specification that retrieves all users, ordered by insert date descending.
/// </summary>
internal sealed class UsersReadSpecification : SpecificationBase<User>
{
    /// <summary>
    ///     Initializes a new instance with no-tracking, single-query behavior and descending insert date order.
    /// </summary>
    internal UsersReadSpecification()
    {
        AddOrderBy(static u => u.InsertDatetime ?? DateTime.SpecifyKind(DateTime.MinValue, DateTimeKind.Utc),
            OrderByType.Desc);
        SetQueryTrackStrategy(QueryTrackingBehavior.NoTracking);
        SetQuerySplitStrategy(QuerySplittingBehavior.SingleQuery);
    }
}