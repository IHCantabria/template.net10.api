using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using template.net10.api.Persistence.Models.Interfaces;

namespace template.net10.api.Persistence.Models;

/// <summary>
///     Entity representing a system user in the identity domain.
///     Mapped to the <c>user</c> table in the <c>identity</c> schema with unique indexes on <c>Email</c> and <c>Uuid</c>.
/// </summary>
[Table("user", Schema = "identity")]
[Microsoft.EntityFrameworkCore.Index("Email", Name = "user_email_key", IsUnique = true)]
[Microsoft.EntityFrameworkCore.Index("Uuid", Name = "user_uuid_key", IsUnique = true)]
internal partial class User : IEntityWithUuid, IEntityWithId<short>
{
    /// <summary>
    ///     Unique login username.
    /// </summary>
    [Column("username")]
    [MaxLength(100)]
    public required string Username { get; set; }

    /// <summary>
    ///     Unique email address of the user.
    /// </summary>
    [Column("email")]
    [MaxLength(100)]
    public required string Email { get; set; }

    /// <summary>
    ///     Hashed password produced by <see cref="Domain.Password.PasswordHasher"/> using PBKDF2 with the stored salt and pepper.
    /// </summary>
    [Column("password_hash")]
    [MaxLength(128)]
    public required string PasswordHash { get; set; }

    /// <summary>
    ///     Random salt used together with the pepper during password hashing.
    /// </summary>
    [Column("password_salt")]
    [MaxLength(128)]
    public required string PasswordSalt { get; set; }

    /// <summary>
    ///     Foreign key to the <see cref="Role"/> assigned to this user.
    /// </summary>
    [Column("role_id")]
    public required short? RoleId { get; set; }

    /// <summary>
    ///     First name of the user. Optional.
    /// </summary>
    [Column("first_name")]
    [MaxLength(100)]
    public required string? FirstName { get; set; }

    /// <summary>
    ///     Last name of the user. Optional.
    /// </summary>
    [Column("last_name")]
    [MaxLength(100)]
    public required string? LastName { get; set; }

    /// <summary>
    ///     UTC timestamp of the record creation. Defaults to <c>now() AT TIME ZONE 'UTC'</c> at the database level.
    /// </summary>
    [Column("insert_datetime", TypeName = "timestamp without time zone")]
    public required DateTime? InsertDatetime { get; set; }

    /// <summary>
    ///     Indicates whether this user account is disabled and cannot authenticate.
    /// </summary>
    [Column("is_disabled")]
    public required bool IsDisabled { get; set; }

    /// <summary>
    ///     Foreign key of the <see cref="User"/> who created this record.
    /// </summary>
    [Column("insert_user_id")]
    public required short? InsertUserId { get; set; }

    /// <summary>
    ///     UTC timestamp of the last update to this record. Defaults to <c>now() AT TIME ZONE 'UTC'</c>.
    /// </summary>
    [Column("update_datetime", TypeName = "timestamp without time zone")]
    public required DateTime UpdateDatetime { get; set; }

    /// <summary>
    ///     Foreign key of the <see cref="User"/> who last updated this record.
    /// </summary>
    [Column("update_user_id")]
    public required short? UpdateUserId { get; set; }

    /// <summary>
    ///     Navigation property to the <see cref="User"/> who created this record (self-referential).
    /// </summary>
    [ForeignKey("InsertUserId")]
    [InverseProperty("InverseInsertUser")]
    public virtual User? InsertUser { get; set; }

    /// <summary>
    ///     Collection of users created by this user (inverse of the self-referential <c>InsertUser</c> relationship).
    /// </summary>
    [InverseProperty("InsertUser")]
    public virtual ICollection<User> InverseInsertUser { get; } = [];

    /// <summary>
    ///     Collection of users last updated by this user (inverse of the self-referential <c>UpdateUser</c> relationship).
    /// </summary>
    [InverseProperty("UpdateUser")]
    public virtual ICollection<User> InverseUpdateUser { get; } = [];

    /// <summary>
    ///     Navigation property to the <see cref="Role"/> assigned to this user.
    /// </summary>
    [ForeignKey("RoleId")]
    [InverseProperty("Users")]
    public virtual Role? Role { get; set; }

    /// <summary>
    ///     Navigation property to the <see cref="User"/> who last updated this record (self-referential).
    /// </summary>
    [ForeignKey("UpdateUserId")]
    [InverseProperty("InverseUpdateUser")]
    public virtual User? UpdateUser { get; set; }

    /// <summary>
    ///     Collection of <see cref="Claim"/> entities directly assigned to this user
    ///     via the <c>claim_user</c> join table (in addition to role-level claims).
    /// </summary>
    [ForeignKey("UserId")]
    [InverseProperty("Users")]
    public virtual ICollection<Claim> Claims { get; } = [];

    /// <inheritdoc cref="IEntityWithId{T}.Id" />
    [Key]
    [Column("id")]
    public required short Id { get; init; }

    /// <inheritdoc cref="IEntityWithUuid.Uuid" />
    [Column("uuid")]
    public required Guid Uuid { get; init; }
}