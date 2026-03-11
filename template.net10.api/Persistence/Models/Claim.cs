using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using template.net10.api.Persistence.Models.Interfaces;

namespace template.net10.api.Persistence.Models;

/// <summary>
///     Entity representing a permission claim in the identity system.
///     Mapped to the <c>claim</c> table in the <c>identity</c> schema with a unique index on <c>Name</c>.
/// </summary>
[Table("claim", Schema = "identity")]
[Microsoft.EntityFrameworkCore.Index("Name", Name = "claim_key", IsUnique = true)]
internal class Claim : IEntityWithNameKey, IEntityWithId<short>
{
    /// <summary>
    ///     Collection of <see cref="Role"/> entities that include this claim,
    ///     linked via the <c>claim_role</c> join table.
    /// </summary>
    [ForeignKey("ClaimId")]
    [InverseProperty("Claims")]
    public virtual ICollection<Role> Roles { get; } = [];

    /// <summary>
    ///     Collection of <see cref="User"/> entities that have this claim directly assigned,
    ///     linked via the <c>claim_user</c> join table.
    /// </summary>
    [ForeignKey("ClaimId")]
    [InverseProperty("Claims")]
    public virtual ICollection<User> Users { get; } = [];

    /// <inheritdoc cref="IEntityWithId{T}.Id" />
    [Key]
    [Column("id")]
    public required short Id { get; init; }

    /// <inheritdoc cref="IEntityWithNameKey.Name" />
    [Column("name")]
    [MaxLength(100)]
    public required string Name { get; init; }
}