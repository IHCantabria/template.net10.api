using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using template.net10.api.Persistence.Models;

namespace template.net10.api.Persistence.Context.Configuration;

/// <summary>
///     EF Core fluent configuration for the <see cref="CurrentVersion"/> entity.
///     Defines the primary key that is never auto-generated and the one-to-one relationship with <see cref="Models.Version"/>.
/// </summary>
internal sealed class CurrentVersionConfiguration : IEntityTypeConfiguration<CurrentVersion>
{
    /// <summary>
    ///     Applies the EF Core configuration for the <see cref="CurrentVersion"/> entity.
    /// </summary>
    /// <param name="builder">The entity type builder provided by EF Core.</param>
    public void Configure(EntityTypeBuilder<CurrentVersion> builder)
    {
        ConfigurePrimaryKeys(builder);
        ConfigureVersionRelation(builder);
    }

    /// <summary>
    ///     Configures the primary key for <see cref="CurrentVersion"/> using <c>VersionId</c>
    ///     with constraint name <c>current_version_pkey</c>. The value is never auto-generated.
    /// </summary>
    /// <param name="builder">The entity type builder.</param>
    private static void ConfigurePrimaryKeys(EntityTypeBuilder<CurrentVersion> builder)
    {
        builder.HasKey(static e => e.VersionId).HasName("current_version_pkey");

        builder.Property(static e => e.VersionId).ValueGeneratedNever();
    }

    /// <summary>
    ///     Configures the one-to-one relationship between <see cref="CurrentVersion"/> and <see cref="Models.Version"/>
    ///     with <see cref="DeleteBehavior.Restrict"/> and the constraint <c>current_version_version_id_fkey</c>.
    /// </summary>
    /// <param name="builder">The entity type builder.</param>
    private static void ConfigureVersionRelation(EntityTypeBuilder<CurrentVersion> builder)
    {
        builder.HasOne(static d => d.Version).WithOne(static p => p.CurrentVersion)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("current_version_version_id_fkey");
    }
}