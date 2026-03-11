using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using template.net10.api.Persistence.Models;

namespace template.net10.api.Persistence.Context.Configuration;

/// <summary>
///     EF Core fluent configuration for the <see cref="Claim"/> entity.
///     Defines the primary key and the many-to-many relationship between <see cref="Claim"/> and <see cref="Role"/>
///     using the <c>claim_role</c> join table in the <c>identity</c> schema.
/// </summary>
internal sealed class ClaimConfiguration : IEntityTypeConfiguration<Claim>
{
    /// <summary>
    ///     Applies the EF Core configuration for the <see cref="Claim"/> entity.
    /// </summary>
    /// <param name="builder">The entity type builder provided by EF Core.</param>
    public void Configure(EntityTypeBuilder<Claim> builder)
    {
        ConfigurePrimaryKeys(builder);
        ConfigureClaimRolesRelation(builder);
    }

    /// <summary>
    ///     Configures the primary key for <see cref="Claim"/> with the constraint name <c>claim_pkey</c>.
    /// </summary>
    /// <param name="builder">The entity type builder.</param>
    private static void ConfigurePrimaryKeys(EntityTypeBuilder<Claim> builder)
    {
        builder.HasKey(static e => e.Id).HasName("claim_pkey");
    }

    /// <summary>
    ///     Configures the many-to-many relationship between <see cref="Claim"/> and <see cref="Role"/>
    ///     via the <c>claim_role</c> join table in the <c>identity</c> schema with <see cref="DeleteBehavior.Restrict"/>.
    /// </summary>
    /// <param name="builder">The entity type builder.</param>
    private static void ConfigureClaimRolesRelation(EntityTypeBuilder<Claim> builder)
    {
        builder.HasMany(static d => d.Roles).WithMany(static p => p.Claims)
            .UsingEntity<Dictionary<string, object>>(
                "ClaimRole", static r => r.HasOne<Role>().WithMany()
                    .HasForeignKey("RoleId")
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("claim_role_role_id_fkey"), static l => l.HasOne<Claim>().WithMany()
                    .HasForeignKey("ClaimId")
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("claim_role_claim_id_fkey"), static j =>
                {
                    j.HasKey("ClaimId", "RoleId").HasName("claim_role_pkey");
                    j.ToTable("claim_role", "identity");
                    j.IndexerProperty<short>("ClaimId").HasColumnName("claim_id");
                    j.IndexerProperty<short>("RoleId").HasColumnName("role_id");
                });
    }
}