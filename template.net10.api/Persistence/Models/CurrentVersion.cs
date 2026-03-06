using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using template.net10.api.Persistence.Models.Interfaces;

namespace template.net10.api.Persistence.Models;

/// <summary>
///     ADD DOCUMENTATION
/// </summary>
[Table("current_version")]
internal class CurrentVersion : IEntity
{
    /// <summary>
    ///     ADD DOCUMENTATION
    /// </summary>
    [Key]
    [Column("version_id")]
    public required short VersionId { get; set; }

    /// <summary>
    ///     ADD DOCUMENTATION
    /// </summary>
    [ForeignKey("VersionId")]
    [InverseProperty("CurrentVersion")]
    public virtual required Version Version { get; set; }
}