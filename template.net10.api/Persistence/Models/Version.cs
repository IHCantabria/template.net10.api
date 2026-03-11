using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using template.net10.api.Persistence.Models.Interfaces;

namespace template.net10.api.Persistence.Models;

/// <summary>
///     Entity representing an application version entry.
///     Mapped to the <c>version</c> table with a unique index on <c>Name</c>.
/// </summary>
[Table("version")]
[Microsoft.EntityFrameworkCore.Index("Name", Name = "versiob_key", IsUnique = true)]
internal class Version : IEntityWithNameKey, IEntityWithId<short>
{
    /// <summary>
    ///     Short version tag (e.g. <c>v1.0.0</c>) used to identify the release.
    /// </summary>
    [Column("tag")]
    [MaxLength(100)]
    public required string Tag { get; set; }

    /// <summary>
    ///     UTC timestamp of the version release. Defaults to <c>now() AT TIME ZONE 'UTC'</c> at the database level.
    /// </summary>
    [Column("date", TypeName = "timestamp without time zone")]
    public required DateTime Date { get; set; }

    /// <summary>
    ///     Navigation property back to <see cref="CurrentVersion"/> if this version is currently active.
    ///     <see langword="null"/> when this is not the active version.
    /// </summary>
    [InverseProperty("Version")]
    public virtual CurrentVersion? CurrentVersion { get; set; }

    /// <inheritdoc cref="IEntityWithId{T}.Id" />
    [Key]
    [Column("id")]
    public required short Id { get; init; }

    /// <inheritdoc cref="IEntityWithNameKey.Name" />
    [Column("name")]
    [MaxLength(100)]
    public required string Name { get; init; }
}