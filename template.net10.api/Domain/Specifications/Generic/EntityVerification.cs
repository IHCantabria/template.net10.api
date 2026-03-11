using System.Diagnostics.CodeAnalysis;
using template.net10.api.Core.Base;
using template.net10.api.Persistence.Models.Interfaces;

namespace template.net10.api.Domain.Specifications.Generic;

/// <summary>
///     Verification specification that checks whether an entity with the given primary key exists.
/// </summary>
/// <typeparam name="TEntity">The entity type that implements <see cref="IEntityWithId{TKey}" />.</typeparam>
/// <typeparam name="TKey">The type of the entity's primary key.</typeparam>
internal sealed class EntityVerificationById<TEntity, TKey> : VerificationBase<TEntity>
    where TEntity : class, IEntityWithId<TKey> where TKey : struct
{
    /// <summary>
    ///     Initializes a new instance filtering by the specified entity identifier.
    /// </summary>
    /// <param name="id">The primary key value to verify existence for.</param>
    internal EntityVerificationById(TKey id)
    {
        AddFilter(e => e.Id.Equals(id));
    }
}

/// <summary>
///     Verification specification that checks whether entities with the given primary keys exist.
/// </summary>
/// <typeparam name="TEntity">The entity type that implements <see cref="IEntityWithId{TKey}" />.</typeparam>
/// <typeparam name="TKey">The type of the entity's primary key.</typeparam>
[SuppressMessage(
    "ReSharper",
    "UnusedType.Global",
    Justification =
        "Generic specification type intended for reuse in repository queries; usage may be indirect or consumer-dependent.")]
internal sealed class EntitiesVerificationByIds<TEntity, TKey> : VerificationBase<TEntity>
    where TEntity : class, IEntityWithId<TKey> where TKey : struct
{
    /// <summary>
    ///     Initializes a new instance, optionally filtering by the specified collection of entity identifiers.
    /// </summary>
    /// <param name="entityIds">
    ///     An optional collection of primary key values. When <see langword="null" />, no filter is
    ///     applied.
    /// </param>
    [SuppressMessage(
        "ReSharper",
        "ExceptionNotDocumentedOptional",
        Justification =
            "Potential exceptions originate from underlying implementation details and are not part of the method contract.")]
    internal EntitiesVerificationByIds(IEnumerable<TKey>? entityIds = null)
    {
        if (entityIds is null) return;

        var enumerable = entityIds.ToList();
        AddFilter(e => enumerable.Contains(e.Id));
    }
}

/// <summary>
///     Verification specification that checks whether an entity with the given DataHub identifier exists.
/// </summary>
/// <typeparam name="TEntity">The entity type that implements <see cref="IEntityWithDatahubId" />.</typeparam>
[SuppressMessage(
    "ReSharper",
    "UnusedType.Global",
    Justification =
        "Generic specification type intended for reuse in repository queries; usage may be indirect or consumer-dependent.")]
internal sealed class EntityVerificationByDatahubId<TEntity> : VerificationBase<TEntity>
    where TEntity : class, IEntityWithDatahubId
{
    /// <summary>
    ///     Initializes a new instance filtering by the specified DataHub identifier.
    /// </summary>
    /// <param name="id">The DataHub identifier to verify existence for.</param>
    internal EntityVerificationByDatahubId(short id)
    {
        AddFilter(e => e.DatahubId == id);
    }
}

/// <summary>
///     Verification specification that checks whether entities with the given DataHub identifiers exist.
/// </summary>
/// <typeparam name="TEntity">The entity type that implements <see cref="IEntityWithDatahubId" />.</typeparam>
[SuppressMessage(
    "ReSharper",
    "UnusedType.Global",
    Justification =
        "Generic specification type intended for reuse in repository queries; usage may be indirect or consumer-dependent.")]
internal sealed class EntitiesVerificationByDatahubIds<TEntity> : VerificationBase<TEntity>
    where TEntity : class, IEntityWithDatahubId
{
    /// <summary>
    ///     Initializes a new instance, optionally filtering by the specified collection of DataHub identifiers.
    /// </summary>
    /// <param name="entityIds">
    ///     An optional collection of DataHub identifiers. When <see langword="null" />, no filter is
    ///     applied.
    /// </param>
    [SuppressMessage(
        "ReSharper",
        "ExceptionNotDocumentedOptional",
        Justification =
            "Potential exceptions originate from underlying implementation details and are not part of the method contract.")]
    internal EntitiesVerificationByDatahubIds(IEnumerable<short>? entityIds = null)
    {
        if (entityIds is null) return;

        var enumerable = entityIds.ToList();
        AddFilter(e => enumerable.Contains(e.DatahubId));
    }
}

/// <summary>
///     Verification specification that checks whether an entity with the given UUID exists.
/// </summary>
/// <typeparam name="TEntity">The entity type that implements <see cref="IEntityWithUuid" />.</typeparam>
internal sealed class EntityVerificationByUuid<TEntity> : VerificationBase<TEntity>
    where TEntity : class, IEntityWithUuid
{
    /// <summary>
    ///     Initializes a new instance filtering by the specified UUID.
    /// </summary>
    /// <param name="uuid">The UUID to verify existence for.</param>
    internal EntityVerificationByUuid(Guid uuid)
    {
        AddFilter(e => e.Uuid == uuid);
    }
}

/// <summary>
///     Verification specification that checks whether entities with the given UUIDs exist.
/// </summary>
/// <typeparam name="TEntity">The entity type that implements <see cref="IEntityWithUuid" />.</typeparam>
[SuppressMessage(
    "ReSharper",
    "UnusedType.Global",
    Justification =
        "Generic specification type intended for reuse in repository queries; usage may be indirect or consumer-dependent.")]
internal sealed class EntitiesVerificationByUuids<TEntity> : VerificationBase<TEntity>
    where TEntity : class, IEntityWithUuid
{
    /// <summary>
    ///     Initializes a new instance, optionally filtering by the specified collection of UUIDs.
    /// </summary>
    /// <param name="entityUuids">An optional collection of UUID strings. When <see langword="null" />, no filter is applied.</param>
    [SuppressMessage(
        "ReSharper",
        "ExceptionNotDocumentedOptional",
        Justification =
            "Potential exceptions originate from underlying implementation details and are not part of the method contract.")]
    internal EntitiesVerificationByUuids(IEnumerable<string>? entityUuids = null)
    {
        if (entityUuids is null) return;

        var enumerable = entityUuids.ToList();
        AddFilter(e => enumerable.Contains(e.Uuid.ToString()));
    }
}

/// <summary>
///     Verification specification that checks whether an entity with the given name key exists.
/// </summary>
/// <typeparam name="TEntity">The entity type that implements <see cref="IEntityWithNameKey" />.</typeparam>
internal sealed class EntityVerificationByNameKey<TEntity> : VerificationBase<TEntity>
    where TEntity : class, IEntityWithNameKey
{
    /// <summary>
    ///     Initializes a new instance filtering by the specified name key.
    /// </summary>
    /// <param name="name">The name key to verify existence for.</param>
    internal EntityVerificationByNameKey(string name)
    {
        AddFilter(e => e.Name == name);
    }
}

/// <summary>
///     Verification specification that checks whether entities with the given name keys exist.
/// </summary>
/// <typeparam name="TEntity">The entity type that implements <see cref="IEntityWithNameKey" />.</typeparam>
[SuppressMessage(
    "ReSharper",
    "UnusedType.Global",
    Justification =
        "Generic specification type intended for reuse in repository queries; usage may be indirect or consumer-dependent.")]
internal sealed class EntitiesVerificationByNameKeys<TEntity> : VerificationBase<TEntity>
    where TEntity : class, IEntityWithNameKey
{
    /// <summary>
    ///     Initializes a new instance, optionally filtering by the specified collection of name keys.
    /// </summary>
    /// <param name="entityNames">An optional collection of name keys. When <see langword="null" />, no filter is applied.</param>
    [SuppressMessage(
        "ReSharper",
        "ExceptionNotDocumentedOptional",
        Justification =
            "Potential exceptions originate from underlying implementation details and are not part of the method contract.")]
    internal EntitiesVerificationByNameKeys(IEnumerable<string>? entityNames = null)
    {
        if (entityNames is null) return;

        var enumerable = entityNames.ToList();
        AddFilter(e => enumerable.Contains(e.Name));
    }
}

/// <summary>
///     Verification specification that checks whether an entity with the given name exists.
/// </summary>
/// <typeparam name="TEntity">The entity type that implements <see cref="IEntityWithName" />.</typeparam>
[SuppressMessage(
    "ReSharper",
    "UnusedType.Global",
    Justification =
        "Generic specification type intended for reuse in repository queries; usage may be indirect or consumer-dependent.")]
internal sealed class EntityVerificationByName<TEntity> : VerificationBase<TEntity>
    where TEntity : class, IEntityWithName
{
    /// <summary>
    ///     Initializes a new instance filtering by the specified entity name.
    /// </summary>
    /// <param name="name">The name to verify existence for.</param>
    internal EntityVerificationByName(string name)
    {
        AddFilter(e => e.Name == name);
    }
}

/// <summary>
///     Verification specification that checks whether entities with the given names exist.
/// </summary>
/// <typeparam name="TEntity">The entity type that implements <see cref="IEntityWithNameKey" />.</typeparam>
[SuppressMessage(
    "ReSharper",
    "UnusedType.Global",
    Justification =
        "Generic specification type intended for reuse in repository queries; usage may be indirect or consumer-dependent.")]
internal sealed class EntitiesVerificationByNames<TEntity> : VerificationBase<TEntity>
    where TEntity : class, IEntityWithNameKey
{
    /// <summary>
    ///     Initializes a new instance, optionally filtering by the specified collection of entity names.
    /// </summary>
    /// <param name="entityNames">An optional collection of entity names. When <see langword="null" />, no filter is applied.</param>
    [SuppressMessage(
        "ReSharper",
        "ExceptionNotDocumentedOptional",
        Justification =
            "Potential exceptions originate from underlying implementation details and are not part of the method contract.")]
    internal EntitiesVerificationByNames(IEnumerable<string>? entityNames = null)
    {
        if (entityNames is null) return;

        var enumerable = entityNames.ToList();
        AddFilter(e => enumerable.Contains(e.Name));
    }
}