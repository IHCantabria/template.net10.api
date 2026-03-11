using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using template.net10.api.Persistence.Models.Interfaces;

namespace template.net10.api.Persistence.Models;

/// <summary>
///     Entity representing the currently active application version.
///     Holds a single row pointing to the active <see cref="Version"/> entry via a one-to-one relationship.
/// </summary>
[Table("current_version")]
internal class CurrentVersion : IEntity
{
    /// <summary>
    ///     Primary key and foreign key referencing the active <see cref="Version"/>.
    /// </summary>
    [Key]
    [Column("version_id")]
    public required short VersionId { get; set; }

    /// <summary>
    ///     Navigation property to the <see cref="Version"/> entity that is currently active.
    /// </summary>
    [ForeignKey("VersionId")]
    [InverseProperty("CurrentVersion")]
    public virtual required Version Version { get; set; }
}