using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using template.net10.api.Persistence.Models.Interfaces;

namespace template.net10.api.Persistence.Models;

/// <summary>
///     Entity representing a user role in the identity system.
///     Mapped to the <c>role</c> table in the <c>identity</c> schema with a unique index on <c>Name</c>.
/// </summary>
[Table("role", Schema = "identity")]
[Microsoft.EntityFrameworkCore.Index("Name", Name = "rol_key", IsUnique = true)]
internal class Role : IEntityWithNameKey, IEntityWithAlias, IEntityWithId<short>
{
    /// <summary>
    ///     Collection of <see cref="User"/> entities assigned to this role.
    /// </summary>
    [InverseProperty("Role")]
    public virtual ICollection<User> Users { get; } = [];

    /// <summary>
    ///     Collection of <see cref="Claim"/> entities associated with this role
    ///     via the <c>claim_role</c> join table.
    /// </summary>
    [ForeignKey("RoleId")]
    [InverseProperty("Roles")]
    public virtual ICollection<Claim> Claims { get; } = [];

    /// <inheritdoc cref="IEntityWithAlias.AliasText" />
    [Column("alias")]
    [MaxLength(100)]
    public required string AliasText { get; set; }

    /// <inheritdoc cref="IEntityWithId{T}.Id" />
    [Key]
    [Column("id")]
    public required short Id { get; init; }

    /// <inheritdoc cref="IEntityWithNameKey.Name" />
    [Column("name")]
    [MaxLength(100)]
    public required string Name { get; init; }
}