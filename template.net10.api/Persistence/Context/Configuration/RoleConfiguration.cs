using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using template.net10.api.Persistence.Models;

namespace template.net10.api.Persistence.Context.Configuration;

/// <summary>
///     EF Core fluent configuration for the <see cref="Role"/> entity.
///     Defines the primary key with constraint name <c>role_pkey</c>.
/// </summary>
internal sealed class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    /// <summary>
    ///     Applies the EF Core configuration for the <see cref="Role"/> entity.
    /// </summary>
    /// <param name="builder">The entity type builder provided by EF Core.</param>
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        ConfigurePrimaryKeys(builder);
    }

    /// <summary>
    ///     Configures the primary key for <see cref="Role"/> with the constraint name <c>role_pkey</c>.
    /// </summary>
    /// <param name="builder">The entity type builder.</param>
    private static void ConfigurePrimaryKeys(EntityTypeBuilder<Role> builder)
    {
        builder.HasKey(static e => e.Id).HasName("role_pkey");
    }
}