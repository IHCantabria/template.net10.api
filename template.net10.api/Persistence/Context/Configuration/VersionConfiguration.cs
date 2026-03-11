using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Version = template.net10.api.Persistence.Models.Version;

namespace template.net10.api.Persistence.Context.Configuration;

/// <summary>
///     EF Core fluent configuration for the <see cref="Version"/> entity.
///     Defines the primary key and the server-side default SQL value for the <c>date</c> column.
/// </summary>
internal sealed class VersionConfiguration : IEntityTypeConfiguration<Version>
{
    /// <summary>
    ///     Applies the EF Core configuration for the <see cref="Version"/> entity.
    /// </summary>
    /// <param name="builder">The entity type builder provided by EF Core.</param>
    public void Configure(EntityTypeBuilder<Version> builder)
    {
        ConfigurePrimaryKeys(builder);
        ConfigureProperties(builder);
    }

    /// <summary>
    ///     Configures the primary key for <see cref="Version"/> with the constraint name <c>version_pkey</c>.
    /// </summary>
    /// <param name="builder">The entity type builder.</param>
    private static void ConfigurePrimaryKeys(EntityTypeBuilder<Version> builder)
    {
        builder.HasKey(static e => e.Id).HasName("version_pkey");
    }

    /// <summary>
    ///     Configures the server-side default value for <see cref="Version.Date"/>
    ///     using the SQL function <c>now() AT TIME ZONE 'UTC'</c>.
    /// </summary>
    /// <param name="builder">The entity type builder.</param>
    private static void ConfigureProperties(EntityTypeBuilder<Version> builder)
    {
        builder.Property(static e => e.Date).HasDefaultValueSql("(now() AT TIME ZONE 'UTC'::text)");
    }
}